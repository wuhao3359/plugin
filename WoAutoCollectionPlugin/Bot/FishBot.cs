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
        private GameData GameData { get; init; }
        private KeyOperates KeyOperates { get; init; }
        private EventFramework EventFramework { get; init; }

        private CommonBot? CommonBot;

        private FishingState LastState = FishingState.None;
        private FishingState FishingState = FishingState.None;
        Stopwatch yfishsw = new();
        private bool canMove = false;
        private bool readyMove = false;
        private bool closed = false;

        public FishBot(GameData GameData)
        {
            this.GameData = GameData;
            KeyOperates = new KeyOperates(GameData);
            EventFramework = new EventFramework(DalamudApi.SigScanner);
            CommonBot = new CommonBot(KeyOperates);
        }

        public void Init()
        {
            canMove = false;
            readyMove = false;
            closed = false;
        }

        // 进入空岛
        public bool RunIntoYunGuanScript()
        {
            //PluginLog.Log($"{GameData.Aetherytes.Count}");
            //foreach (var aetheryte in GameData.Aetherytes)
            //{
            //    uint key = aetheryte.Key;
            //    Aetheryte value = aetheryte.Value;
            //    PluginLog.Log($"{key} {value.Name} {value.Id}{value}");
            //}
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
                ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
                Vector3 position = KeyOperates.GetUserPosition(SizeFactor);
                double d = Maths.Distance(position, ToArea[1]);
                if (Maths.Distance(position, ToArea[1]) > 5)
                {
                    MovePositions(ToArea, false);
                }
                else {
                    PluginLog.Log($"距离: {d} 不需要移动");
                }

                // 进入空岛
                if (!CommonUi.AddonSelectStringIsOpen() && !CommonUi.AddonSelectYesnoIsOpen()) {
                    KeyOperates.KeyMethod(Keys.num1_key);
                    KeyOperates.KeyMethod(Keys.num0_key);
                    Thread.Sleep(500);
                    KeyOperates.KeyMethod(Keys.num0_key);
                }
                int n = 0;
                while (!CommonUi.AddonSelectStringIsOpen() && n < 5) {
                    Thread.Sleep(500);
                    n++;
                }
                if (CommonUi.AddonSelectStringIsOpen()) {
                    KeyOperates.KeyMethod(Keys.num0_key);
                    KeyOperates.KeyMethod(Keys.num0_key);
                }
                
                n = 0;
                while (!CommonUi.AddonSelectYesnoIsOpen() && n < 5)
                {
                    Thread.Sleep(500);
                    n++;
                }
                Thread.Sleep(1500);
                if (CommonUi.AddonSelectYesnoIsOpen())
                {
                    CommonUi.SelectYesButton();
                    KeyOperates.KeyMethod(Keys.num0_key);
                    Thread.Sleep(500);
                }

                if (CommonUi.AddonSelectYesnoIsOpen()) {
                    KeyOperates.KeyMethod(Keys.num0_key);
                    KeyOperates.KeyMethod(Keys.num0_key);
                }

                n = 0;
                while (!CommonUi.AddonContentsFinderConfirmOpen() && n < 5)
                {
                    Thread.Sleep(500);
                    n++;
                }
                Thread.Sleep(1500);
                if (CommonUi.AddonContentsFinderConfirmOpen())
                {
                    CommonUi.ContentsFinderConfirmButton();
                    KeyOperates.KeyMethod(Keys.num0_key);
                    Thread.Sleep(500);
                }

                if (CommonUi.AddonContentsFinderConfirmOpen()) {
                    KeyOperates.KeyMethod(Keys.num0_key);
                    KeyOperates.KeyMethod(Keys.num0_key);
                }

                Thread.Sleep(10000);
            }
            return true;
        }

        // 在空岛中 自动前往指定地点钓鱼
        public bool RunYFishScript(string args)
        {
            string[] str = args.Split(' ');
            int area = int.Parse(str[0]);
            int repair = 0;
            if (str.Length >= 3) {
                repair = int.Parse(str[2]);
            }

            yfishsw = new();
            Init();
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);

            // 划分区域
            Vector3[] ToArea = Array.Empty<Vector3>();
            Vector3[] YFishArea = Array.Empty<Vector3>();
            if (area == 1)
            {
                ToArea = Position.ToAreaA;
                YFishArea = Position.YFishAreaA;
            }
            else if (area == 2)
            {
                ToArea = Position.ToAreaB;
                YFishArea = Position.YFishAreaB;
            }

            Vector3 position = KeyOperates.GetUserPosition(SizeFactor);
            PluginLog.Log($"开始 {position.X} {position.Y} {position.Z}");

            // 修理
            if (RepairUi.CanRepair())
            {
                PluginLog.Log($"修理装备...");
                position = KeyOperates.MoveToPoint(position, Position.YunGuanRepairNPC, territoryType, false);
                if (repair > 0)
                {
                    CommonBot.Repair();
                }
                else
                {
                    CommonBot.NpcRepair();
                }
            }
            else {
                PluginLog.Log($"不需要修理装备...");
            }

            KeyOperates.KeyMethod(Keys.q_key);
            Thread.Sleep(3000);
            // 通过路径到达固定区域位置
            position = MovePositions(ToArea, true);

            KeyOperates.KeyMethod(Keys.q_key);
            Thread.Sleep(1000);
            KeyOperates.KeyMethod(Keys.q_key);

            // 在固定区域到达作业点 作业循环 40min切换  0->2->1->2->0....
            int currentPoint = 0;
            Stopwatch sw = new();

            for (int i = 0; i <= 10; i++)
            {
                sw.Reset();
                if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                {
                    PluginLog.Log($"中途结束");
                    return false;
                }
                sw.Start();

                position = KeyOperates.MoveToPoint(position, YFishArea[2], territoryType, false);
                position = KeyOperates.MoveToPoint(position, YFishArea[currentPoint], territoryType, false);
                if (currentPoint > 0)
                {
                    currentPoint = 0;
                }
                else
                {
                    currentPoint = 1;
                }
                // 开始作业
                readyMove = false;
                KeyOperates.KeyMethod(Keys.w_key, 200);
                KeyOperates.KeyMethod(Keys.n2_key);

                while (sw.ElapsedMilliseconds / 1000 / 60 < 40)
                {
                    Thread.Sleep(1000);
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
                    Thread.Sleep(1000);
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
            }
            PluginLog.Log($"任务结束");
            return true;
        }

        private Vector3 MovePositions(Vector3[] ToArea, bool UseMount)
        {
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            Vector3 position = KeyOperates.GetUserPosition(SizeFactor);
            for (int i = 0; i < ToArea.Length; i++)
            {
                if (closed)
                {
                    PluginLog.Log($"中途结束");
                    return KeyOperates.GetUserPosition(SizeFactor);
                }
                position = KeyOperates.MoveToPoint(position, ToArea[i], territoryType, UseMount);
                PluginLog.Log($"到达点{i} {position.X} {position.Y} {position.Z}");
                Thread.Sleep(1000);
            }
            return position;
        }

        public void StopYFishScript()
        {
            closed = true;
            KeyOperates.ForceStop();
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
                    KeyOperates.KeyMethod(Keys.n8_key);
                }
                KeyOperates.KeyMethod(Keys.n1_key);
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
                            KeyOperates.KeyMethod(Keys.n0_key);
                            gp += 150;
                            Thread.Sleep(1500);
                        }
                    }
                    if (gp < maxGp * 0.5)
                    {
                        KeyOperates.KeyMethod(Keys.minus_key);
                        Thread.Sleep(1500);
                    }
                    if (readyMove)
                    {
                        KeyOperates.KeyMethod(Keys.F1_key);
                    }
                    else
                    {
                        KeyOperates.KeyMethod(Keys.n2_key);
                    }
                }
            });
            task.Start();
        }
    }
}