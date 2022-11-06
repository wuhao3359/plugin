using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using WoAutoCollectionPlugin.SeFunctions;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class FishBot
    {
        private EventFramework EventFramework { get; init; }

        private FishingState LastState = FishingState.None;
        private FishingState FishingState = FishingState.None;
        Stopwatch yfishsw = new();
        private bool canMove = false;
        private bool readyMove = false;
        private bool closed = false;

        public FishBot()
        {
        }

        public void Init()
        {
            canMove = false;
            readyMove = false;
            closed = false;
        }

        public void MoveInit()
        {
            canMove = false;
            readyMove = false;
        }

        // 进入空岛
        public bool RunIntoYunGuanScript()
        {
            if (DalamudApi.ClientState.TerritoryType - Position.TianQiongJieTerritoryType != 0)
            {
                // 传送到伊修加德
                Teleporter.Teleport(70);
                // 转移到天穹街
                // TODO
            }
            if (DalamudApi.ClientState.TerritoryType - Position.TianQiongJieTerritoryType == 0)
            {
                // 移动到指定NPC 路径点
                Vector3[] ToArea = Position.YunGuanNPC;
                ushort SizeFactor = WoAutoCollectionPlugin.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
                //Vector3 position = KeyOperates.GetUserPosition(SizeFactor);
                //double d = Maths.Distance(position, ToArea[1]);
                //if (Maths.Distance(position, ToArea[1]) > 5)
                //{
                //    MovePositions(ToArea, false);
                //}
                //else {
                //    PluginLog.Log($"距离: {d} 不需要移动");
                //}
                MovePositions(ToArea, false);
                // 进入空岛
                if (!CommonUi.AddonSelectStringIsOpen() && !CommonUi.AddonSelectYesnoIsOpen()) {
                    WoAutoCollectionPlugin.GameData.CommonBot.SetTarget("奥瓦埃尔");
                    Thread.Sleep(800);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    CommonUi.SelectString1Button();
                }

                Thread.Sleep(1500);
                int n = 0;
                while (CommonUi.AddonSelectYesnoIsOpen() && n < 5)
                {
                    Thread.Sleep(500);
                    CommonUi.SelectYesButton();
                    n++;
                }

                Thread.Sleep(1500);
                n = 0;
                while (CommonUi.AddonContentsFinderConfirmIsOpen() && n < 5)
                {
                    Thread.Sleep(500);
                    CommonUi.ContentsFinderConfirmButton();
                    n++;
                }

                Thread.Sleep(10000);
            }
            return true;
        }

        public void YFishScript(string args) {
            closed = false;
            int n = 0;
            DalamudApi.Framework.Update += OnYFishUpdate;
            while (!closed && n < 8)
            {
                try
                {
                    if (WoAutoCollectionPlugin.GameData.TerritoryType.TryGetValue(DalamudApi.ClientState.TerritoryType, out var territoryType))
                    {
                        PluginLog.Log($"当前位置: {DalamudApi.ClientState.TerritoryType} {territoryType.PlaceName.Value.Name}");
                    }
                    if (DalamudApi.ClientState.TerritoryType - Position.TianQiongJieTerritoryType == 0)
                    {
                        RunIntoYunGuanScript();
                    }

                    if (DalamudApi.ClientState.TerritoryType - Position.YunGuanTerritoryType == 0)
                    {
                        RunYFishScript(args, n & 1);
                    }
                    else
                    {
                        PluginLog.Log($"当前位置不在空岛, {DalamudApi.ClientState.TerritoryType} ,skip...");
                        Thread.Sleep(2000);
                    }
                }
                catch (Exception e)
                {
                    PluginLog.Error($"error!!!\n{e}");
                }

                PluginLog.Log($"准备开始下一轮... {n}");
                n++;
                Thread.Sleep(3000);
            }
            DalamudApi.Framework.Update -= OnYFishUpdate;
        }

        // 在空岛中 自动前往指定地点钓鱼
        public bool RunYFishScript(string args, int N)
        {
            string[] str = args.Split(' ');
            int area = int.Parse(str[0]) + N * 10;
            int repair = 0;
            if (str.Length >= 2) {
                repair = int.Parse(str[1]);
            }

            yfishsw = new();
            MoveInit();
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            ushort SizeFactor = WoAutoCollectionPlugin.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);

            // 划分区域
            (Vector3[] ToArea, Vector3[] YFishArea) = GetAreaPoint(area);

            Vector3 position = WoAutoCollectionPlugin.GameData.KeyOperates.GetUserPosition(SizeFactor);
            PluginLog.Log($"开始 {position.X} {position.Y} {position.Z}");

            // 修理
            if (RepairUi.CanRepair())
            {
                PluginLog.Log($"修理装备...");
                position = WoAutoCollectionPlugin.GameData.KeyOperates.MoveToPoint(position, Position.YunGuanRepairNPC, territoryType, false, false);
                if (repair > 0)
                {
                    WoAutoCollectionPlugin.GameData.CommonBot.Repair();
                }
                else
                {
                    WoAutoCollectionPlugin.GameData.CommonBot.NpcRepair();
                }
            }
            else {
                PluginLog.Log($"不需要修理装备...");
            }

            int weather = 0;
            int pos = 0;
            int work = 0;
            MovePositions(ToArea, true);
            for (int i = 0; i <= 20; i++) {
                // 到达中心点
                //WoAutoCollectionPlugin.GameData.KeyOperates.MoveToPoint(position, Position.Center, territoryType, true, false);

                // 根据天气去对应钓场
                //(pos, Vector3[] posVector) = GetPosByWeather();
                //position = MovePositions(posVector, true);

                // 找寻无人点
                //(work, Vector3[] workVector) = GetWorkPos(pos);
                //if (work > 0)
                //{
                //    MovePositions(workVector, true);
                //}
                //else
                //{
                //    continue;
                //}

                if (DalamudApi.Condition[ConditionFlag.Mounted])
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.q_key);
                    Thread.Sleep(1000);
                }
                if (DalamudApi.Condition[ConditionFlag.Mounted])
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.q_key);
                    Thread.Sleep(500);
                }

                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.w_key, 200);
                // 切换鱼饵 TODO
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n2_key);
                Stopwatch sw = new();
                while (sw.ElapsedMilliseconds / 1000 / 60 < 40)
                {
                    Thread.Sleep(5000);
                    if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                    {
                        PluginLog.Log($"中途结束");
                        return false;
                    }
                    if (!(DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing]))
                    {
                        break;
                    }

                    // 获取当前天气
                }
                //position = MovePositions(workVector, true);
            }

            PluginLog.Log($"任务结束");
            return true;
        }

        private Vector3 MovePositions(Vector3[] ToArea, bool UseMount)
        {
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            ushort SizeFactor = WoAutoCollectionPlugin.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            Vector3 position = WoAutoCollectionPlugin.GameData.KeyOperates.GetUserPosition(SizeFactor);
            for (int i = 0; i < ToArea.Length; i++)
            {
                if (closed)
                {
                    PluginLog.Log($"中途结束");
                    return WoAutoCollectionPlugin.GameData.KeyOperates.GetUserPosition(SizeFactor);
                }
                position = WoAutoCollectionPlugin.GameData.KeyOperates.MoveToPoint(position, ToArea[i], territoryType, UseMount, false);
                PluginLog.Log($"到达点{i} {position.X} {position.Y} {position.Z}");
                Thread.Sleep(1000);
            }
            return position;
        }

        public void StopYFishScript()
        {
            closed = true;
            WoAutoCollectionPlugin.GameData.KeyOperates.ForceStop();
        }

        public void OnYFishUpdate(Framework _)
        {
            FishingState = EventFramework.FishingState;
            if (LastState == FishingState)
                return;
            LastState = FishingState;
            switch (FishingState)
            {
                case FishingState.PoleOut:
                    canMove = false;
                    yfishsw.Restart();
                    break;
                case FishingState.Bite:
                    OnYBite();
                    break;
                case FishingState.Reeling:
                    break;
                case FishingState.PoleReady:
                    YFishScript();
                    break;
                case FishingState.Waiting:
                    break;
                case FishingState.Quit:
                    canMove = true;
                    break;
            }
        }

        private void OnYBite()
        {
            Task task = new(() =>
            {
                PluginLog.Log($"yfish time: {yfishsw.ElapsedMilliseconds / 1000}");
                if (yfishsw.ElapsedMilliseconds / 1000 > 13)
                {
                    PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                    uint maxGp = player.MaxGp;
                    if (maxGp >= 700) {
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n7_key);
                    }
                }
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n1_key);
            });
            task.Start();
        }

        private void YFishScript()
        {
            Task task = new(() =>
            {
                Thread.Sleep(2000);
                PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                if (player != null)
                {
                    uint gp = player.CurrentGp;
                    uint maxGp = player.MaxGp;
                    byte stackCount = 0;
                    IEnumerator<Dalamud.Game.ClientState.Statuses.Status> statusList = player.StatusList.GetEnumerator();
                    while (statusList.MoveNext())
                    {
                        // 2778-捕鱼人之识
                        Dalamud.Game.ClientState.Statuses.Status status = statusList.Current;
                        uint statusId = status.StatusId;
                        byte StackCount = status.StackCount;
                        if (statusId == 2778)
                        {
                            stackCount = StackCount;
                        }
                    }
                    if (gp < maxGp * 0.6)
                    {
                        if (stackCount >= 3)
                        {
                            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n0_key);
                            gp += 150;
                            Thread.Sleep(1000);
                        }
                    }
                    if (gp < maxGp * 0.5)
                    {
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.minus_key);
                        Thread.Sleep(1500);
                    }
                    if (readyMove)
                    {
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F1_key);
                    }
                    else
                    {
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F2_key);
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n2_key);
                    }
                }
            });
            task.Start();
        }

        private (Vector3[], Vector3[]) GetAreaPoint(int area) {
            Vector3[] ToArea = Array.Empty<Vector3>();
            Vector3[] YFishArea = Array.Empty<Vector3>();

            if (area == 1)
            {
                ToArea = Position.ToArea1;
                YFishArea = Position.YFishArea1;
            }
            else if (area == 11)
            {
                ToArea = Position.ToArea11;
                YFishArea = Position.YFishArea11;
            }
            else if (area == 2)
            {
                ToArea = Position.ToAreaB;
                YFishArea = Position.YFishAreaB;
            }
            else if (area == 12)
            {
                ToArea = Position.ToAreaB;
                YFishArea = Position.YFishAreaB;
            }
            else if (area == 3)
            {
                ToArea = Position.ToAreaC;
                YFishArea = Position.YFishAreaC;
            }
            else if (area == 13)
            {
                ToArea = Position.ToAreaC;
                YFishArea = Position.YFishAreaC;
            }
            else if (area == 100)
            {
                ToArea = Position.ToArea100;
                YFishArea = Position.YFishArea100;
            }
            else if (area == 110)
            {
                ToArea = Position.ToArea100;
                YFishArea = Position.YFishArea100;
            }
            return (ToArea, YFishArea);
        }

        // TODO
        private (int, Vector3[]) GetPosByWeather() {
            Vector3[] v = { };
            return (0, v);
        }

        private (int, Vector3[]) GetWorkPos(int pos) {
            List<Vector3[]> FishList = Position.FishAList;
            if (pos == 1) 
            {
                FishList = Position.FishBList;
            } else if (pos == 2) 
            {
                FishList = Position.FishCList;
            }
            else if (pos == 3)
            {
                FishList = Position.FishDList;
            }

            return Util.GetNulHunmanPos(FishList);
        }
    }
}