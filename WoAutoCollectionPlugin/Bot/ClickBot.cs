using Dalamud.Logging;
using System.Threading;
using WoAutoCollectionPlugin.Data;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class ClickBot
    {
        private KeyOperates KeyOperates { get; init; }

        public ClickBot(KeyOperates KeyOperates)
        {
            this.KeyOperates = KeyOperates;
        }

        // 修理
        public int ClickAllRepairButton()
        {
            if (RepairUi.AllRepairButton()) {
                int n = 0;
                while (!CommonUi.AddonSelectYesnoIsOpen() && n < 5) {
                    Thread.Sleep(500);
                    KeyOperates.KeyMethod(Keys.num0_key);
                }
                if (CommonUi.AddonSelectYesnoIsOpen()) {
                    return 1;
                }
                return 0;
            }
            PluginLog.Log("AllRepairButton not exist");
            return -1;
        }

    }
}