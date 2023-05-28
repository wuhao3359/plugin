using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using AlphaProject.Managers;
using AlphaProject.SeFunctions;
using AlphaProject.Ui;
using AlphaProject.UseAction;
using AlphaProject.Utility;
using AlphaProject.Helper;

namespace AlphaProject.Bot
{
    public class GatherBot
    {
        private static bool closed = false;
        private int gatherCount = 0;

        public Dictionary<string, string> param;

        public GatherBot()
        {
        }

        public void test() {
        }

        public void Init()
        {
            closed = false;
        }

        public void NormalScript(string args) {
            closed = false;
            int n = 0;
            string[] str = args.Split('-');
            while (!closed & n < 1000)
            {
                if (closed)
                {
                    PluginLog.Log($"中途结束");
                    return;
                }
                try
                {
                    for (int i = 0; i < str.Length; i++) {
                        int id = Positions.GetIdByName(str[i]);
                        if (id == 0) {
                            PluginLog.Log($"错误名称: {str[i]}");
                            continue;
                        }
                        RunNormalScript(id, "100");
                        Thread.Sleep(3000);
                    }
                }
                catch (Exception e)
                {
                    PluginLog.Error($"error!!!\n{e}");
                }

                n++;
                PluginLog.Log($"{n} 次结束");
            }
        }

        // 普通采集点
        public bool RunNormalScript(int id, string lv)
        {
            Init();

            (int Id, int MaxBackPack, string Name, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, int[] CanCollectPoints, int[] UnknownPointsNum, int[] Area) = GetData(id, lv);
            if (Id <= 0) {
                PluginLog.Log($"param error");
                return false;
            }
            if (Tp != 0)
            {
                Teleporter.Teleport(Tp);
                if (!CommonUi.CurrentJob(Job))
                {
                    Thread.Sleep(2000);
                    CommandProcessorHelper.DoGearChange(JobName);
                    Thread.Sleep(500);
                }
                MovePositions(Path, true);
            }
            int n = 0;
            int error = 0;
            while (!closed & n < 100)
            {
                ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
                Vector3 position = AlphaProject.GameData.KeyOperates.GetUserPosition(SizeFactor);
                ushort territoryType = DalamudApi.ClientState.TerritoryType;
                List<GameObject> gameObjects = new();
                List<int> gameObjectsIndex = new();

                for (int i = 0, j = 0, k = 0; i < Points.Length; i++)
                {
                    if (closed)
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
                        if (gameObjects.ToArray().Length == 0) {
                            error++;
                            if (error >= 1) {
                                PluginLog.Log($"出现错误停止");
                                return false;
                            }
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
                                    AlphaProject.GameData.CommonBot.UseItem();
                                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.e_key);
                                }
                                float x = Maths.GetCoordinate(go.Position.X, AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType));
                                float y = Maths.GetCoordinate(go.Position.Y, AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType));
                                float z = Maths.GetCoordinate(go.Position.Z, AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType));
                                PluginLog.Log($"目标高度: {Maths.GetCoordinate(go.Position.Y, AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType))} < ---> 辅助点高度: {y}");
                                Vector3 GatherPoint = new(x, y, z);
                                position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, Points[i], territoryType, false, false);
                                position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, GatherPoint, territoryType, false, false);
                                if (go.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.GatheringPoint)
                                {
                                    var targetMgr = DalamudApi.TargetManager;
                                    targetMgr.SetTarget(go);

                                    int tt = 0;
                                    while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 5)
                                    {
                                        if (tt >= 2)
                                        {
                                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.w_key, 200);
                                        }
                                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.q_key);
                                        Thread.Sleep(1000);
                                        tt++;

                                        if (closed)
                                        {
                                            PluginLog.Log($"task stopping");
                                            return true;
                                        }
                                    }

                                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.down_arrow_key, 200);
                                    tt = 0;
                                    while (!CommonUi.AddonGatheringIsOpen() && tt < 7)
                                    {
                                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                                        Thread.Sleep(800);
                                        tt++;
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
                                    Thread.Sleep(1000);

                                    if (CommonUi.AddonGatheringIsOpen())
                                    {
                                        AlphaProject.GameData.CommonBot.NormalMaterialsMethod(Name);
                                    }
                                    if (gameObjects.ToArray().Length > 0)
                                    {
                                        gameObjects.RemoveAt(0);
                                        gameObjectsIndex.RemoveAt(0);
                                    }
                                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.up_arrow_key, 200);
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
                        position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, Points[i], territoryType, true, false);
                    }
                }
                n++;
            }
            return true;
        }

        public void YGatherScript(string args) {
            closed = false;
            int n = 0;
            while (!closed && n < 20)
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
                        gatherCount = 0;
                    }

                    if (DalamudApi.ClientState.TerritoryType - Positions.YunGuanTerritoryType == 0)
                    {
                        RunYGatherScript(args);
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

                Thread.Sleep(3000);
                n++;
            }
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
                MovePositions(ToArea, false);
                // 进入空岛
                if (!CommonUi.AddonSelectStringIsOpen() && !CommonUi.AddonSelectYesnoIsOpen())
                {
                    AlphaProject.GameData.CommonBot.SetTarget("奥瓦埃尔");
                    Thread.Sleep(500);
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

        private Vector3 MovePositions(Vector3[] ToArea, bool UseMount)
        {
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            Vector3 position = AlphaProject.GameData.KeyOperates.GetUserPosition(SizeFactor);
            for (int i = 0; i < ToArea.Length; i++)
            {
                if (closed)
                {
                    PluginLog.Log($"中途结束");
                    return AlphaProject.GameData.KeyOperates.GetUserPosition(SizeFactor);
                }
                position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, ToArea[i], territoryType, UseMount, false);
                PluginLog.Log($"到达点{i} {position.X} {position.Y} {position.Z}");
            }
            return position;
        }

        public void RunYGatherScript(string args) {
            string[] str = args.Split(' ');
            int area = int.Parse(str[0]);
            int repair = 0;

            Init();
            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.down_arrow_key, 225);
            ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            Vector3 position = AlphaProject.GameData.KeyOperates.GetUserPosition(SizeFactor);

            if (!AlphaProject.GameData.param.TryGetValue("extractMateria", out var v)) {
                AlphaProject.GameData.param.Add("extractMateria", "1");
            }
            if (CommonUi.CanRepair())
            {
                PluginLog.Log($"修理装备...");
                position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, Positions.YunGuanRepairNPC, territoryType, false);
                if (repair > 0)
                {
                    AlphaProject.GameData.CommonBot.Repair();
                }
                else
                {
                    AlphaProject.GameData.CommonBot.NpcRepair("修理工");
                }
            }
            else
            {
                PluginLog.Log($"不需要修理装备...");
            }
            Thread.Sleep(500);
            int count = CommonUi.CanExtractMateria();
            if (count >= 5)
            {
                PluginLog.Log($"开始精制魔晶石...");
                AlphaProject.GameData.CommonBot.ExtractMateria(count);
            }

            (Vector3[] AreaPosition, int[] Tp, int[] GatherPosition, int GathingButton) = GetAreaByType(area);

            for (int k = 0; k < AreaPosition.Length; k++) {
                if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                {
                    PluginLog.Log($"YGathing stopping");
                    return;
                }
                position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, AreaPosition[k], territoryType, true, false);
                // 默认为移动点
                // 传送点
                if (Array.IndexOf(Tp, k) != -1) {
                    if (!DalamudApi.Condition[ConditionFlag.Jumping]) {
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.w_key, 1000);
                    }
                    if (DalamudApi.Condition[ConditionFlag.Jumping])
                    {
                        while (DalamudApi.Condition[ConditionFlag.Jumping])
                        {
                            PluginLog.Log($"TPing wait");
                            Thread.Sleep(1000);
                        }
                        Thread.Sleep(500);
                    }
                }

                // 采集点
                if (Array.IndexOf(GatherPosition, k) != -1) {
                    GameObject? go = Util.CurrentYPositionCanGather(AlphaProject.GameData.KeyOperates.GetUserPosition(SizeFactor), SizeFactor);
                    if (go != null)
                    {
                        float x = Maths.GetCoordinate(go.Position.X, SizeFactor);
                        float y = Maths.GetCoordinate(go.Position.Y, SizeFactor);
                        float z = Maths.GetCoordinate(go.Position.Z, SizeFactor);
                        Vector3 GatherPoint = new Vector3(x, y, z);
                        if (Maths.Distance(position, GatherPoint) > 100) {
                            PluginLog.Log($"可采集点太远");
                            continue;
                        }
                        position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, GatherPoint, territoryType, false, false);
                        PluginLog.Log($"到达可采集点: K: {k} {position.X} {position.Y} {position.Z}");
                        var targetMgr = DalamudApi.TargetManager;
                        targetMgr.SetTarget(go);

                        int n = 0;
                        while (DalamudApi.Condition[ConditionFlag.Mounted] && n < 7)
                        {
                            if (n >= 3) {
                                AlphaProject.GameData.KeyOperates.KeyMethod(Keys.w_key, 200);
                            }
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.q_key);
                            Thread.Sleep(1000);
                            n++;

                            if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                            {
                                PluginLog.Log($"YGathing stopping");
                                return;
                            }
                        }

                        int t = 0;
                        while (!CommonUi.AddonGatheringIsOpen() && t < 5)
                        {
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                            Thread.Sleep(500);
                            if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                            {
                                PluginLog.Log($"YGathing stopping");
                                return;
                            }
                            t++;
                        }
                        if (t >= 5) {
                            k--;
                            continue;
                        }
                        Thread.Sleep(500);

                        if (CommonUi.AddonGatheringIsOpen())
                        {
                            PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                            uint gp = player.CurrentGp;
                            int level = player.Level;
                            if (level >= 80)
                            {
                                if (gp >= 500)
                                {
                                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.F2_key);
                                    Thread.Sleep(2000);
                                }
                            }
                            else
                            {
                                if (gp >= 400)
                                {
                                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.F1_key);
                                    Thread.Sleep(2000);
                                }
                            }
                            Thread.Sleep(1000);

                            n = 0;
                            while (CommonUi.AddonGatheringIsOpen() && n < 20)
                            {
                                if (n == 0 && go.Name.ToString().Contains("梦幻")) {
                                    k--;
                                }
                                if (go.Name.ToString().Contains("梦幻"))
                                {
                                    CommonUi.GatheringButton(1);
                                }
                                else {
                                    CommonUi.GatheringButton(GathingButton);
                                }
                                gatherCount++;
                                n++;
                                Thread.Sleep(2000);
                            }
                        }
                        else {
                            k--;
                        }
                        if (gatherCount >= 40) {
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.up_arrow_key, 200);
                            Thread.Sleep(500);
                            try
                            {
                                Game.UseSpellActionById(19700);
                            }
                            catch {
                                PluginLog.Log($"use action 19700 error!!!");
                            }
                            
                            if (DalamudApi.Condition[ConditionFlag.Casting]) {
                                gatherCount -= 40;
                                Thread.Sleep(6800);
                            }
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.down_arrow_key, 225);
                        }
                    }
                    else {
                        PluginLog.Log($"null gather point");
                    }
                }
                if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                {
                    PluginLog.Log($"YGathing stopping");
                    return;
                }
            }
        }

        public void StopScript()
        {
            closed = true;
            AlphaProject.GameData.KeyOperates.ForceStop();
        }

        public (int, int, string, uint, string, uint, uint, Vector3[], Vector3[], int[], int[], int[]) GetData(int id, string lv) {
            if (id == 0)
            {
                List<int> list = Positions.GetMateriaId(lv);
                List<int> li = new();
                for (int i = 0; i < list.Count; i ++)
                {
                    (int Id, int MaxBackPack, string Name, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, int[] CanCollectPoints, int[] UnknownPointsNum, int[] Area) = Positions.GetMaterialById(list[i]);
                    int count = BagManager.GetInventoryItemCount((uint)Id);
                    if (MaxBackPack > count)
                    {
                        li.Add(Id);
                    }
                }
                Random rd = new();
                int r = rd.Next(li.Count);
                PluginLog.Log($"随机采集Count: {r} {li.Count} {list.Count}");
                if (r < 0 || r > li.Count)
                {
                    r = 0;
                }

                if (li.Count == 0) {
                    PluginLog.Log($"固定采集ID: 36086");
                    return Positions.GetMaterialById(36086);
                }
                PluginLog.Log($"随机采集ID: {r} {li[r]} {li.Count} {list.Count}");
                return Positions.GetMaterialById(li[r]);
            }
            else
            {
                return Positions.GetMaterialById(id);
            }
        }

        public (Vector3[], int[], int[], int) GetAreaByType(int type) {
            Vector3[] AreaPosition = Array.Empty<Vector3>();
            int[] Tp = Array.Empty<int>();
            int[] GatherPosition = Array.Empty<int>();
            int GathingButton = type;

            if (type <= 10)
            {
                AreaPosition = Positions.AreaPositionA;
                Tp = Positions.TpA;
                GatherPosition = Positions.GatherPositionA;
                GathingButton = type;
            }
            else {
                AreaPosition = Positions.AreaPositionB;
                Tp = Positions.TpB;
                GatherPosition = Positions.GatherPositionB;
                GathingButton = type - 10;
            }
            return (AreaPosition, Tp, GatherPosition, GathingButton);
        }
    }
}