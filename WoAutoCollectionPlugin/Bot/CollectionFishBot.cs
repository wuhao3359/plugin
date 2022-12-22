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
using WoAutoCollectionPlugin.Data;
using WoAutoCollectionPlugin.Managers;
using WoAutoCollectionPlugin.SeFunctions;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.UseAction;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class CollectionFishBot
    {
        private EventFramework EventFramework { get; init; }

        private static SeTugType TugType { get; set; } = null!;

        private FishRecord Record;

        private FishingState LastState = FishingState.None;
        private FishingState FishingState = FishingState.None;
        Stopwatch fishsw = new();
        private bool canMove = false;
        private bool readyMove = false;
        private bool closed = false;

        private bool LastFish = false;

        private int fishTime = 0;
        private uint fishTp = 0;
        private ushort fishTerritoryType = 0;

        public CollectionFishBot()
        {
            EventFramework = new EventFramework(DalamudApi.SigScanner);
            TugType = new SeTugType(DalamudApi.SigScanner);
            Record = new FishRecord();
        }

        public void Init()
        {
            canMove = false;
            readyMove = false;
        }

        public void CollectionFishScript(string args) {
            closed = false;
            int n = 0;
            DalamudApi.Framework.Update +=OnCollectionFishUpdate;
            string command = "collectionfish";
            WoAutoCollectionPlugin.GameData.param = Util.CommandParse(command, args);

            WoAutoCollectionPlugin.GameData.param.TryGetValue("ftype", out var t);
            WoAutoCollectionPlugin.GameData.param.TryGetValue("fexchangeItem", out var fe);
            if (!int.TryParse(t, out var type))
            {
                PluginLog.Log($"ftype 参数错误");
                return;
            }
            if (!int.TryParse(fe, out var exchangeItem)) {
                PluginLog.Log($"fexchangeItem 参数错误");
                return;
            }

            string fishName = "";
            if (type == 1)
            {
                fishName = Position.PurpleFishName;
            }
            else if (type == 2)
            {
                fishName = Position.WhiteFishName;
            }
            else if (type == 3)
            {
                fishName = Position.FishNameSandA;
            }
            else if (type == 4)
            {
                fishName = Position.FishNameSandB;
            }
            else {
                PluginLog.Log($"ftype 参数错误");
                return;
            }
            while (!closed && n < 10)
            {
                try
                {
                    (uint Category, uint Sub, uint ItemId) = RecipeItems.UploadApply(fishName);
                    if (type <= 2) {
                        // 大地白票 紫票
                        if (BagManager.GetInventoryItemCountById(ItemId) > 0)
                        {
                            if (closed)
                            {
                                PluginLog.Log($"task shop stopping");
                                return;
                            }
                            if (DalamudApi.ClientState.TerritoryType - Position.ShopTerritoryType != 0)
                            {
                                Teleporter.Teleport(Position.ShopTp);
                                Thread.Sleep(12000);
                            }
                            else
                            {
                                PluginLog.Log($"不需要TP");
                            }

                            if (DalamudApi.ClientState.TerritoryType - Position.ShopTerritoryType != 0)
                            {
                                PluginLog.Log($"不在任务区域");
                                continue;
                            }
                            if (CommonUi.CanRepair())
                            {
                                MovePositions(Position.RepairNPC, false);
                                WoAutoCollectionPlugin.GameData.CommonBot.NpcRepair("阿塔帕");
                            }
                            Thread.Sleep(5000);
                            MovePositions(Position.UploadNPC, false);

                            int k = 0;
                            while (BagManager.GetInventoryItemCountById(ItemId) > 0 && k < 3)
                            {
                                if (closed)
                                {
                                    PluginLog.Log($"UploadAndExchange stopping");
                                    return;
                                }
                                if (WoAutoCollectionPlugin.GameData.CommonBot.CraftUpload(Category, Sub, ItemId))
                                {
                                    MovePositions(Position.ExchangeNPC, false);
                                    WoAutoCollectionPlugin.GameData.CommonBot.CraftExchange(exchangeItem);
                                    if (BagManager.GetInventoryItemCountById(ItemId) > 0)
                                    {
                                        MovePositions(Position.ExchangeToUploadNPC, false);
                                    }
                                }
                                k++;
                            }
                        }
                    } else if (type >= 3) { 
                        // 精选 TODO
                    }

                    if (BagManager.InventoryRemaining() > 5)
                    {
                        if (closed)
                        {
                            PluginLog.Log($"task fish stopping");
                            return;
                        }
                        RunCollectionFishScript(type);
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
            DalamudApi.Framework.Update -= OnCollectionFishUpdate;
        }

        // 前往指定钓鱼地点 [√]
        // 钓鱼   [√]
        // 清背包 (换工票/精选) [√]
        public bool RunCollectionFishScript(int type)
        {
            fishsw = new();

            ushort SizeFactor = WoAutoCollectionPlugin.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            // 划分区域
            Vector3[] ToArea = Array.Empty<Vector3>();
            Vector3[] FishArea = Array.Empty<Vector3>();
            if (type == 1)
            {
                ToArea = Position.ToPurpleFishArea;
                FishArea = Position.PurpleFishArea;
                fishTime = Position.PurpleFishTime;
            } else if (type == 2) {
                ToArea = Position.ToWhiteFishArea;
                FishArea = Position.WhiteFishArea;
                fishTime = Position.WhiteFishTime;
                fishTp = Position.WhiteFishTp;
                fishTerritoryType = Position.WhiteFishTerritoryType;
            } else if (type == 3) 
            { 
                // 时间限制
            }
            else if (type == 4)
            {

            }

            Teleporter.Teleport(fishTp);
            Thread.Sleep(12000);

            if (DalamudApi.ClientState.TerritoryType - fishTerritoryType != 0)
            {
                PluginLog.Log($"不在任务区域");
                return false;
            }

            Vector3 position = WoAutoCollectionPlugin.GameData.KeyOperates.GetUserPosition(SizeFactor);
            // 通过路径到达固定区域位置
            position = MovePositions(ToArea, true);

            int tt = 0;
            while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 3)
            {
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.q_key);
                Thread.Sleep(1000);
                tt++;
            }

            // 在固定区域到达作业点 作业循环 40min切换  0->2->1->2->0....
            int currentPoint = 0;
            Stopwatch sw = new();

            for (int i = 0; i <= 10; i++)
            {
                Init();
                sw.Reset();
                if (closed)
                {
                    PluginLog.Log($"中途结束");
                    return false;
                }
                sw.Start();

                position = WoAutoCollectionPlugin.GameData.KeyOperates.MoveToPoint(position, FishArea[2], DalamudApi.ClientState.TerritoryType, false, false);
                position = WoAutoCollectionPlugin.GameData.KeyOperates.MoveToPoint(position, FishArea[currentPoint], DalamudApi.ClientState.TerritoryType, false, false);
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
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.w_key, 200);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n2_key);

                while (sw.ElapsedMilliseconds / 1000 / 60 < 44)
                {
                    Thread.Sleep(1000);
                    if (closed)
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
                    if (closed)
                    {
                        PluginLog.Log($"中途结束");
                        return false;
                    }
                    if (!(DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing]))
                    {
                        canMove = true;
                    }
                }
                
                // 判断是否需要精制
                int count = CommonUi.CanExtractMateria();
                if (count >= 5)
                {
                    WoAutoCollectionPlugin.GameData.CommonBot.ExtractMateria(count);
                }
                // 判断是否需要修理
                if (CommonUi.NeedsRepair())
                {
                    return true;
                }

                if (BagManager.InventoryRemaining() <= 5) {
                    return true;
                }
            }
            PluginLog.Log($"任务结束");
            return true;
        }

        public void StopCollectionFishScript()
        {
            closed = true;
            WoAutoCollectionPlugin.GameData.KeyOperates.ForceStop();
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
            Task task = new(() =>
            {
                PluginLog.Log($"CFish bit with {Record.Tug} fish time: {fishsw.ElapsedMilliseconds / 1000}");
                if (fishsw.ElapsedMilliseconds / 1000 >= fishTime)
                {
                    switch (Record.Tug.ToString())
                    {
                        case "Weak":
                            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n3_key);
                            break;
                        case "Strong":
                            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n4_key);
                            break;
                        case "Legendary":
                            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n4_key);
                            break; 
                        default:
                            break;
                    }
                }
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n1_key);
            });
            task.Start();
        }

        private void CollectionFishScript()
        {
            Task task = new(() =>
            {
                if (BagManager.InventoryRemaining() <= 5) {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F1_key);
                    return;
                }
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
                            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n0_key);
                            gp += 150;
                            Thread.Sleep(1000);
                        }
                    }
                    if (gp < maxGp * 0.5)
                    {
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.minus_key);
                        Thread.Sleep(1000);
                    }
                    if (!existStatus)
                    {
                        Thread.Sleep(3000);
                        if (gp > 560)
                        {
                            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F4_key);
                            Thread.Sleep(1000);
                            existStatus = true;
                            gp -= 560;
                        }
                    }
                    if (LastFish && gp > 350)
                    {
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F5_key);
                        Thread.Sleep(1000);
                    }
                    LastFish = false;
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n8_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n2_key);
                }
                else {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F1_key);
                }
            });
            task.Start();
        }

        protected void MakeSure()
        {
            LastFish = true;
            Task task = new(() =>
            {
                Thread.Sleep(800);
                if (CommonUi.AddonSelectYesnoIsOpen()) {
                    CommonUi.SelectYesButton();
                }
                Thread.Sleep(300);
            });
            task.Start();
        }

        private Vector3 MovePositions(Vector3[] Path, bool UseMount)
        {
            ushort SizeFactor = WoAutoCollectionPlugin.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            Vector3 position = WoAutoCollectionPlugin.GameData.KeyOperates.GetUserPosition(SizeFactor);
            for (int i = 0; i < Path.Length; i++)
            {
                if (closed)
                {
                    PluginLog.Log($"中途结束");
                    return WoAutoCollectionPlugin.GameData.KeyOperates.GetUserPosition(SizeFactor);
                }
                position = WoAutoCollectionPlugin.GameData.KeyOperates.MoveToPoint(position, Path[i], DalamudApi.ClientState.TerritoryType, UseMount, false);
            }
            return position;
        }
    }
}