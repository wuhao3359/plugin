using Dalamud.Logging;
using System.Threading;
using WoAutoCollectionPlugin.Data;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class CommonBot
    {
        private GameData GameData { get; init; }
        private KeyOperates KeyOperates { get; init; }

        private bool closed = false;

        public CommonBot(GameData GameData)
        {
            this.GameData = GameData;
            KeyOperates = new KeyOperates(GameData);
            Init();
        }

        public void Init()
        {
            closed = false;
        }

        public void Closed()
        {
            closed = true;
        }

        // 修理
        public bool Repair() {
            bool flag = true;
            if (closed)
            {
                PluginLog.Log($"Repair stopping");
                return flag;
            }

            KeyOperates.KeyMethod(Keys.plus_key);
            Thread.Sleep(200);
            if (RepairUi.AllRepairButton())
            {
                Thread.Sleep(300);
                KeyOperates.KeyMethod(Keys.num0_key);
                KeyOperates.KeyMethod(Keys.num0_key);
                Thread.Sleep(4000);
            }
            else {
                flag = false;
            }
            KeyOperates.KeyMethod(Keys.esc_key);
            Thread.Sleep(500);
            return flag;
        }

        public bool NpcRepair()
        {
            bool flag = true;
            KeyOperates.KeyMethod(Keys.num0_key);
            KeyOperates.KeyMethod(Keys.num0_key);
            KeyOperates.KeyMethod(Keys.num2_key);
            Thread.Sleep(1500);
            if (RepairUi.AllRepairButton())
            {
                Thread.Sleep(300);
                KeyOperates.KeyMethod(Keys.num0_key);
                KeyOperates.KeyMethod(Keys.num0_key);
                Thread.Sleep(500);
            }
            else
            {
                flag = false;
            }
            KeyOperates.KeyMethod(Keys.esc_key);
            Thread.Sleep(500);
            return flag;
        }

        // 精制
        public bool ExtractMateria(int count)
        {
            if (closed)
            {
                PluginLog.Log($"ExtractMateria stopping");
                return true;
            }
            KeyOperates.KeyMethod(Keys.minus_key);
            Thread.Sleep(1000);
            for (int i = 0; i < count; i++) {
                KeyOperates.KeyMethod(Keys.num0_key);
                KeyOperates.KeyMethod(Keys.num4_key);
                KeyOperates.KeyMethod(Keys.num0_key);
                Thread.Sleep(3500);
            }
            KeyOperates.KeyMethod(Keys.esc_key);
            Thread.Sleep(500);
            return true;
        }

        public bool CraftUploadAndExchange(string craftName, int exchangeItem)
        {
            if (closed)
            {
                PluginLog.Log($"CraftUploadAndExchange stopping");
                return true;
            }
            if (CraftUpload(craftName) && CraftExchange(exchangeItem)) {
                return true;
            }
            return false;
        }

        // 交收藏品
        public bool CraftUpload(string craftName)
        {
            PluginLog.Log($"CraftUploading");
            (uint Category, uint Sub) = Items.UploadApply(craftName);
            if (Category == 0 && Sub == 0) {
                return false;
            }

            Thread.Sleep(1000);
            KeyOperates.KeyMethod(Keys.num1_key);
            KeyOperates.KeyMethod(Keys.num0_key);
            Thread.Sleep(2500);
            for (int i = 0; i < Category; i++) {
                KeyOperates.KeyMethod(Keys.num2_key);
            }

            for (int i = 0; i < Sub; i++)
            {
                KeyOperates.KeyMethod(Keys.num2_key);
            }

            int n = 0;
            while (!RecipeNoteUi.SelectYesNoIsOpen() && n < 30)
            {
                KeyOperates.KeyMethod(Keys.num0_key);
                Thread.Sleep(500);
                n++;
                if (closed)
                {
                    PluginLog.Log($"upload stopping");
                    return true;
                }
            }

            KeyOperates.KeyMethod(Keys.num0_key);
            KeyOperates.KeyMethod(Keys.esc_key);
            Thread.Sleep(1000);
            return true;
        }

        // 交换道具
        public bool CraftExchange(int item)
        {
            PluginLog.Log($"CraftExchanging");
            (uint Category, uint Sub) = Items.ExchangeApply(item);
            if (Category == 0 && Sub == 0)
            {
                return false;
            }

            Thread.Sleep(2000);
            KeyOperates.KeyMethod(Keys.num3_key);
            KeyOperates.KeyMethod(Keys.num0_key);
            KeyOperates.KeyMethod(Keys.num0_key);
            Thread.Sleep(2000);

            if (closed)
            {
                PluginLog.Log($"exchange stopping");
                return true;
            }

            if (item == 1) {
                KeyOperates.KeyMethod(Keys.num0_key);
                KeyOperates.KeyMethod(Keys.num8_key);
                KeyOperates.KeyMethod(Keys.num0_key);

                for (int i = 0; i < Sub; i++)
                {
                    KeyOperates.KeyMethod(Keys.num2_key);
                }

                KeyOperates.KeyMethod(Keys.num6_key);
                KeyOperates.KeyMethod(Keys.num9_key);
                KeyOperates.KeyMethod(Keys.num4_key);
                KeyOperates.KeyMethod(Keys.num0_key);
                KeyOperates.KeyMethod(Keys.num4_key);
                KeyOperates.KeyMethod(Keys.num0_key);
            }

            KeyOperates.KeyMethod(Keys.esc_key);
            Thread.Sleep(1000);
            return true;
        }
    }
}