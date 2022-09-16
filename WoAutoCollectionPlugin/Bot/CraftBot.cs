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

        public void RunCraftScript(string args)
        {
            Init();
            string[] str = args.Split(' ');
            int pressKey = int.Parse(str[0]) + 48;
            int cycles = int.Parse(str[1]);
            PluginLog.Log($"{pressKey} {cycles}");
            int exchangeItem = 10;
            if (str.Length > 2) {
                exchangeItem = int.Parse(str[2]);
            }

            String craftName = "";
            for (int i = 0; i < cycles; i++) {
                if (closed) {
                    PluginLog.Log($"craft stopping");
                    return;
                }
                while (!RecipeNoteUi.RecipeNoteIsOpen()) {
                    KeyOperates.KeyMethod(Keys.n_key);
                    Thread.Sleep(500);
                    if (closed)
                    {
                        PluginLog.Log($"craft stopping");
                        return;
                    }
                }

                // TODO Select HQ

                Thread.Sleep(500);
                if (RecipeNoteUi.RecipeNoteIsOpen())
                {
                    RecipeNoteUi.SynthesizeButton();
                    while (RecipeNoteUi.RecipeNoteIsOpen()) {
                        KeyOperates.KeyMethod(Keys.num0_key);
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

                if (!RecipeNoteUi.SynthesisIsOpen()) {
                    Thread.Sleep(500);
                }
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
                Thread.Sleep(1000);
            }

            Thread.Sleep(1000);
            if (RecipeNoteUi.RecipeNoteIsOpen()) {
                KeyOperates.KeyMethod(Keys.esc_key);
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