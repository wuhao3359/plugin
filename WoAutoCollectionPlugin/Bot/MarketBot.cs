using System;
using System.Threading;

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

            // 点击1号雇员 判断 RetainerSellList 物品数量 根据数量进行改价 然后上架商品

            for (int i = 1; i <= 2; i++) {
                Clicker.SelectRetainerByIndex(i);
                Thread.Sleep(3000);
                Clicker.UpdateRetainerSellList(i, out var succeed);

                //
                Clicker.PutUpRetainerSellList(i, out succeed);
            }
            


            // 点击2号雇员 重复上述步骤
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