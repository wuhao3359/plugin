using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WoAutoCollectionPlugin.Data;
using WoAutoCollectionPlugin.Managers;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class CraftBot
    {
        //private GameData GameData { get; init; }
        private KeyOperates KeyOperates { get; init; }

        private CommonBot? CommonBot;

        private static bool closed = false;

        public CraftBot(GameData GameData)
        {
            //this.GameData = GameData;
            KeyOperates = new KeyOperates(GameData);
            CommonBot = new CommonBot(KeyOperates);
        }

        public void Init()
        {
            closed = false;
            CommonBot.Init();
        }

        public void StopScript()
        {
            closed = true;
            CommonBot.StopScript();
        }

        public void CraftScript(string args)
        {
            closed = false;
            while (!closed)
            {
                try
                {
                    Init();

                    string[] str = args.Split(' ');
                    int pressKey = int.Parse(str[0]) + 48;
                    string recipeName = str[1];
                    int exchangeItem = 10;
                    PluginLog.Log($"{pressKey} {recipeName}");
                    if (str.Length > 2)
                    {
                        exchangeItem = int.Parse(str[2]);
                    }
                    bool result = int.TryParse(recipeName, out var id);
                    if (result)
                    {
                        PluginLog.Log($"根据配方制作...");
                        RunCraftScript(pressKey, id, exchangeItem);
                    }
                    else {
                        PluginLog.Log($"根据名称制作...");
                        RunCraftScriptByName(pressKey, recipeName, exchangeItem);
                    }
                    
                }
                catch (Exception e)
                {
                    PluginLog.Error($"error!!!\n{e}");
                }
            }
        }

        public void QuickCraftByName(string Name, (int Id, string Name, int Quantity, bool Craft)[] LowCraft) {
            int n = 0;
            while (!closed && n < 10) {
                n++;
                if (ItemQuantityEnough(LowCraft))
                {
                    closed = false;
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
            while (!closed && BagManager.InventoryRemaining() > 10)
            {
                Thread.Sleep(1000);
                if (closed)
                {
                    PluginLog.Log($"craft stopping");
                    return;
                }
                int n = 0;
                while (!RecipeNoteUi.RecipeNoteIsOpen() && n < 10)
                {
                    uint recipeId = RecipeNoteUi.SearchRecipeId(recipeName);
                    PluginLog.Log($"--- {recipeName}, {recipeId}");
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
                KeyOperates.KeyMethod(Byte.Parse(pressKey.ToString()));

                if (recipeName == "")
                    recipeName = RecipeNoteUi.GetItemName();

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

                if (BagManager.InventoryRemaining() <= 10)
                {
                    Thread.Sleep(1000);
                    if (RecipeNoteUi.RecipeNoteIsOpen())
                    {
                        KeyOperates.KeyMethod(Keys.esc_key);
                    }
                    else
                    {
                        continue;
                    }

                    //CommonBot.RepairAndExtractMateria();

                    // 上交收藏品和交换道具
                    if (recipeName.Contains("收藏用") && exchangeItem > 0)
                    {
                        if (!CommonBot.CraftUploadAndExchange(recipeName, exchangeItem))
                        {
                            PluginLog.Log($"params error... plz check");
                            return;
                        }
                        else {
                            PluginLog.Log($"CraftUploadAndExchange End.");
                        }
                    }
                    // 上交重建品和交换道具 TODO
                }
                Thread.Sleep(1000);
            }
        }

        public bool ItemQuantityEnough((int Id, string Name, int Quantity, bool Craft)[] LowCraft) {
            for (int i = 0; i < LowCraft.Length; i++)
            {
                int quantity = BagManager.GetItemQuantityInContainer((uint)LowCraft[i].Id);
                if (quantity < LowCraft[i].Quantity)
                {
                    return false;
                }
            }
            return true;
        }

        public void RunCraftScript(int pressKey, int id, int exchangeItem) {
            (int Id, int MaxBackPack, string Name, uint Job, string JobName, uint Lv, bool QuickCraft, (int Id, string Name, int Quantity, bool Craft)[] LowCraft) = RecipeItems.GetMidCraftItems(31652);

            if (CraftPreCheck(LowCraft)) {
                // 切换职业 
                if (!CommonUi.CurrentJob(Job))
                {
                    WoAutoCollectionPlugin.Executor.DoGearChange(JobName);
                    Thread.Sleep(500);
                }
                if (QuickCraft)
                {
                    QuickCraftByName(Name, LowCraft);
                }
                else {
                    RunCraftScriptByName(pressKey, Name, exchangeItem);
                }
                
            }
        }

        public bool CraftPreCheck((int Id, string Name, int Quantity, bool Craft)[] LowCraft)
        {
            foreach ((int Id, string Name, int Quantity, bool Craft) in LowCraft)
            {
                // name的数量小于一定数量
                return false;
            }
            return true;
        }

        public (int, int, string, uint, string, uint, bool, (int, string, int, bool)[]) GetData(int id)
        {
            if (id == 0)
            {
                List<int> list = RecipeItems.GetCraftItemIds();
                List<int> li = new();
                foreach (int i in list)
                {
                    (int Id, int MaxBackPack, string Name, uint Job, string JobName, uint Lv, bool QuickCraft, (int Id, string Name, int Quantity, bool Craft)[] Craft) = RecipeItems.GetMidCraftItems(i);
                    if (QuickCraft && MaxBackPack > BagManager.GetInventoryItemCount((uint)i))
                    {
                        li.Add(i);
                    }
                }

                Random rd = new();
                int r = rd.Next(li.Count);
                id = li[r];
                PluginLog.Log($"随机生产中间物ID: {id}");
                return RecipeItems.GetMidCraftItems(id);
            }
            else
            {
                return RecipeItems.GetMidCraftItems(id);
            }

            (int Id, string Name, int Quantity, bool Craft)[] Craft0 = {};
            return (0, 0, "", 0, "", 0, false, Craft0);
        }
    }
}