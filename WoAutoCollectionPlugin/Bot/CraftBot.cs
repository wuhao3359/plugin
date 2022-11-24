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

        public CraftBot() {}

        public void Init()
        {
            closed = false;
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
                } else if (t == "3") {
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

        public void QuickCraftByName(string Name, (int Id, string Name, int Quantity, bool Craft)[] LowCraft) {
            int n = 0;
            while (!closed && n < 10) {
                n++;
                if (!BagManager.QickItemQuantityEnough(LowCraft))
                {
                    closed = true;
                    break;
                }
                RunQuickCraftScriptByName(Name);
                Thread.Sleep(5000);
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

            Thread.Sleep(500);
            if (RecipeNoteUi.RecipeNoteIsOpen())
            {
                RecipeNoteUi.QuickSynthesizeButton();
                n = 0;
                // TODO 快速生产
                while (RecipeNoteUi.RecipeNoteIsOpen() && n < 3)
                {
                    n++;
                    Thread.Sleep(500);
                    if (closed)
                    {
                        PluginLog.Log($"craft stopping");
                        return;
                    }
                }
            }
            else
            {
                PluginLog.Log($"RecipeNote not open, continue");
                return;
            }

            //  0 9 0 2 0 TODO
        }

        public void RunCraftScriptByName(int pressKey, string recipeName, int exchangeItem)
        {
            int i = 0;
            while (!closed)
            {
                Thread.Sleep(1000);
                if (closed)
                {
                    PluginLog.Log($"craft stopping");
                    return;
                }

                if (BagManager.InventoryRemaining() > 5)
                {
                    int n = 0;
                    while (!RecipeNoteUi.RecipeNoteIsOpen() && n < 10)
                    {
                        uint recipeId = RecipeNoteUi.SearchRecipeId(recipeName);
                        PluginLog.Log($"---> {recipeName}, {recipeId}");
                        RecipeNoteUi.OpenRecipeNote(recipeId);

                        Thread.Sleep(1000);
                        if (closed)
                        {
                            PluginLog.Log($"craft stopping");
                            return;
                        }
                        n++;
                    }

                    // TODO Select Item

                    Thread.Sleep(500);
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
                    else
                    {
                        PluginLog.Log($"RecipeNote not open, continue");
                        continue;
                    }

                    Thread.Sleep(1800);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Byte.Parse(pressKey.ToString()));

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
                    PluginLog.Log($"Finish: {i} Item: {recipeName}");
                    i++;
                    Thread.Sleep(1000);

                    if (BagManager.InventoryRemaining() <= 5)
                    {
                        Thread.Sleep(1000);
                        if (RecipeNoteUi.RecipeNoteIsOpen())
                        {
                            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                        }

                        // 精制+修理
                        WoAutoCollectionPlugin.GameData.CommonBot.RepairAndExtractMateriaInCraft();

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
                    PluginLog.Log($"背包容量不足, 任务停止...");
                    closed = true;
                }
            }
        }

        public void RunCraftScript() {
            List<int> list = RecipeItems.GetAllQuickCraftItems();
            
            int id = 0;
            (int Id, string Name, int Quantity, bool Craft)[] quickCraft;

            if (list.Count > 0) { 
                for (int i = 0; i < 5; i++)
                {
                    Random rd = new();
                    int r = rd.Next(list.Count);
                    id = list[r];
                    PluginLog.Log($"随机生产ID: {r} {id}");
                    (int Id, int MaxBackPack, string Name, uint Job, string JobName, uint Lv, bool QuickCraft, (int Id, string Name, int Quantity, bool Craft)[] LowCraft) = RecipeItems.GetMidCraftItems(id);

                    if (BagManager.QickItemQuantityEnough(LowCraft))
                    {
                        if (!CommonUi.CurrentJob(Job))
                        {
                            WoAutoCollectionPlugin.Executor.DoGearChange(JobName);
                            Thread.Sleep(500);
                        }
                        break;
                    }
                }
            }

            if (id > 0)
            {
                (int Id, int MaxBackPack, string Name, uint Job, string JobName, uint Lv, bool QuickCraft, (int Id, string Name, int Quantity, bool Craft)[] LowCraft) = RecipeItems.GetMidCraftItems(id);
                QuickCraftByName(Name, LowCraft);
            }
            else {
                PluginLog.Log($"未找到生产物品...");
            }
        }


        public void CheckScript(uint recipeId) {
            WoAutoCollectionPlugin.GameData.Recipes.TryGetValue(recipeId, out var r);
            UnkData5Obj[] UnkData5 = r.UnkData5;
            if (UnkData5.Length > 0)
            {
                foreach (UnkData5Obj obj in UnkData5)
                {
                    PluginLog.Log($"ItemIngredient : {obj.ItemIngredient}, AmountIngredient : {obj.AmountIngredient}");
                    if (BagManager.GetItemQuantityInContainer((uint)obj.ItemIngredient) < obj.AmountIngredient * 100)
                    {
                        CheckScript(uint.Parse(obj.ItemIngredient.ToString()));
                        if (r.CanQuickSynth)
                        {
                            // 快速制作
                        }
                        else { 
                            // 普通制作
                        }
                    }
                }
            }
            else {
                if (BagManager.GetItemQuantityInContainer(recipeId) < 666)
                {
                    // 原材料采集
                    PluginLog.Log($"执行采集任务...");
                    try
                    {
                        WoAutoCollectionPlugin.GameData.GatherBot.RunNormalScript((int)recipeId, 90);
                    }
                    catch (Exception e)
                    {
                        PluginLog.Error($"采集任务, error!!!\n{e}");
                    }
                }
            }
        }
    }
}