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
            CommonBot = new CommonBot(GameData);
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
                int n = 0;
                while (!RecipeNoteUi.RecipeNoteIsOpen() && n < 5) {
                    KeyOperates.KeyMethod(Keys.n_key);
                    Thread.Sleep(500);
                    n++;
                    if (closed)
                    {
                        PluginLog.Log($"craft stopping");
                        return;
                    }
                }

                n = 0;
                while (!RecipeNoteUi.SynthesizeButton() && n < 5) {
                    Thread.Sleep(300);
                    n++;
                    if (closed)
                    {
                        PluginLog.Log($"craft stopping");
                        return;
                    }
                }

                Thread.Sleep(500);
                // TODO Select HQ

                KeyOperates.KeyMethod(Keys.num0_key);

                n = 0;
                while (!RecipeNoteUi.SynthesisIsOpen() && n < 5) {
                    Thread.Sleep(200);
                    KeyOperates.KeyMethod(Keys.num0_key);
                    n++;
                    if (closed)
                    {
                        PluginLog.Log($"craft stopping");
                        return;
                    }
                }
                Thread.Sleep(500);
                KeyOperates.KeyMethod(Byte.Parse(pressKey.ToString()));

                if (craftName == "")
                    craftName = RecipeNoteUi.GetItemName();

                n = 0;
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
            KeyOperates.KeyMethod(Keys.esc_key);

            // 判断是否需要修理
            if (RepairUi.NeedsRepair()) {
                CommonBot.Repair();
            }

            // 判断是否需要精制
            int count = RepairUi.CanExtractMateria();
            if (count >= 5)
            {
                CommonBot.ExtractMateria(count);
            }

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