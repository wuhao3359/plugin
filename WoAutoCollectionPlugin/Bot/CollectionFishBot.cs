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
    public class CollectionFishBot
    {
        private GameData GameData { get; init; }
        private KeyOperates KeyOperates { get; init; }
        private EventFramework EventFramework { get; init; }

        private static SeTugType TugType { get; set; } = null!;

        private FishRecord Record;

        private CommonBot? CommonBot;

        private FishingState LastState = FishingState.None;
        private FishingState FishingState = FishingState.None;
        Stopwatch fishsw = new();
        private bool canMove = false;
        private bool readyMove = false;
        private bool closed = false;

        private bool LastFish = false;

        public CollectionFishBot(GameData GameData)
        {
            this.GameData = GameData;
            KeyOperates = new KeyOperates(GameData);
            EventFramework = new EventFramework(DalamudApi.SigScanner);
            CommonBot = new CommonBot(GameData);

            TugType = new SeTugType(DalamudApi.SigScanner);
            Record = new FishRecord();
        }

        public void Init()
        {
            canMove = false;
            readyMove = false;
            closed = false;
        }

        // 前往指定钓鱼地点 [√]
        // 钓鱼   [√]
        // 清背包 (换工票/精选) [×] TODO
        public bool RunCollectionFishScript(string args)
        {
            string[] str = args.Split(' ');
            int area = int.Parse(str[0]);
            int repair = -1;
            if (str.Length >= 1) {
                repair = int.Parse(str[1]);
            }

            fishsw = new();
            Init();
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);

            // 划分区域
            Vector3[] ToArea = Array.Empty<Vector3>();
            Vector3[] FishArea = Array.Empty<Vector3>();
            if (area == 1)
            {
                ToArea = Position.ToPurpleFishArea;
                FishArea = Position.PurpleFishArea;
            }

            Vector3 position = KeyOperates.GetUserPosition(SizeFactor);
            PluginLog.Log($"开始 {position.X} {position.Y} {position.Z}");
            KeyOperates.KeyMethod(Keys.q_key);
            Thread.Sleep(3000);
            // 通过路径到达固定区域位置
            position = MovePositions(ToArea, true);

            KeyOperates.KeyMethod(Keys.q_key);
            Thread.Sleep(3000);
            KeyOperates.KeyMethod(Keys.q_key);

            // 在固定区域到达作业点 作业循环 40min切换  0->2->1->2->0....
            int currentPoint = 0;
            Stopwatch sw = new();

            for (int i = 0; i <= 20; i++)
            {
                sw.Reset();
                if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                {
                    PluginLog.Log($"中途结束");
                    return false;
                }
                sw.Start();

                position = KeyOperates.TestMoveToPoint(position, FishArea[2], territoryType, false);
                position = KeyOperates.TestMoveToPoint(position, FishArea[currentPoint], territoryType, false);
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

                if (repair >= 0 && RepairUi.NeedsRepair())
                {
                    CommonBot.Repair();
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
                position = KeyOperates.TestMoveToPoint(position, ToArea[i], territoryType, UseMount);
                PluginLog.Log($"到达点{i} {position.X} {position.Y} {position.Z}");
                Thread.Sleep(1000);
            }
            return position;
        }

        public void StopCollectionFishScript()
        {
            closed = true;
            KeyOperates.ForceStop();
        }

        public void OnCollectionFishUpdate(Framework _)
        {
            FishingState = EventFramework.FishingState;
            if (LastState == FishingState)
                return;
            LastState = FishingState;
            switch (FishingState)
            {
                case FishingState.PoleOut:
                    canMove = false;
                    fishsw.Restart();
                    break;
                case FishingState.Bite:
                    OnCollectionFishBite();
                    break;
                case FishingState.Reeling:
                    break;
                case FishingState.PoleReady:
                    CollectionFishScript();
                    break;
                case FishingState.Waiting:
                    MakeSure();
                    break;
                case FishingState.Quit:
                    canMove = true;
                    break;
            }
        }

        private void OnCollectionFishBite()
        {
            Record.SetTugHook(TugType.Bite, Record.Hook);
            PluginLog.Log("Fish bit with {BiteType}", Record.Tug);
            Task task = new(() =>
            {
                PluginLog.Log($"Fish bit with {Record.Tug} fish time: {fishsw.ElapsedMilliseconds / 1000}");
                if (fishsw.ElapsedMilliseconds / 1000 < 10)
                {
                    KeyOperates.KeyMethod(Keys.n1_key);
                }
                else
                {
                    switch (Record.Tug.ToString())
                    {
                        case "Week":
                            KeyOperates.KeyMethod(Keys.n3_key);
                            break;
                        case "Strong":
                            KeyOperates.KeyMethod(Keys.n4_key);
                            break;
                        default:
                            break;
                    }
                    KeyOperates.KeyMethod(Keys.n1_key);
                }
            });
            task.Start();
        }

        private void CollectionFishScript()
        {
            Task task = new(() =>
            {
                Thread.Sleep(2000);
                PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                if (!readyMove)
                {
                    bool existStatus = false;
                    byte stackCount = 0;
                    IEnumerator<Dalamud.Game.ClientState.Statuses.Status> statusList = player.StatusList.GetEnumerator();
                    while (statusList.MoveNext())
                    {
                        // 2778-捕鱼人之识 850-耐心
                        Dalamud.Game.ClientState.Statuses.Status status = statusList.Current;
                        uint statusId = status.StatusId;
                        byte StackCount = status.StackCount;
                        if (statusId == 850)
                        {
                            existStatus = true;
                        }
                        if (statusId == 2778)
                        {
                            stackCount = StackCount;
                        }
                    }

                    uint gp = player.CurrentGp;
                    uint maxGp = player.MaxGp;
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
                    if (!existStatus)
                    {
                        Thread.Sleep(6000);
                        if (gp > 560)
                        {
                            KeyOperates.KeyMethod(Keys.n5_key);
                            existStatus = true;
                            gp -= 560;
                        }
                    }
                    if (LastFish && gp > 350)
                    {
                        KeyOperates.KeyMethod(Keys.n6_key, 0, true);
                        Thread.Sleep(1500);
                    }
                    LastFish = false;

                    KeyOperates.KeyMethod(Keys.n2_key);
                }
                else {
                    KeyOperates.KeyMethod(Keys.F1_key);
                }
            });
            task.Start();
        }

        protected void MakeSure()
        {
            LastFish = true;
            Task task = new(() =>
            {
                KeyOperates.KeyMethod(Keys.num0_key);
                KeyOperates.KeyMethod(Keys.num0_key);
            });
            task.Start();
        }
    }
}