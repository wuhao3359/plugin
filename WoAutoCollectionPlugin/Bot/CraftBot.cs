using Dalamud.Logging;
using System;
using System.Threading;
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

        public void StopCraftScript() {
            closed = true;
            CommonBot.Closed();
        }

        public void CraftScript(string args) {
            closed = false;
            while (!closed)
            {
                try
                {
                    RunCraftScript(args);
                }
                catch (Exception e)
                {
                    PluginLog.Error($"error!!!\n{e}");
                }
            }
        }

        public void RunCraftScript(string args)
        {
            Init();
            string[] str = args.Split(' ');
            int pressKey = int.Parse(str[0]) + 48;
            String recipeName = str[1];
            PluginLog.Log($"{pressKey} {recipeName}");
            int exchangeItem = 10;
            if (str.Length > 2) {
                exchangeItem = int.Parse(str[2]);
            }

            String craftName = recipeName;
            int i = 0;
            while (!closed) { 
                if (closed) {
                    PluginLog.Log($"craft stopping");
                    return;
                }
                while (!RecipeNoteUi.RecipeNoteIsOpen()) {
                    uint recipeId = RecipeNoteUi.SearchRecipeId(recipeName);
                    PluginLog.Log($"{recipeName}, {recipeId}");
                    RecipeNoteUi.OpenRecipeNote(recipeId);

                    Thread.Sleep(800);
                    if (closed)
                    {
                        PluginLog.Log($"craft stopping");
                        return;
                    }
                }

                // TODO Select Item

                Thread.Sleep(500);
                if (RecipeNoteUi.RecipeNoteIsOpen())
                {
                    RecipeNoteUi.SynthesizeButton();
                    while (RecipeNoteUi.RecipeNoteIsOpen()) {
                        Thread.Sleep(500);
                        if (closed)
                        {
                            PluginLog.Log($"craft stopping");
                            return;
                        }
                    }
                }
                else {
                    PluginLog.Log($"RecipeNote not open, continue");
                    continue;
                }

                Thread.Sleep(1800);
                KeyOperates.KeyMethod(Byte.Parse(pressKey.ToString()));

                if (craftName == "")
                    craftName = RecipeNoteUi.GetItemName();

                int n = 0;
                while (RecipeNoteUi.SynthesisIsOpen() && n < 100) {
                    Thread.Sleep(500);
                    if (closed)
                    {
                        PluginLog.Log($"craft stopping");
                        return;
                    }
                }
                PluginLog.Log($"Finish: {i} Item: {craftName}");
                i++;
                Thread.Sleep(1000);

                if (i >= 12) {
                    Thread.Sleep(1000);
                    if (RecipeNoteUi.RecipeNoteIsOpen())
                    {
                        KeyOperates.KeyMethod(Keys.esc_key);
                    }
                    else {
                        continue;
                    }

                    CommonBot.RepairAndExtractMateria();

                    // 上交收藏品和交换道具
                    if (craftName.Contains("收藏用") && exchangeItem > 0)
                    {
                        if (!CommonBot.CraftUploadAndExchange(craftName, exchangeItem))
                        {
                            PluginLog.Log($"params error... plz check");
                            return;
                        }
                    }
                }
            }
        }

        // TODO 日常使用
        // 1.生产白票
        // 2.自动采集缺少的材料
        // 3.中间自动采集限时材料
        public void DailyScript(string args) {
            closed = false;
            while (!closed)
            {
                try
                {
                    
                }
                catch (Exception e)
                {
                    PluginLog.Error($"error!!!\n{e}");
                }
            }
        }
    }
}