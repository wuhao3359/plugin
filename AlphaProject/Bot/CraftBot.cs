using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AlphaProject.Data;
using AlphaProject.Managers;
using AlphaProject.Ui;
using AlphaProject.Utility;
using static Lumina.Excel.GeneratedSheets.Recipe;
using AlphaProject.Helper;

namespace AlphaProject.Bot
{
    public class CraftBot
    {
        private static bool closed = false;

        private List<uint> jobIds = new();

        public CraftBot() {}

        public void Init()
        {
            closed = false;
            jobIds = new();
            AlphaProject.GameData.CommonBot.Init();
        }

        public void StopScript()
        {
            closed = true;
            AlphaProject.GameData.CommonBot.StopScript();
        }

        public void CraftScript(string args)
        {
            closed = false;
            try
            {
                Init();

                // 参数解析
                string command = Tasks.GCraft;
                AlphaProject.GameData.param = Util.CommandParse(command, args);

                AlphaProject.GameData.param.TryGetValue("pressKey", out var p);
                AlphaProject.GameData.param.TryGetValue("type", out var t);
                AlphaProject.GameData.param.TryGetValue("recipeName", out var r);
                AlphaProject.GameData.param.TryGetValue("exchangeItem", out var e);
                PluginLog.Log($"craft params: pressKey: {p}, type: {t}, recipeName: {r}, exchangeItem: {e}");
                int pressKey = int.Parse(p) + 48;
                string recipeName = r;
                int exchangeItem = int.Parse(e);

                if (t == "1")
                {
                    PluginLog.Log($"根据名称普通制作...");
                    RunCraftScriptByName(pressKey, recipeName, exchangeItem);
                }
                else if (t == "2")
                {   // type = 2 代表收藏品
                    PluginLog.Log($"根据名称普通制作...");
                    RunCraftScriptByName(pressKey, recipeName, exchangeItem);
                }
                else if (t == "3") {
                    PluginLog.Log($"根据名称快速制作...");
                    RunCraftScriptByName(pressKey, recipeName, exchangeItem);
                }

                //PluginLog.Log($"根据配方制作...");
                //RunCraftScript(pressKey, id, exchangeItem);

            }
            catch (Exception e)
            {
                PluginLog.Error($"error!!!\n{e}");
            }
        }

        public void QuickCraftByName(string Name, (int Id, string Name, int Quantity)[] LowCraft) {
            int n = 0;
            while (!closed && n < 1000) {
                n++;
                if (!BagManager.QickItemQuantityEnough(LowCraft))
                {
                    PluginLog.Log($"生产: {Name}, 材料不足...");
                    if (RecipeNoteUi.RecipeNoteIsOpen())
                    {
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                    }
                    break;
                }
                if (closed)
                {
                    PluginLog.Log($"quick craft stopping");
                    return;
                }
                RunQuickCraftScriptByName(Name);
                PluginLog.Log($"Finish: {n} Item: {Name}");
                Thread.Sleep(800);
            }
        }

        public void RunQuickCraftScriptByName(string Name) {
            int n = 0; 
            while (!RecipeNoteUi.RecipeNoteIsOpen() && n < 3)
            {
                uint recipeId = RecipeNoteUi.SearchRecipeId(Name);
                PluginLog.Log($"{Name}, {recipeId}");
                RecipeNoteUi.OpenRecipeNote(recipeId);

                Thread.Sleep(1000);
                if (closed)
                {
                    PluginLog.Log($"quick craft stopping");
                    return;
                }
                n++;
            }
            if (RecipeNoteUi.RecipeNoteIsOpen())
            {
                RecipeNoteUi.SynthesizeButton();
                while (RecipeNoteUi.RecipeNoteIsOpen())
                {
                    Thread.Sleep(500);
                    if (closed)
                    {
                        PluginLog.Log($"craft stopping");
                        return;
                    }
                }
            }
            Thread.Sleep(1800);
            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.e_key);

            n = 0;
            while (RecipeNoteUi.SynthesisIsOpen() && n < 100)
            {
                Thread.Sleep(500);
                if (closed)
                {
                    PluginLog.Log($"craft stopping");
                    return;
                }
            }
        }

        public void RunCraftScriptByName(int pressKey, string recipeName, int exchangeItem)
        {
            bool needHQ = true;
            int error = 0;
            int i = 0;
            uint recipeId = RecipeNoteUi.SearchRecipeId(recipeName);
            PluginLog.Log($"---> {recipeName}, {recipeId}");
            while (!closed)
            {
                Thread.Sleep(1500);
                if (closed)
                {
                    PluginLog.Log($"craft stopping");
                    return;
                }

                if (BagManager.InventoryRemaining() > 5)
                {
                    if (!RecipeNoteUi.RecipeNoteIsOpen() && !RecipeNoteUi.SynthesisIsOpen())
                    {
                        if (!DalamudApi.Condition[ConditionFlag.Crafting]) {
                            if (needHQ)
                            {
                                recipeId = RecipeNoteUi.SearchRecipeId(recipeName);
                                PluginLog.Log($"<=======> {recipeName}, {recipeId}");
                                Thread.Sleep(1000);
                            }
                            RecipeNoteUi.OpenRecipeNote(recipeId);
                        }
                        Thread.Sleep(1000);
                    }

                    if (RecipeNoteUi.RecipeNoteIsOpen())
                    {
                        if (needHQ) {
                            RecipeNoteUi.Material1HqButton();
                            Thread.Sleep(800);
                            needHQ = false;
                        }
                        RecipeNoteUi.SynthesizeButton();
                        if (error > 3) {
                            closed = true;
                            PluginLog.Log($"停止 error: {error}");
                        }
                        Thread.Sleep(2000);
                    }

                    if (RecipeNoteUi.SynthesisIsOpen())
                    {
                        error = 0;
                        AlphaProject.GameData.KeyOperates.KeyMethod(Byte.Parse(pressKey.ToString()));
                        int n = 0;
                        while (RecipeNoteUi.SynthesisIsOpen())
                        {
                            Thread.Sleep(1000);
                            if (closed)
                            {
                                PluginLog.Log($"craft stopping");
                                return;
                            }
                            n++;
                            if (n > 60)
                            {
                                AlphaProject.GameData.KeyOperates.KeyMethod(Keys.e_key);
                                Thread.Sleep(2000);
                            }
                        }
                        PluginLog.Log($"Finish: {i} Item: {recipeName}, {recipeId}");
                        i++;
                    }
                    else {
                        error++;
                        needHQ = true;
                    }
                    Thread.Sleep(500);

                    // 修理装备
                    if (CommonUi.NeedsRepair())
                    {
                        needHQ = true;
                        PluginLog.Log($"开始修理装备");
                        if (RecipeNoteUi.RecipeNoteIsOpen()) {
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                            Thread.Sleep(1500);
                        }
                        AlphaProject.GameData.param.TryGetValue("repair", out var rep);
                        PluginLog.Log($"修理装备配置: {rep}");
                        if (rep == "1")
                        {
                            if (AlphaProject.GameData.param.TryGetValue("type", out var t) && t == "2") {
                                AlphaProject.GameData.CommonBot.MovePositions(Positions.RepairNPCA, false);
                            }
                            AlphaProject.GameData.CommonBot.NpcRepair("阿里斯特尔");
                        }
                        else if (rep == "99")
                        {
                            AlphaProject.GameData.CommonBot.Repair();
                        }
                        else if (rep == "100")
                        {
                            AlphaProject.GameData.CommonBot.NpcRepair("修理工");
                        }
                    }

                    // 魔晶石精制
                    int em = CommonUi.CanExtractMateria();
                    if (em > 2) {
                        AlphaProject.GameData.CommonBot.ExtractMateria(em);
                        needHQ = true;
                    }

                    if (BagManager.InventoryRemaining() <= 5)
                    {
                        if (RecipeNoteUi.RecipeNoteIsOpen())
                        {
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                            Thread.Sleep(1000);
                        }
                        Thread.Sleep(1000);
                        // 上交收藏品和交换道具
                        if (AlphaProject.GameData.param.TryGetValue("type", out var t)) {
                            if (t == "2") {
                                if (!AlphaProject.GameData.CommonBot.CraftUploadAndExchange())
                                {
                                    PluginLog.Log($"params error... plz check");
                                    return;
                                }
                                else
                                {
                                    PluginLog.Log($"CraftUploadAndExchange End.");
                                }
                            }
                        } 
                        // 上交重建品和交换道具 TODO
                    }
                    Thread.Sleep(1000);
                }
                else {
                    // 尝试重试一次
                    if (AlphaProject.GameData.param.TryGetValue("type", out var t))
                    {
                        if (t == "2")
                        {
                            AlphaProject.GameData.CommonBot.CraftUploadAndExchange();
                        }
                    }
                    if (BagManager.InventoryRemaining() <= 5) {
                        PluginLog.Log($"背包容量不足, 任务停止...");
                        closed = true;
                    }
                }
            }
        }

        public void RunCraftScript() {
            List<int> list = RecipeItems.GetAllQuickCraftItems();
            int id = 0;
            
            for (int i = 0; i < list.Count; i++)
            {
                id = list[0];
                PluginLog.Log($"准备生产ID: {id}");
                (int Id, string Name, uint Job, string JobName, uint Lv, (int Id, string Name, int Quantity)[] LowCraft) = RecipeItems.GetMidCraftItems(id, jobIds);

                if (BagManager.QickItemQuantityEnough(LowCraft))
                {
                    if (!CommonUi.CurrentJob(Job))
                    {
                        Thread.Sleep(2000);
                        CommandProcessorHelper.DoGearChange(JobName);
                        Thread.Sleep(500);
                        PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                        if (player.Level < Lv)
                        {
                            jobIds.Add(Job);
                            id = 0;
                        }
                        else
                        {
                            PluginLog.Log($"准备生产: {Name}");
                            QuickCraftByName(Name, LowCraft);
                            break;
                        }
                    }
                }
            }
            if (id == 0) {
                PluginLog.Log($"没有找到合适物品...");
            }
        }
    }
}