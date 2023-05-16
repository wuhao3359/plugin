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

namespace AlphaProject.Bot
{
    public class FishBot
    {
        private FishingState LastState = FishingState.None;
        private FishingState FishingState = FishingState.None;
        Stopwatch yfishsw = new();
        Stopwatch sw = new();
        private bool canMove = false;
        private bool readyMove = false;
        private bool closed = false;

        private int CurrentGround = 0;
        private int CurrentPoint = 0;
        public FishBot()
        {
        }

        public void Init()
        {
            canMove = false;
            readyMove = false;
            closed = false;
            CurrentGround = 0;
            CurrentPoint = 0;
        }

        public void MoveInit()
        {
            canMove = false;
            readyMove = false;
        }

        // 进入空岛
        public bool RunIntoYunGuanScript()
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
                MovePositions(ToArea, false, DalamudApi.ClientState.TerritoryType);
                // 进入空岛
                if (!CommonUi.AddonSelectStringIsOpen() && !CommonUi.AddonSelectYesnoIsOpen()) {
                    AlphaProject.GameData.CommonBot.SetTarget("奥瓦埃尔");
                    Thread.Sleep(800);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
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
        public bool RunYFishScript(string args)
        {
            string[] str = args.Split(' ');
            int area = int.Parse(str[0]);

            yfishsw = new();
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            Vector3 position = AlphaProject.GameData.KeyOperates.GetUserPosition(SizeFactor);

            // 修理
            if (CommonUi.CanRepair())
            {
                PluginLog.Log($"修理装备...");
                position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, Positions.YunGuanRepairNPC, territoryType, false, false);
                AlphaProject.GameData.CommonBot.NpcRepair("修理工");
            }
            else {
                PluginLog.Log($"不需要修理装备...");
            }

            if (area >= 100)
            {
                MovePositions(Positions.Leveling, true, territoryType);
                for (int i = 0; i <= 20; i++)
                {
                    int tt = 0;
                    while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 12)
                    {
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.q_key);
                        Thread.Sleep(1000);
                        tt++;

                        if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                        {
                            PluginLog.Log($"task stopping");
                            return true;
                        }
                    }

                    position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, Positions.LevelingPoints[CurrentPoint], territoryType, false, false);
                    CurrentPoint++;
                    if (CurrentPoint > 1)
                    {
                        CurrentPoint = 0;
                    }

                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.w_key, 800);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.n2_key);
                    sw.Restart();
                    MoveInit();
                    while (sw.ElapsedMilliseconds / 1000 / 60 < 45)
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
                    }
                    readyMove = true;
                    while (!canMove)
                    {
                        Thread.Sleep(5000);
                        PluginLog.Log($"等待停止...");
                        if (!(DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing]))
                        {
                            canMove = true;
                        }
                    }
                }
            }
            else {
                // 划分区域
                (Vector3[] ToGroundA, Vector3[] ToGroundB, Vector3[] ToGroundC, Vector3[] GroundA, Vector3[] GroundB, Vector3[] GroundC) = GetAreaPoint(area);
                MovePositions(Positions.ToStart, true, territoryType);
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
                    if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                    {
                        PluginLog.Log($"task stopping");
                        return false;
                    }
                    MovePositions(ToGround, true, territoryType);
                    int tt = 0;
                    while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 15)
                    {
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.q_key);
                        Thread.Sleep(1000);
                        tt++;

                        if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                        {
                            PluginLog.Log($"task stopping");
                            return false;
                        }
                    }
                    if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                    {
                        PluginLog.Log($"task stopping");
                        return false;
                    }
                    position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, Ground[CurrentPoint], territoryType, false, false);
                    CurrentPoint++;
                    if (CurrentPoint > 1)
                    {
                        CurrentPoint = 0;
                    }

                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.w_key, 800);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.n2_key);
                    sw.Restart();
                    MoveInit();
                    while (sw.ElapsedMilliseconds / 1000 / 60 < 45)
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
                    }
                    readyMove = true;
                    while (!canMove) {
                        Thread.Sleep(5000);
                        PluginLog.Log($"等待停止...");
                        if (!(DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing]))
                        {
                            canMove = true;
                        }
                    }
                    Thread.Sleep(1000);
                }
            }

            PluginLog.Log($"任务结束");
            return true;
        }

        private Vector3 MovePositions(Vector3[] ToArea, bool UseMount, ushort territoryType)
        {
            ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            Vector3 position = AlphaProject.GameData.KeyOperates.GetUserPosition(SizeFactor);
            for (int i = 0; i < ToArea.Length; i++)
            {
                if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                {
                    PluginLog.Log($"中途结束");
                    return AlphaProject.GameData.KeyOperates.GetUserPosition(SizeFactor);
                }
                position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, ToArea[i], territoryType, UseMount, false);
                //PluginLog.Log($"到达点{i} {position.X} {position.Y} {position.Z}");
                Thread.Sleep(500);
            }
            return position;
        }

        public void StopScript()
        {
            closed = true;
            AlphaProject.GameData.KeyOperates.ForceStop();
        }

        public void OnYFishUpdate(Framework _)
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
            else {
                PluginLog.Log($"EventFramework null");
                DalamudApi.Framework.Update -= OnYFishUpdate;
            }
            
        }

        private void OnYBite()
        {
            Task task = new(() =>
            {
                PluginLog.Log($"yfish time: {yfishsw.ElapsedMilliseconds / 1000}");
                PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                uint maxGp = player.MaxGp;
                if (CurrentGround == 0) {
                    // C 狂风云海 灵飘尘  <14 双提 14-24 三提 >24单提
                    if (yfishsw.ElapsedMilliseconds / 1000 > 10 && yfishsw.ElapsedMilliseconds / 1000 < 14)
                    {
                        if (maxGp >= 400)
                        {
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.n6_key);
                        }
                    }
                    else if (yfishsw.ElapsedMilliseconds / 1000 >= 14 && yfishsw.ElapsedMilliseconds / 1000 < 24)
                    {
                        if (maxGp >= 700)
                        {
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.n7_key);
                        }
                    }
                } else if (CurrentGround == 2)
                {
                    // B 旋风云海 灵罡风  <12 双提 >12 单提
                    if (yfishsw.ElapsedMilliseconds / 1000 > 10 && yfishsw.ElapsedMilliseconds / 1000 < 12)
                    {
                        if (maxGp >= 400)
                        {
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.n6_key);
                        }
                    }
                }
                else if (CurrentGround == 1)
                {
                    // A 摇风云海 灵飞电  <16 双提 >16单提
                    if (yfishsw.ElapsedMilliseconds / 1000 > 10 && yfishsw.ElapsedMilliseconds / 1000 < 16)
                    {
                        if (maxGp >= 400)
                        {
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.n6_key);
                        }
                    }
                }
                AlphaProject.GameData.KeyOperates.KeyMethod(Keys.n1_key);
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
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.n0_key);
                            gp += 150;
                            Thread.Sleep(1000);
                        }
                    }
                    if (gp < maxGp * 0.5)
                    {
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.plus_key);
                        Thread.Sleep(1500);
                    }
                    if (readyMove)
                    {
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.F1_key);
                    }
                    else
                    {
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.n8_key);
                        if (gp > 100) {
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.F2_key);
                        }
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.n2_key);
                    }
                }
            });
            task.Start();
        }

        private (Vector3[], Vector3[], Vector3[], Vector3[], Vector3[], Vector3[]) GetAreaPoint(int area) {
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