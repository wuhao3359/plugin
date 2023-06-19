using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using AlphaProject.Data;
using AlphaProject.Managers;
using AlphaProject.SeFunctions;
using AlphaProject.Ui;
using AlphaProject.Utility;
using AlphaProject.Helper;

namespace AlphaProject.Bot
{
    public static class CollectionFishBot
    {

        //private static SeTugType TugType { get; set; } = null!;

        //private FishRecord Record;

        private static FishingState LastState = FishingState.None;
        private static FishingState FishingState = FishingState.None;
        static Stopwatch Fishsw = new();
        private static bool CanMove = false;
        private static bool readyMove = false;
        private static bool Closed = false;

        private static bool LastFish = false;

        private static int FishTime = 0;
        private static uint FishTp = 0;
        private static ushort FishTerritoryType = 0;


        public static void Init()
        {
            CanMove = false;
            readyMove = false;
            CommonBot.Init();
        }

        public static void StopScript()
        {
            Closed = true;
            KeyOperates.ForceStop();
        }

        public static void CollectionFishScript(string args) {
            Closed = false;
            int n = 0;
            string command = Tasks.CFish;
            AlphaProject.GameData.param = Util.CommandParse(command, args);

            AlphaProject.GameData.param.TryGetValue("ftype", out var t);
            AlphaProject.GameData.param.TryGetValue("fexchangeItem", out var fe);
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
                fishName = Positions.PurpleFishName;
            }
            else if (type == 2)
            {
                fishName = Positions.WhiteFishName;
            }
            else if (type == 3)
            {
                fishName = Positions.FishNameSandA;
            }
            else if (type == 4)
            {
                fishName = Positions.FishNameSandA;
            }
            else {
                PluginLog.Log($"ftype 参数错误");
                return;
            }

            if (!CommonUi.CurrentJob(18))
            {
                Thread.Sleep(500);
                CommandProcessorHelper.DoGearChange("捕鱼人");
                Thread.Sleep(500);
            }

            DalamudApi.Framework.Update += OnCollectionFishUpdate;
            while (!Closed && n < 10)
            {
                if (Closed)
                {
                    PluginLog.Log($"task fish stopping");
                    break;
                }
                try
                {
                    (uint Category, uint Sub, uint ItemId) = RecipeItems.UploadApply(fishName);
                    if (type <= 2) {
                        // 大地白票 紫票
                        if (BagManager.GetInventoryItemCountById(ItemId) > 0)
                        {
                            if (Closed)
                            {
                                PluginLog.Log($"task shop stopping");
                                break;
                            }
                            if (DalamudApi.ClientState.TerritoryType - Positions.ShopTerritoryType != 0)
                            {
                                Teleporter.Teleport(Positions.ShopTp);
                            }
                            else
                            {
                                PluginLog.Log($"不需要TP");
                            }

                            if (DalamudApi.ClientState.TerritoryType - Positions.ShopTerritoryType != 0)
                            {
                                PluginLog.Log($"不在任务区域");
                                continue;
                            }
                            if (CommonUi.CanRepair())
                            {
                                KeyOperates.MovePositions(Positions.RepairNPC, false);
                                CommonBot.NpcRepair("阿塔帕");
                            }
                            Thread.Sleep(5000);
                            // MovePositions(Positions.UploadNPC, false);

                            // TODO
                            //int k = 0;
                            //while (BagManager.GetInventoryItemCountById(ItemId) > 0 && k < 3)
                            //{
                            //    if (Closed)
                            //    {
                            //        PluginLog.Log($"UploadAndExchange stopping");
                            //        break;
                            //    }
                            //    if (CommonBot.CraftUpload(Category, Sub, ItemId))
                            //    {
                            //        MovePositions(Positions.ExchangeNPC, false);
                            //        if (exchangeItem > 100)
                            //        {
                            //            for (int tt = 0; tt <= 3; tt++)
                            //            {
                            //                if (Closed)
                            //                {
                            //                    PluginLog.Log($"Exchange stopping");
                            //                    break;
                            //                }
                            //                CommonBot.CraftExchange(exchangeItem);
                            //            }
                            //        }
                            //        else {
                            //            CommonBot.CraftExchange(exchangeItem);
                            //        }
                            //        if (BagManager.GetInventoryItemCountById(ItemId) > 0)
                            //        {
                            //            MovePositions(Positions.ExchangeToUploadNPC, false);
                            //        }
                            //    }
                            //    k++;
                            //}
                        }
                    }
                    if (BagManager.InventoryRemaining() > 5)
                    {
                        if (Closed)
                        {
                            PluginLog.Log($"task fish stopping");
                            break;
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
            if (type >= 3)
            {
                int CollectableCount = CommonUi.CanExtractMateriaCollectable();
                if (CollectableCount > 0)
                {
                    CommonBot.ExtractMateriaCollectable(CollectableCount);
                }
            }
        }

        // 刺鱼
        public static void SpearfishScript(string args) {
            Closed = false;
            int n = 0;
            string command = Tasks.SpearFish;
            AlphaProject.GameData.param = Util.CommandParse(command, args);

            AlphaProject.GameData.param.TryGetValue("ftype", out var t);
            if (!int.TryParse(t, out var type))
            {
                PluginLog.Log($"ftype 参数错误");
                return;
            }

            if (!CommonUi.CurrentJob(18))
            {
                Thread.Sleep(300 + new Random().Next(200, 500));
                AlphaProject.Executor.DoGearChange("捕鱼人");
                Thread.Sleep(300 + new Random().Next(200, 500));
            }

            while (!Closed && n < 10) {
                if (Closed)
                {
                    PluginLog.Log($"task fish stopping");
                    break;
                }

                try {
                    if (BagManager.InventoryRemaining() > 5)
                    {
                        if (Closed)
                        {
                            PluginLog.Log($"task fish stopping");
                            break;
                        }
                        RunSpearfishScript();
                    }
                }
                catch (Exception e)
                {
                    PluginLog.Error($"刺鱼 error!!!\n{e}");
                }
                n++;
            }
        }

        // 前往指定钓鱼地点 [√]
        // 钓鱼   [√]
        // 清背包 (换工票/精选) [√]
        public static bool RunCollectionFishScript(int type)
        {
            Fishsw = new();

            ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            // 划分区域
            Vector3[] ToArea = Array.Empty<Vector3>();
            Vector3[] FishArea = Array.Empty<Vector3>();
            if (type == 1)
            {
                ToArea = Positions.ToPurpleFishArea;
                FishArea = Positions.PurpleFishArea;
                FishTime = Positions.PurpleFishTime;
                FishTp = Positions.PurpleFishTp;
                FishTerritoryType = Positions.PurpleFishTerritoryType;
            } else if (type == 2) {
                ToArea = Positions.ToWhiteFishArea;
                FishArea = Positions.WhiteFishArea;
                FishTime = Positions.WhiteFishTime;
                FishTp = Positions.WhiteFishTp;
                FishTerritoryType = Positions.WhiteFishTerritoryType;
            } else if (type == 3) 
            {
                // 红弓鳍鱼 灵砂 地点A
                ToArea = Positions.ToFishAreaSandA;
                FishArea = Positions.FishAreaSandA1;
                FishTime = Positions.SandFishTimeA1;
                FishTp = Positions.SandFishTp;
                FishTerritoryType = Positions.SandFishTerritoryType;
            }
            else if (type == 4)
            {
                // 红弓鳍鱼 灵砂 地点B
                ToArea = Positions.ToFishAreaSandA;
                FishArea = Positions.FishAreaSandA2;
                FishTime = Positions.SandFishTimeA1;
                FishTp = Positions.SandFishTp;
                FishTerritoryType = Positions.SandFishTerritoryType;
            }

            Vector3 position = KeyOperates.GetUserPosition(SizeFactor);
            Teleporter.Teleport(FishTp);

            if (DalamudApi.ClientState.TerritoryType - FishTerritoryType != 0)
            {
                PluginLog.Log($"不在任务区域");
                return false;
            }

            // 通过路径到达固定区域位置
            position = KeyOperates.MovePositions(ToArea, true);

            int tt = 0;
            while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 3)
            {
                KeyOperates.KeyMethod(Keys.q_key);
                Thread.Sleep(800 + new Random().Next(300, 800));
                tt++;
            }

            // 在固定区域到达作业点 作业循环 40min切换  0->2->1->2->0....
            int currentPoint = 0;
            Stopwatch sw = new();

            for (int i = 0; i <= 10; i++)
            {
                Init();
                sw.Reset();
                if (Closed)
                {
                    PluginLog.Log($"中途结束...");
                    int ii = 0;
                    while (DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing]) {
                        PluginLog.Log($"正在停止钓鱼中...");
                        KeyOperates.KeyMethod(Keys.F1_key);
                        if (CommonUi.AddonSelectYesnoIsOpen())
                        {
                            CommonUi.SelectYesButton();
                        }
                        Thread.Sleep(800 + new Random().Next(300, 800));
                        if (ii > 15)
                        {
                            break;
                        }
                        ii++;
                    }
                    return false;
                }
                sw.Start();

                position = KeyOperates.MoveToPoint(position, FishArea[2], DalamudApi.ClientState.TerritoryType, false, false);
                position = KeyOperates.MoveToPoint(position, FishArea[currentPoint], DalamudApi.ClientState.TerritoryType, false, false);
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
                if (!CommonUi.HasStatus("收藏品采集"))
                {
                    KeyOperates.KeyMethod(Keys.n5_key);
                    Thread.Sleep(200 + new Random().Next(300, 800));
                }
                if (!CommonUi.HasStatus("钓上大尺寸的鱼几率提升"))
                {
                    KeyOperates.KeyMethod(Keys.F4_key);
                    Thread.Sleep(100 + new Random().Next(300, 600));
                }
                KeyOperates.KeyMethod(Keys.n2_key);

                while (sw.ElapsedMilliseconds / 1000 / 60 < 37)
                {
                    Thread.Sleep(800 + new Random().Next(300, 800));
                    if (Closed)
                    {
                        PluginLog.Log($"中途结束");
                        break;
                    }
                    if (!(DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing]))
                    {
                        break;
                    }
                }

                readyMove = true;
                while (!CanMove)
                {
                    Thread.Sleep(800 + new Random().Next(300, 800));
                    if (Closed)
                    {
                        PluginLog.Log($"中途结束, 等待收杆...");
                        KeyOperates.KeyMethod(Keys.F1_key);
                    }
                    if (!(DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing]))
                    {
                        CanMove = true;
                    }
                }

                if (CommonUi.NeedsRepair())
                {
                    return true;
                }

                if (BagManager.InventoryRemaining() <= 5) {
                    return true;
                }
            }
            PluginLog.Log($"捕鱼任务结束");
            return true;
        }

        public static bool RunSpearfishScript() {
            (int Id, string Name, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, int[] CanCollectPoints, int[] UnknownPointsNum, int[] Area) = Positions.GetSpearfish();
            PluginLog.Log($"开始执行任务, id: {Id} Name: {Name}, Job: {Job}..");
            if (Tp <= 0)
            {
                PluginLog.Log($"数据异常, skip {Id}..");
                return false;
            }
            Teleporter.Teleport(Tp);

            CommonBot.UseItem();
            // 切换职业 
            if (!CommonUi.CurrentJob(Job))
            {
                Thread.Sleep(2000 + new Random().Next(100, 800));
                CommandProcessorHelper.DoGearChange(JobName);
                Thread.Sleep(200 + new Random().Next(300, 800));
            }
            KeyOperates.MovePositions(Path, true);
            if (!CommonUi.HasStatus("收藏品采集"))
            {
                KeyOperates.KeyMethod(Keys.n5_key);
                Thread.Sleep(200 + new Random().Next(300, 800));
            }
            int n = 0;
            while (!Closed & n < 100)
            {
                ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
                Vector3 position = KeyOperates.GetUserPosition(SizeFactor);
                ushort territoryType = DalamudApi.ClientState.TerritoryType;
                List<GameObject> gameObjects = new();
                List<int> gameObjectsIndex = new();

                for (int i = 0, j = 0, k = 0; i < Points.Length; i++)
                {
                    if (Closed)
                    {
                        PluginLog.Log($"中途结束");
                        return false;
                    }
                    if (gameObjects.ToArray().Length == 0 && Area[k] == i && j < CanCollectPoints.Length)
                    {
                        (gameObjects, gameObjectsIndex) = Util.GetCanGatherPosition(Points, CanCollectPoints, j, UnknownPointsNum[k], SizeFactor);
                        if (k < UnknownPointsNum.Length - 1)
                        {
                            j += UnknownPointsNum[k];
                            k++;
                        }
                    }

                    if (Array.IndexOf(CanCollectPoints, i) != -1)
                    {
                        if (gameObjectsIndex.ToArray().Length > 0 && gameObjectsIndex[0] != i)
                        {
                            PluginLog.Log($"skip point {i}");
                            continue;
                        }

                        if (gameObjects.ToArray().Length > 0)
                        {
                            GameObject go = gameObjects[0];
                            if (go != null)
                            {
                                if (!DalamudApi.Condition[ConditionFlag.Mounted])
                                {
                                    CommonBot.UseItem();
                                    KeyOperates.KeyMethod(Keys.e_key);
                                }
                                float x = Maths.GetCoordinate(go.Position.X, AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType));
                                float y = Maths.GetCoordinate(go.Position.Y, AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType));
                                float z = Maths.GetCoordinate(go.Position.Z, AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType));
                                Vector3 GatherPoint = new(x, y, z);
                                position = KeyOperates.MoveToPoint(position, Points[i], territoryType, false, false);
                                position = KeyOperates.MoveToPoint(position, GatherPoint, territoryType, false, false);
                                if (go.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.GatheringPoint)
                                {
                                    var targetMgr = DalamudApi.TargetManager;
                                    targetMgr.SetTarget(go);

                                    int tt = 0;
                                    while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 5)
                                    {
                                        if (tt >= 2)
                                        {
                                            KeyOperates.KeyMethod(Keys.w_key, 200);
                                        }
                                        KeyOperates.KeyMethod(Keys.q_key);
                                        Thread.Sleep(800 + new Random().Next(300, 800));
                                        tt++;

                                        if (Closed)
                                        {
                                            PluginLog.Log($"task stopping");
                                            return true;
                                        }
                                    }
                                    tt = 0;
                                    while (!CommonUi.AddonSpearFishingIsOpen() && tt < 7)
                                    {
                                        KeyOperates.KeyMethod(Keys.num0_key);
                                        if (tt == 3 || tt == 4 || tt == 4)
                                        {
                                            KeyOperates.KeyMethod(Keys.down_arrow_key);
                                        }
                                        Thread.Sleep(500 + new Random().Next(500, 800));
                                        tt++;
                                        if (tt >= 6) {
                                            Thread.Sleep(1000 + new Random().Next(500, 1000));
                                            if (!CommonUi.AddonSpearFishingIsOpen()) {
                                                KeyOperates.AdjustHeight(GatherPoint);
                                            }
                                        }
                                    }
                                    if (tt >= 7)
                                    {
                                        PluginLog.Log($"未打开采集面板, skip {i}..");
                                        if (gameObjects.ToArray().Length > 0)
                                        {
                                            gameObjects.RemoveAt(0);
                                            gameObjectsIndex.RemoveAt(0);
                                        }
                                        continue;
                                    }
                                    Thread.Sleep(300 + new Random().Next(200, 500));
                                    if (CommonUi.AddonSpearFishingIsOpen())
                                    {
                                        CommonBot.SpearfishMethod();
                                        CommonBot.UseItem();

                                        PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                                        byte stackCount = 0;
                                        IEnumerator<Dalamud.Game.ClientState.Statuses.Status> statusList = player.StatusList.GetEnumerator();
                                        while (statusList.MoveNext())
                                        {
                                            // 2778-捕鱼人之识 850-耐心
                                            Dalamud.Game.ClientState.Statuses.Status status = statusList.Current;
                                            uint statusId = status.StatusId;
                                            byte StackCount = status.StackCount;
                                            if (statusId == 2778)
                                            {
                                                stackCount = StackCount;
                                                break;
                                            }
                                        }
                                        if (player.CurrentGp < player.MaxGp * 0.6)
                                        {
                                            if (stackCount >= 3)
                                            {
                                                KeyOperates.KeyMethod(Keys.n0_key);
                                                Thread.Sleep(800 + new Random().Next(200, 500));
                                            }
                                        }
                                    }
                                    if (gameObjects.ToArray().Length > 0)
                                    {
                                        gameObjects.RemoveAt(0);
                                        gameObjectsIndex.RemoveAt(0);
                                    }
                                }
                            }
                            else
                            {
                                PluginLog.Log($"从 点{j} 开始");
                            }
                        }
                    }
                    else
                    {
                        position = KeyOperates.MoveToPoint(position, Points[i], territoryType, true, false);
                    }
                }
                n++;
                int c = CommonUi.CanExtractMateriaCollectable();
                if (c > 20)
                {
                    CommonBot.ExtractMateriaCollectable(c);
                }
            }

            int CollectableCount = CommonUi.CanExtractMateriaCollectable();
            if (CollectableCount > 0)
            {
                CommonBot.ExtractMateriaCollectable(CollectableCount);
            }

            PluginLog.Log($"刺鱼任务结束");
            return true;
        }

        public static void OnCollectionFishUpdate(Framework _)
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
                        Fishsw.Restart();
                        break;
                    case FishingState.Bite:
                        OnCollectionFishBite();
                        break;
                    case FishingState.Reeling:
                        break;
                    case FishingState.PoleReady:
                        if (!Closed)
                        {
                            CollectionFishScript();
                        }
                        else
                        {
                            PluginLog.Log($"中途结束, 正在停止中...");
                        }
                        break;
                    case FishingState.Waiting:
                        MakeSure();
                        break;
                    case FishingState.Quit:
                        CanMove = true;
                        break;
                }
            }
        }

        private static void OnCollectionFishBite()
        {
            //Record.SetTugHook(TugType.Bite, Record.Hook);
            Task task = new(() =>
            {
                PluginLog.Log($"CFish bit with {AlphaProject.GameData.TugType.Bite} fish time: {Fishsw.ElapsedMilliseconds / 1000}");
                if (Fishsw.ElapsedMilliseconds / 1000 >= FishTime)
                {
                    switch (AlphaProject.GameData.TugType.Bite.ToString())
                    {
                        case "Weak":
                            KeyOperates.KeyMethod(Keys.n3_key);
                            break;
                        case "Strong":
                            KeyOperates.KeyMethod(Keys.n4_key);
                            break;
                        case "Legendary":
                            KeyOperates.KeyMethod(Keys.n4_key);
                            break; 
                        default:
                            break;
                    }
                }
                KeyOperates.KeyMethod(Keys.n1_key);
            });
            task.Start();
        }

        private static void CollectionFishScript()
        {
            Task task = new(() =>
            {
                if (BagManager.InventoryRemaining() <= 5) {
                    KeyOperates.KeyMethod(Keys.F1_key);
                    return;
                }
                Thread.Sleep(2000);
                PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                if (!readyMove && !CanMove)
                {
                    byte stackCount = 0;
                    bool existStatus = false;
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
                            Thread.Sleep(800 + new Random().Next(200, 500));
                        }
                    }
                    if (gp < maxGp * 0.5)
                    {
                        KeyOperates.KeyMethod(Keys.plus_key);
                        Thread.Sleep(800 + new Random().Next(200, 500));
                    }
                    if (!existStatus)
                    {
                        Thread.Sleep(3000 + new Random().Next(0, 500));
                        if (gp > 560)
                        {
                            KeyOperates.KeyMethod(Keys.F4_key);
                            Thread.Sleep(800 + new Random().Next(200, 500));
                            existStatus = true;
                            gp -= 560;
                        }
                    }
                    if (LastFish && gp > 350 && CommonUi.HasStatus("钓上大尺寸的鱼几率提升"))
                    {
                        KeyOperates.KeyMethod(Keys.F5_key);
                        Thread.Sleep(800 + new Random().Next(200, 500));
                    }
                    LastFish = false;
                    KeyOperates.KeyMethod(Keys.n8_key);
                    KeyOperates.KeyMethod(Keys.n2_key);
                }
                else {
                    KeyOperates.KeyMethod(Keys.F1_key);
                }
            });
            task.Start();
        }

        public static void MakeSure()
        {
            LastFish = true;
            Task task = new(() =>
            {
                Thread.Sleep(500 + new Random().Next(300, 800));
                if (CommonUi.AddonSelectYesnoIsOpen()) {
                    CommonUi.SelectYesButton();
                }
                Thread.Sleep(200 + new Random().Next(300, 800));
            });
            task.Start();
        }
    }
}