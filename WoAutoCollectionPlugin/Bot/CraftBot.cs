using Dalamud.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
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

                    RunCraftScript(pressKey, recipeName, exchangeItem);
                }
                catch (Exception e)
                {
                    PluginLog.Error($"error!!!\n{e}");
                }
            }
        }

        public void RunCraftScript(int pressKey, string recipeName, int exchangeItem)
        {
            int i = 0;
            while (!closed && BagManager.InventoryRemaining() >= 10)
            {
                Thread.Sleep(1000);
                if (closed)
                {
                    PluginLog.Log($"craft stopping");
                    return;
                }
                while (!RecipeNoteUi.RecipeNoteIsOpen())
                {
                    uint recipeId = RecipeNoteUi.SearchRecipeId(recipeName);
                    PluginLog.Log($"{recipeName}, {recipeId}");
                    RecipeNoteUi.OpenRecipeNote(recipeId);

                    Thread.Sleep(1000);
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

                int n = 0;
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

                    CommonBot.RepairAndExtractMateria();

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
                }
                Thread.Sleep(1000);
            }
        }
    }
}