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

namespace AlphaProject.Bot
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
            Init();
            // 传送并移动到雇员铃
            Teleporter.Teleport(Positions.RetainerBellTp);
            MovePositions(Positions.RetainerBell, false);
            Thread.Sleep(2000 + new Random().Next(200, 500));
            // 打开雇员铃
            AlphaProject.GameData.CommonBot.SetTarget("雇员铃");
            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
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
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
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

        public void RunScript(int NextClick)
        {
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
                AlphaProject.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                Thread.Sleep(1000);
                n++;
            }
            AlphaProject.status = (byte)TaskState.READY;
        }

        private Vector3 MovePositions(Vector3[] Path, bool UseMount)
        {
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            Vector3 position = AlphaProject.GameData.KeyOperates.GetUserPosition(SizeFactor);
            for (int i = 0; i < Path.Length; i++)
            {
                if (closed)
                {
                    PluginLog.Log($"中途结束");
                    return AlphaProject.GameData.KeyOperates.GetUserPosition(SizeFactor);
                }
                position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, Path[i], territoryType, UseMount, false);
            }
            return position;
        }

    }
}