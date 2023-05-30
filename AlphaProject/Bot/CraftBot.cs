using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using AlphaProject.Data;
using AlphaProject.Managers;
using AlphaProject.Ui;
using AlphaProject.Utility;
using static Lumina.Excel.GeneratedSheets.Recipe;
using AlphaProject.Helper;

namespace AlphaProject.Bot
{
    internal class CraftBot
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
                if (AlphaProject.Configuration.CraftType == 1)
                {
                    PluginLog.Log($"根据名称普通制作...");
                    RunCraftScriptByName();
                }
                else if (AlphaProject.Configuration.CraftType == 2)
                {
                    PluginLog.Log($"根据名称快速制作...");
                    RunCraftScriptByName();
                }
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

        public unsafe void RunCraftScriptByName()
        {
            int error = 0;
            int craftX = 0;
            uint recipeId = RecipeNoteUi.SearchRecipeId(AlphaProject.Configuration.RecipeName);
            PluginLog.Log($"---> {AlphaProject.Configuration.RecipeName}, {recipeId}");
            if (recipeId == 0) {
                PluginLog.Error($"not found recipe...");
                return;
            }
            while (!closed)
            {
                Thread.Sleep(new Random().Next(1200, 2100));

                if (BagManager.InventoryRemaining() <= 5)
                {
                    PluginLog.Log("背包容量不足...");
                    closed = true;
                    return;
                }

                if (CraftHelper.CheckForRecipeIngredients(recipeId, out List<uint> lackItems)) {
                    if (lackItems.Count > 0) {
                        PluginLog.Warning("原材料不足...");
                        AlphaProject.AP.TM.AddTask(lackItems);
                    }
                }

                if (!CraftHelper.RecipeNoteWindowOpen() && !CraftHelper.SynthesisWindowOpen())
                    {
                    if (!DalamudApi.Condition[ConditionFlag.Crafting])
                    {
                        RecipeNoteUi.OpenRecipeNote(recipeId);
                        Thread.Sleep(new Random().Next(1200, 2000));
                    }
                }

                if (CraftHelper.RecipeNoteWindowOpen())
                {
                    // TODO choose
                    RecipeNoteUi.SynthesizeButton();
                    if (error > 3) {
                        closed = true;
                        PluginLog.Log($"停止 error: {error}");
                    }
                    Thread.Sleep(new Random().Next(1800, 2500));
                }

                if (RecipeNoteUi.SynthesisIsOpen())
                {
                    error = 0;
                    int n = 0;
                    while (RecipeNoteUi.SynthesisIsOpen())
                    {
                        Thread.Sleep(new Random().Next(1000, 1200));
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
                    PluginLog.Log($"Finish: {craftX} Item: {AlphaProject.Configuration.RecipeName}, {recipeId}");
                    craftX++;
                }
                else {
                    error++;
                }
                Thread.Sleep(new Random().Next(500, 800));

                // 修理装备
                if (CommonUi.NeedsRepair())
                {
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
                }

                if (BagManager.InventoryRemaining() <= 5)
                {
                    if (RecipeNoteUi.RecipeNoteIsOpen())
                    {
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                        Thread.Sleep(new Random().Next(900, 1200));
                    }
                    Thread.Sleep(new Random().Next(900, 1200));
                    // 上交收藏品和交换道具
                    //if (AlphaProject.Configuration.CraftType == 2) {
                    //    if (!AlphaProject.GameData.CommonBot.CraftUploadAndExchange())
                    //    {
                    //        PluginLog.Log($"params error... plz check");
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        PluginLog.Log($"CraftUploadAndExchange End.");
                    //    }
                    //}
                    // 上交重建品和交换道具 TODO
                }
                Thread.Sleep(new Random().Next(900, 1200));

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