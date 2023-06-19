using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using AlphaProject.SeFunctions;
using AlphaProject.Ui;
using AlphaProject.Utility;
using AlphaProject.Enums;
using AlphaProject.Helper;

namespace AlphaProject.Bot
{
    public static class MarketBot
    {
        static int NextClickAt = 0;
        private static bool Closed = false;

        public static void Init()
        {
            Closed = false;
            Clicker.Init();
        }

        public static void StopScript() {
            Closed = true;
            Clicker.Stop();
        }

        public static void Script()
        {
            Init();
            // 传送并移动到雇员铃
            Teleporter.Teleport(Positions.RetainerBellTp);
            KeyOperates.MovePositions(Positions.RetainerBell, false);
            Thread.Sleep(2000 + new Random().Next(200, 500));
            // 打开雇员铃
            CommonHelper.SetTarget("雇员铃");
            KeyOperates.KeyMethod(Keys.num0_key);
            KeyOperates.KeyMethod(Keys.num0_key);
            Thread.Sleep(1000 + new Random().Next(200, 500));
            // 点击雇员 判断 RetainerSellList 正在出售数量selling 根据selling进行改价 然后上架20-selling商品
            if (DalamudApi.Condition[ConditionFlag.OccupiedSummoningBell])
            {
                for (int i = 1; i <= 2; i++)
                {
                    Thread.Sleep(2000 + new Random().Next(200, 800));
                    if (CommonUi.AddonRetainerListIsOpen())
                    {
                        Clicker.SelectRetainerByIndex(i);
                        Thread.Sleep(2500 + new Random().Next(200, 800));
                        KeyOperates.KeyMethod(Keys.num0_key);
                        Thread.Sleep(1000 + new Random().Next(200, 500));
                        if (CommonUi.AddonSelectStringIsOpen())
                        {
                            CommonUi.SelectString3Button();
                            Thread.Sleep(2000 + new Random().Next(200, 500));
                            // 改价
                            Clicker.UpdateRetainerSellList(i, out var succeed, out List<(uint, int)> sellingList);
                            Thread.Sleep(3000 + new Random().Next(200, 500));
                            foreach ((uint itemId, int itemCount) in sellingList)
                            {
                                PluginLog.Log($"itemId: {itemId}, itemCount: {itemCount}");
                            }

                            //if (succeed)
                            //{
                            //    上架 PS: 上架商品根据配置 TODO
                            //    Clicker.PutUpRetainerSellList(i, sellingList, out succeed);
                            //}
                            //else
                            //{
                            //    PluginLog.Error("change price error!!!");
                            //}
                        }
                    }
                    Clicker.CloseMarketAddon();
                }
            }
            else {
                PluginLog.Error("change price failed");
            }
        }

        public static void RunScript(int NextClick)
        {
            PluginLog.Log($"Configuration Chanege: {AlphaProject.Configuration.AutoMarket}");
            if (!AlphaProject.Configuration.AutoMarket) {
                return;
            }

            AlphaProject.status = (byte)TaskState.PRICE;
            NextClickAt++;
            if (NextClickAt <= NextClick)
            {
                PluginLog.Log($"start change end");
                return;
            }
            PluginLog.Log($"start change price");
            NextClickAt = 0;
            Script();
            int n = 0;
            while ((DalamudApi.Condition[ConditionFlag.OccupiedSummoningBell] || DalamudApi.Condition[ConditionFlag.OccupiedInQuestEvent]) && n < 5) {
                KeyOperates.KeyMethod(Keys.esc_key);
                Thread.Sleep(1000);
                n++;
            }
            AlphaProject.status = (byte)TaskState.READY;
        }
    }
}