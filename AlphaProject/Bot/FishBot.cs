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
using AlphaProject.SeFunctions;
using AlphaProject.Ui;
using AlphaProject.Utility;
using AlphaProject.Helper;

namespace AlphaProject.Bot
{
    public static class FishBot
    {
        private static FishingState LastState = FishingState.None;
        private static FishingState FishingState = FishingState.None;
        static Stopwatch Yfishsw = new();
        static Stopwatch Sw = new();
        private static bool CanMove = false;
        private static bool ReadyMove = false;
        private static bool Closed = false;

        private static int CurrentGround = 0;
        private static int CurrentPoint = 0;

        public static void Init()
        {
            CanMove = false;
            ReadyMove = false;
            Closed = false;
            CurrentGround = 0;
            CurrentPoint = 0;
        }

        public static void MoveInit()
        {
            CanMove = false;
            ReadyMove = false;
        }

        // 进入空岛
        public static bool RunIntoYunGuanScript()
        {
            if (DalamudApi.ClientState.TerritoryType - Positions.TianQiongJieTerritoryType != 0)
            {
                // 传送到伊修加德
                Teleporter.Teleport(70);
                // 转移到天穹街
                // TODO
            }
            if (DalamudApi.ClientState.TerritoryType - Positions.TianQiongJieTerritoryType == 0)
            {
                // 移动到指定NPC 路径点
                Vector3[] ToArea = Positions.YunGuanNPC;
                ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
                KeyOperates.MovePositions(ToArea, false, DalamudApi.ClientState.TerritoryType);
                // 进入空岛
                if (!CommonUi.AddonSelectStringIsOpen() && !CommonUi.AddonSelectYesnoIsOpen()) {
                    CommonHelper.SetTarget("奥瓦埃尔");
                    Thread.Sleep(800);
                    KeyOperates.KeyMethod(Keys.num0_key);
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

        public static void YFishScript(string args) {
            Closed = false;
            int n = 0;
            DalamudApi.Framework.Update += OnYFishUpdate;
            while (!Closed && n < 8)
            {
                try
                {
                    if (AlphaProject.GameData.TerritoryType.TryGetValue(DalamudApi.ClientState.TerritoryType, out var territoryType))
                    {
                        PluginLog.Log($"当前位置: {DalamudApi.ClientState.TerritoryType} {territoryType.PlaceName.Value.Name}");
                    }
                    if (DalamudApi.ClientState.TerritoryType - Positions.TianQiongJieTerritoryType == 0)
                    {
                        RunIntoYunGuanScript();
                    }

                    if (DalamudApi.ClientState.TerritoryType - Positions.YunGuanTerritoryType == 0)
                    {
                        RunYFishScript(args);
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
        public static bool RunYFishScript(string args)
        {
            string[] str = args.Split(' ');
            int area = int.Parse(str[0]);

            Yfishsw = new();
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            Vector3 position = KeyOperates.GetUserPosition(SizeFactor);

            // 修理
            if (CommonUi.CanRepair())
            {
                PluginLog.Log($"修理装备...");
                position = KeyOperates.MoveToPoint(position, Positions.YunGuanRepairNPC, territoryType, false, false);
                CommonBot.NpcRepair("修理工");
            }
            else {
                PluginLog.Log($"不需要修理装备...");
            }

            if (area >= 100)
            {
                KeyOperates.MovePositions(Positions.Leveling, true, territoryType);
                for (int i = 0; i <= 20; i++)
                {
                    int tt = 0;
                    while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 12)
                    {
                        KeyOperates.KeyMethod(Keys.q_key);
                        Thread.Sleep(1000);
                        tt++;

                        if (Closed || territoryType != DalamudApi.ClientState.TerritoryType)
                        {
                            PluginLog.Log($"task stopping");
                            return true;
                        }
                    }

                    position = KeyOperates.MoveToPoint(position, Positions.LevelingPoints[CurrentPoint], territoryType, false, false);
                    CurrentPoint++;
                    if (CurrentPoint > 1)
                    {
                        CurrentPoint = 0;
                    }

                    KeyOperates.KeyMethod(Keys.w_key, 800);
                    KeyOperates.KeyMethod(Keys.n2_key);
                    Sw.Restart();
                    MoveInit();
                    while (Sw.ElapsedMilliseconds / 1000 / 60 < 45)
                    {
                        Thread.Sleep(5000);
                        if (Closed || territoryType != DalamudApi.ClientState.TerritoryType)
                        {
                            PluginLog.Log($"中途结束");
                            return false;
                        }
                        if (!(DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing]))
                        {
                            break;
                        }
                    }
                    ReadyMove = true;
                    while (!CanMove)
                    {
                        Thread.Sleep(5000);
                        PluginLog.Log($"等待停止...");
                        if (!(DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing]))
                        {
                            CanMove = true;
                        }
                    }
                }
            }
            else {
                // 划分区域
                (Vector3[] ToGroundA, Vector3[] ToGroundB, Vector3[] ToGroundC, Vector3[] GroundA, Vector3[] GroundB, Vector3[] GroundC) = GetAreaPoint(area);
                KeyOperates.MovePositions(Positions.ToStart, true, territoryType);
                for (int i = 0; i <= 20; i++)
                {
                    Vector3[] ToGround = Array.Empty<Vector3>();
                    Vector3[] Ground = Array.Empty<Vector3>();
                    if (CurrentGround == 0)
                    {
                        PluginLog.Log($"A点...");
                        ToGround = ToGroundA;
                        Ground = GroundA;
                    }
                    else if (CurrentGround == 1)
                    {
                        PluginLog.Log($"B点...");
                        ToGround = ToGroundB;
                        Ground = GroundB;
                    }
                    else if (CurrentGround == 2)
                    {
                        PluginLog.Log($"C点...");
                        ToGround = ToGroundC;
                        Ground = GroundC;
                    }
                    
                    CurrentGround++;
                    if (CurrentGround == 3)
                    {
                        CurrentGround = 0;
                    }
                    Thread.Sleep(2000);
                    if (Closed || territoryType != DalamudApi.ClientState.TerritoryType)
                    {
                        PluginLog.Log($"task stopping");
                        return false;
                    }
                    KeyOperates.MovePositions(ToGround, true, territoryType);
                    int tt = 0;
                    while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 15)
                    {
                        KeyOperates.KeyMethod(Keys.q_key);
                        Thread.Sleep(1000);
                        tt++;

                        if (Closed || territoryType != DalamudApi.ClientState.TerritoryType)
                        {
                            PluginLog.Log($"task stopping");
                            return false;
                        }
                    }
                    if (Closed || territoryType != DalamudApi.ClientState.TerritoryType)
                    {
                        PluginLog.Log($"task stopping");
                        return false;
                    }
                    position = KeyOperates.MoveToPoint(position, Ground[CurrentPoint], territoryType, false, false);
                    CurrentPoint++;
                    if (CurrentPoint > 1)
                    {
                        CurrentPoint = 0;
                    }

                    KeyOperates.KeyMethod(Keys.w_key, 800);
                    KeyOperates.KeyMethod(Keys.n2_key);
                    Sw.Restart();
                    MoveInit();
                    while (Sw.ElapsedMilliseconds / 1000 / 60 < 45)
                    {
                        Thread.Sleep(5000);
                        if (Closed || territoryType != DalamudApi.ClientState.TerritoryType)
                        {
                            PluginLog.Log($"中途结束");
                            return false;
                        }
                        if (!(DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing]))
                        {
                            break;
                        }
                    }
                    ReadyMove = true;
                    while (!CanMove) {
                        Thread.Sleep(5000);
                        PluginLog.Log($"等待停止...");
                        if (!(DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing]))
                        {
                            CanMove = true;
                        }
                    }
                    Thread.Sleep(1000);
                }
            }

            PluginLog.Log($"任务结束");
            return true;
        }

        public static void StopScript()
        {
            Closed = true;
            KeyOperates.ForceStop();
        }

        public static void OnYFishUpdate(Framework _)
        {
            if (AlphaProject.GameData.EventFramework != null)
            {
                FishingState = AlphaProject.GameData.EventFramework.FishingState;
                if (LastState == FishingState)
                    return;
                LastState = FishingState;
                switch (FishingState)
                {
                    case FishingState.PoleOut:
                        CanMove = false;
                        Yfishsw.Restart();
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
                        CanMove = true;
                        break;
                }
            }
            else {
                PluginLog.Log($"EventFramework null");
                DalamudApi.Framework.Update -= OnYFishUpdate;
            }
            
        }

        private static void OnYBite()
        {
            Task task = new(() =>
            {
                PluginLog.Log($"yfish time: {Yfishsw.ElapsedMilliseconds / 1000}");
                PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                uint maxGp = player.MaxGp;
                if (CurrentGround == 0) {
                    // C 狂风云海 灵飘尘  <14 双提 14-24 三提 >24单提
                    if (Yfishsw.ElapsedMilliseconds / 1000 > 10 && Yfishsw.ElapsedMilliseconds / 1000 < 14)
                    {
                        if (maxGp >= 400)
                        {
                            KeyOperates.KeyMethod(Keys.n6_key);
                        }
                    }
                    else if (Yfishsw.ElapsedMilliseconds / 1000 >= 14 && Yfishsw.ElapsedMilliseconds / 1000 < 24)
                    {
                        if (maxGp >= 700)
                        {
                            KeyOperates.KeyMethod(Keys.n7_key);
                        }
                    }
                } else if (CurrentGround == 2)
                {
                    // B 旋风云海 灵罡风  <12 双提 >12 单提
                    if (Yfishsw.ElapsedMilliseconds / 1000 > 10 && Yfishsw.ElapsedMilliseconds / 1000 < 12)
                    {
                        if (maxGp >= 400)
                        {
                            KeyOperates.KeyMethod(Keys.n6_key);
                        }
                    }
                }
                else if (CurrentGround == 1)
                {
                    // A 摇风云海 灵飞电  <16 双提 >16单提
                    if (Yfishsw.ElapsedMilliseconds / 1000 > 10 && Yfishsw.ElapsedMilliseconds / 1000 < 16)
                    {
                        if (maxGp >= 400)
                        {
                            KeyOperates.KeyMethod(Keys.n6_key);
                        }
                    }
                }
                KeyOperates.KeyMethod(Keys.n1_key);
            });
            task.Start();
        }

        private static void YFishScript()
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
                            KeyOperates.KeyMethod(Keys.n0_key);
                            gp += 150;
                            Thread.Sleep(1000);
                        }
                    }
                    if (gp < maxGp * 0.5)
                    {
                        KeyOperates.KeyMethod(Keys.plus_key);
                        Thread.Sleep(1500);
                    }
                    if (ReadyMove)
                    {
                        KeyOperates.KeyMethod(Keys.F1_key);
                    }
                    else
                    {
                        KeyOperates.KeyMethod(Keys.n8_key);
                        if (gp > 100) {
                            KeyOperates.KeyMethod(Keys.F2_key);
                        }
                        KeyOperates.KeyMethod(Keys.n2_key);
                    }
                }
            });
            task.Start();
        }

        private static (Vector3[], Vector3[], Vector3[], Vector3[], Vector3[], Vector3[]) GetAreaPoint(int area) {
            Vector3[] ToGroundA = Array.Empty<Vector3>();
            Vector3[] ToGroundB = Array.Empty<Vector3>();
            Vector3[] ToGroundC = Array.Empty<Vector3>();
            Vector3[] GroundA = Array.Empty<Vector3>();
            Vector3[] GroundB = Array.Empty<Vector3>();
            Vector3[] GroundC = Array.Empty<Vector3>();

            if (area == 1)
            {
                ToGroundA = Positions.ToGroundA1;
                ToGroundB = Positions.ToGroundB1;
                ToGroundC = Positions.ToGroundC1;
                GroundA = Positions.GroundA1;
                GroundB = Positions.GroundB1;
                GroundC = Positions.GroundC1;
            }
            else if (area == 2)
            {
                ToGroundA = Positions.ToGroundA2;
                ToGroundB = Positions.ToGroundB2;
                ToGroundC = Positions.ToGroundC2;
                GroundA = Positions.GroundA2;
                GroundB = Positions.GroundB2;
                GroundC = Positions.GroundC2;
            }
            return (ToGroundA, ToGroundB, ToGroundC, GroundA, GroundB, GroundC);
        }
    }
}