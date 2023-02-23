using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WoAutoCollectionPlugin.Data;
using WoAutoCollectionPlugin.Managers;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.Utility;
using static Lumina.Excel.GeneratedSheets.Recipe;

namespace WoAutoCollectionPlugin.Bot
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
            WoAutoCollectionPlugin.GameData.CommonBot.Init();
        }

        public void StopScript()
        {
            closed = true;
            WoAutoCollectionPlugin.GameData.CommonBot.StopScript();
        }

        public void CraftScript(string args)
        {
            closed = false;
            try
            {
                Init();

                // 参数解析
                string command = "craft";
                WoAutoCollectionPlugin.GameData.param = Util.CommandParse(command, args);

                WoAutoCollectionPlugin.GameData.param.TryGetValue("pressKey", out var p);
                WoAutoCollectionPlugin.GameData.param.TryGetValue("type", out var t);
                WoAutoCollectionPlugin.GameData.param.TryGetValue("recipeName", out var r);
                WoAutoCollectionPlugin.GameData.param.TryGetValue("exchangeItem", out var e);
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
                {
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
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
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
            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.e_key);

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
                        Thread.Sleep(2000);
                    }

                    if (RecipeNoteUi.SynthesisIsOpen())
                    {
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Byte.Parse(pressKey.ToString()));
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
                                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.e_key);
                                Thread.Sleep(2000);
                            }
                        }
                        PluginLog.Log($"Finish: {i} Item: {recipeName}");
                        i++;
                    }
                    else {
                        needHQ = true;
                    }
                    Thread.Sleep(500);

                    // 修理装备
                    if (CommonUi.NeedsRepair())
                    {
                        needHQ = true;
                        PluginLog.Log($"开始修理装备");
                        if (RecipeNoteUi.RecipeNoteIsOpen()) {
                            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                            Thread.Sleep(1500);
                        }
                        WoAutoCollectionPlugin.GameData.param.TryGetValue("repair", out var rep);
                        PluginLog.Log($"修理装备配置: {rep}");
                        if (rep == "1")
                        {
                            if (WoAutoCollectionPlugin.GameData.param.TryGetValue("type", out var t) && t == "2") {
                                WoAutoCollectionPlugin.GameData.CommonBot.MovePositions(Position.RepairNPCA, false);
                            }
                            WoAutoCollectionPlugin.GameData.CommonBot.NpcRepair("阿里斯特尔");
                        }
                        else if (rep == "99")
                        {
                            WoAutoCollectionPlugin.GameData.CommonBot.Repair();
                        }
                    }

                    // 魔晶石精制
                    int em = CommonUi.CanExtractMateria();
                    if (em > 2) {
                        WoAutoCollectionPlugin.GameData.CommonBot.ExtractMateria(em);
                        needHQ = true;
                    }

                    if (BagManager.InventoryRemaining() <= 5)
                    {
                        if (RecipeNoteUi.RecipeNoteIsOpen())
                        {
                            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                            Thread.Sleep(1000);
                        }
                        Thread.Sleep(1000);
                        // 上交收藏品和交换道具
                        if (WoAutoCollectionPlugin.GameData.param.TryGetValue("type", out var t)) {
                            if (t == "2") {
                                if (!WoAutoCollectionPlugin.GameData.CommonBot.CraftUploadAndExchange())
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
                    if (WoAutoCollectionPlugin.GameData.param.TryGetValue("type", out var t))
                    {
                        if (t == "2")
                        {
                            WoAutoCollectionPlugin.GameData.CommonBot.CraftUploadAndExchange();
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
                        WoAutoCollectionPlugin.Executor.DoGearChange(JobName);
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

        public void CheckScript(uint recipeId) {
            //WoAutoCollectionPlugin.GameData.Recipes.TryGetValue(recipeId, out var r);
            //UnkData5Obj[] UnkData5 = r.UnkData5;
            //if (UnkData5.Length > 0)
            //{
            //    foreach (UnkData5Obj obj in UnkData5)
            //    {
            //        PluginLog.Log($"ItemIngredient : {obj.ItemIngredient}, AmountIngredient : {obj.AmountIngredient}");
            //        if (BagManager.GetItemQuantityInContainer((uint)obj.ItemIngredient) < obj.AmountIngredient * 100)
            //        {
            //            CheckScript(uint.Parse(obj.ItemIngredient.ToString()));
            //            if (r.CanQuickSynth)
            //            {
            //                // 快速制作
            //            }
            //            else { 
            //                // 普通制作
            //            }
            //        }
            //    }
            //}
            //else {
            //    if (BagManager.GetItemQuantityInContainer(recipeId) < 666)
            //    {
            //        // 原材料采集
            //        PluginLog.Log($"执行采集任务...");
            //        try
            //        {
            //            WoAutoCollectionPlugin.GameData.GatherBot.RunNormalScript((int)recipeId, 90);
            //        }
            //        catch (Exception e)
            //        {
            //            PluginLog.Error($"采集任务, error!!!\n{e}");
            //        }
            //    }
            //}
        }
    }
}