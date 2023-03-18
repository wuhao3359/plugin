using System;
using System.Threading;
using WoAutoCollectionPlugin.Utility;
using static FFXIVClientStructs.FFXIV.Client.UI.AddonRelicNoteBook;

namespace WoAutoCollectionPlugin.Bot
{
    public class MarketBot
    {
        static long NextClickAt = 0;
        private bool closed = false;

        public MarketBot()
        {
            
        }

        public void init()
        {
            closed = false;
        }

        public void StopScript() {
            closed = true;
        }

        public void Script()
        {
            // TODO
            // 传送并移动到雇员铃

            // 打开雇员铃
            WoAutoCollectionPlugin.GameData.CommonBot.SetTarget("雇员铃");
            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);

            // 点击雇员 判断 RetainerSellList 正在出售数量selling 根据selling进行改价 然后上架20-selling商品
            for (int i = 1; i <= 2; i++) {
                // 雇员选择
                Clicker.SelectRetainerByIndex(i);
                Thread.Sleep(3000);
                // 改价
                Clicker.UpdateRetainerSellList(i, out var succeed, out int selling);

                // 上架
                Clicker.PutUpRetainerSellList(i, 20 - selling, out succeed);
            }
            // PS: 上架商品根据配置

            // 完成结束

        }

        public void RunScript()
        {
            Script();
        }

        internal static bool IsUpdatePriceAllowed()
        {
            return Environment.TickCount64 > NextClickAt;
        }

        internal static void RecordClickTime(int time = 750)
        {
            time.ValidateRange(100, int.MaxValue);
            NextClickAt = Environment.TickCount64 + time;
        }

    }
}