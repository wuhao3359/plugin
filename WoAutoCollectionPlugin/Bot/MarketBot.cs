using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using WoAutoCollectionPlugin.SeFunctions;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class MarketBot
    {
        static int NextClickAt = 0;
        private bool closed = false;

        public MarketBot()
        {
            
        }

        public void Init()
        {
            closed = false;
            Clicker.Init();
        }

        public void StopScript() {
            closed = true;
            Clicker.Stop();
        }

        public void Script()
        {
            // 传送并移动到雇员铃
            Teleporter.Teleport(Positions.RetainerBellTp);
            MovePositions(Positions.RetainerBell, false);
            Thread.Sleep(2000);
            // 打开雇员铃
            WoAutoCollectionPlugin.GameData.CommonBot.SetTarget("雇员铃");
            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
            Thread.Sleep(1000);
            // 点击雇员 判断 RetainerSellList 正在出售数量selling 根据selling进行改价 然后上架20-selling商品
            if (DalamudApi.Condition[ConditionFlag.OccupiedSummoningBell])
            {
                for (int i = 1; i <= 2; i++)
                {
                    Thread.Sleep(2000);
                    if (CommonUi.AddonRetainerListIsOpen())
                    {
                        Clicker.SelectRetainerByIndex(i);
                        Thread.Sleep(3000);
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                        Thread.Sleep(1000);
                        if (CommonUi.AddonSelectStringIsOpen())
                        {
                            CommonUi.SelectString3Button();
                            Thread.Sleep(2000);
                            // 改价
                            Clicker.UpdateRetainerSellList(i, out var succeed, out List<(uint, int)> sellingList);
                            Thread.Sleep(3000);
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

        public void RunScript(int NextClick)
        {
            WoAutoCollectionPlugin.status = "change price";
            NextClickAt++;
            if (NextClickAt <= NextClick)
            {
                return;
            }
            PluginLog.Log($"start change price");
            NextClickAt = 0;
            Script();
            int n = 0;
            while ((DalamudApi.Condition[ConditionFlag.OccupiedSummoningBell] || DalamudApi.Condition[ConditionFlag.OccupiedInQuestEvent]) && n < 5) {
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                Thread.Sleep(1000);
                n++;
            }
            WoAutoCollectionPlugin.status = "";
        }

        private Vector3 MovePositions(Vector3[] Path, bool UseMount)
        {
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            ushort SizeFactor = WoAutoCollectionPlugin.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            Vector3 position = WoAutoCollectionPlugin.GameData.KeyOperates.GetUserPosition(SizeFactor);
            for (int i = 0; i < Path.Length; i++)
            {
                if (closed)
                {
                    PluginLog.Log($"中途结束");
                    return WoAutoCollectionPlugin.GameData.KeyOperates.GetUserPosition(SizeFactor);
                }
                position = WoAutoCollectionPlugin.GameData.KeyOperates.MoveToPoint(position, Path[i], territoryType, UseMount, false);
            }
            return position;
        }

    }
}