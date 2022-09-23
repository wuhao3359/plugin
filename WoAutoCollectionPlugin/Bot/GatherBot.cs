using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.UseAction;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class GatherBot
    {
        private GameData GameData { get; init; }
        private KeyOperates KeyOperates { get; init; }

        private CommonBot? CommonBot;

        private static bool closed = false;
        private int count = 0; 

        public GatherBot(GameData GameData)
        {
            this.GameData = GameData;
            KeyOperates = new KeyOperates(GameData);
            CommonBot = new CommonBot(KeyOperates);
        }

        public void Init()
        {
            closed = false;
            count = 0;
        }

        // 普通采集点
        public bool RunNormalScript(int area)
        {
            Init();
            ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            Vector3[] Area = Array.Empty<Vector3>();
            int[] index = Array.Empty<int>();
            int[] indexNum = Array.Empty<int>();
            int[] ABC = Array.Empty<int>();
            int GathingButton = 0;

            (Area, index, indexNum, ABC) = GetArea(area);
            // TODO 传送
            // 去起始点O

            Vector3 position = KeyOperates.GetUserPosition(SizeFactor);
            PluginLog.Log($"采集 {position.X} {position.Y} {position.Z}");

            Thread.Sleep(500);

            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            List< GameObject > gameObjects = new();
            List< int > gameObjectsIndex = new();
            for (int i = 0, j = 0, k = 0 ; i < Area.Length; i++)
            {
                GathingButton = GetGathingButton(area);
                if (closed)
                {
                    PluginLog.Log($"中途结束");
                    return false;
                }

                if (ABC[k] == i && gameObjects.ToArray().Length == 0 && j < index.Length) {
                    (gameObjects, gameObjectsIndex) = Util.GetCanGatherPosition(Area, index, j, SizeFactor);
                    j += indexNum[k];
                    k++;

                    if (k > 2) {
                        k = 2;
                    }
                }
                if (gameObjectsIndex.ToArray().Length > 0)
                {
                    PluginLog.Log($"采集点 {gameObjectsIndex[0]}");
                }

                if (Array.IndexOf(index, i) != -1)
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
                            KeyOperates.KeyMethod(Keys.e_key);
                            position = KeyOperates.MoveToPoint(position, Area[i], territoryType, false, false);
                            PluginLog.Log($"到达到达点{i} {position.X} {position.Y} {position.Z}");
                            var targetMgr = DalamudApi.TargetManager;
                            if (go.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.GatheringPoint)
                            {
                                if (DalamudApi.Condition[ConditionFlag.Mounted])
                                {
                                    KeyOperates.KeyMethod(Keys.q_key);
                                    Thread.Sleep(500);
                                }
                                PluginLog.Log($"work: {go.ObjectId} type: {go.ObjectKind}");
                                targetMgr.SetTarget(go);

                                KeyOperates.KeyMethod(Keys.num0_key);
                                Thread.Sleep(500);
                                PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                                uint gp = player.CurrentGp;
                                if (area < 100)
                                {
                                    if (gp > 501)
                                    {
                                        KeyOperates.KeyMethod(Keys.F3_key);
                                        Thread.Sleep(2000);
                                    }
                                }
                                else {
                                    if (gp > 351)
                                    {
                                        KeyOperates.KeyMethod(Keys.F4_key);
                                        Thread.Sleep(2000);
                                        KeyOperates.KeyMethod(Keys.F5_key);
                                        Thread.Sleep(2000);
                                    }
                                    else {
                                        GathingButton = Position.Gatheing3Button;
                                    }
                                }

                                if (!CommonUi.GatheringButton(GathingButton)) {
                                    while (CommonUi.AddonGatheringIsOpen()) {
                                        KeyOperates.KeyMethod(Keys.num0_key);
                                        Thread.Sleep(500);
                                    }
                                }

                                // TODO
                                KeyOperates.KeyMethod(Keys.num0_key);
                                Thread.Sleep(500);
                                if (!DalamudApi.Condition[ConditionFlag.Gathering42]) {
                                    KeyOperates.KeyMethod(Keys.num0_key);
                                }
                                if (!DalamudApi.Condition[ConditionFlag.Gathering42])
                                {
                                    PluginLog.Log($"未知原因 skip...");
                                    if (gameObjects.ToArray().Length > 0)
                                    {
                                        gameObjects.RemoveAt(0);
                                        gameObjectsIndex.RemoveAt(0);
                                    }
                                    continue;
                                }
                                while (DalamudApi.Condition[ConditionFlag.Gathering42])
                                {
                                    Thread.Sleep(1000);
                                }
                                
                                if (gameObjects.ToArray().Length > 0)
                                {
                                    gameObjects.RemoveAt(0);
                                    gameObjectsIndex.RemoveAt(0);
                                }
                            }
                        }
                        else {
                            PluginLog.Log($"从 点{j} 开始");
                        }
                    }
                }
                else
                {
                    PluginLog.Log($"去点{i} { Area[i].X} { Area[i].Y} { Area[i].Z}");
                    CommonBot.RepairAndExtractMateria();

                    position = KeyOperates.MoveToPoint(position, Area[i], territoryType, true, false);
                    PluginLog.Log($"到达点{i} {position.X} {position.Y} {position.Z}");
                    PluginLog.Log($"not work point {i}");
                }
            }
            return true;
        }

        public void RunYGatherScript(string args) {
            string[] str = args.Split(' ');
            int area = int.Parse(str[0]);
            int repair = 0;
            if (str.Length >= 2)
            {
                repair = int.Parse(str[1]);
            }

            Init();
            KeyOperates.KeyMethod(Keys.down_arrow_key, 175);
            ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            Vector3 position = KeyOperates.GetUserPosition(SizeFactor);

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
            else
            {
                PluginLog.Log($"不需要修理装备...");
            }
            int count = RepairUi.CanExtractMateria();
            if (count >= 5)
            {
                PluginLog.Log($"开始精制魔晶石...");
                CommonBot.ExtractMateria(count);
            }

            Vector3[] AreaPosition = Array.Empty<Vector3>();
            int[] Tp = Array.Empty<int>();
            int[] GatherPosition = Array.Empty<int>();
            int GathingButton = 1;

            (AreaPosition, Tp, GatherPosition, GathingButton) = GetAreaByType(area);

            for (int k = 0; k < AreaPosition.Length; k++) {
                if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                {
                    PluginLog.Log($"YGathing stopping");
                    return;
                }
                position = KeyOperates.MoveToPoint(position, AreaPosition[k], territoryType, true, false);
                PluginLog.Log($"到达{k} TP点{Array.IndexOf(Tp, k) != -1} 采集点{Array.IndexOf(GatherPosition, k) != -1}");
                // 默认为移动点
                // 传送点
                if (Array.IndexOf(Tp, k) != -1) {
                    if (!DalamudApi.Condition[ConditionFlag.Jumping]) {
                        KeyOperates.KeyMethod(Keys.w_key, 1500);
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
                    GameObject? go = Util.CurrentPositionCanGather(KeyOperates.GetUserPosition(SizeFactor), SizeFactor);
                    if (go != null)
                    {
                        float x = Maths.GetCoordinate(go.Position.X, SizeFactor);
                        float y = Maths.GetCoordinate(go.Position.Y, SizeFactor);
                        float z = Maths.GetCoordinate(go.Position.Z, SizeFactor);
                        Vector3 GatherPoint = new Vector3(x, y, z);
                        position = KeyOperates.MoveToPoint(position, GatherPoint, territoryType, false, false);
                        PluginLog.Log($"到达可采集点: {position.X} {position.Y} {position.Z}");
                        var targetMgr = DalamudApi.TargetManager;
                        targetMgr.SetTarget(go);

                        int n = 0;
                        while (DalamudApi.Condition[ConditionFlag.Mounted])
                        {
                            if (n >= 3) {
                                KeyOperates.KeyMethod(Keys.w_key, 200);
                            }
                            KeyOperates.KeyMethod(Keys.q_key);
                            Thread.Sleep(1000);
                            n++;

                            if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                            {
                                PluginLog.Log($"YGathing stopping");
                                return;
                            }
                        }
                        KeyOperates.KeyMethod(Keys.num0_key);
                        Thread.Sleep(500);
                        PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                        uint gp = player.CurrentGp;
                        int level = player.Level;
                        if (level >= 80) {
                            if (gp >= 500)
                            {
                                KeyOperates.KeyMethod(Keys.F3_key);
                                Thread.Sleep(2000);
                            }
                        } else {
                            if (gp >= 400)
                            {
                                KeyOperates.KeyMethod(Keys.F2_key);
                                Thread.Sleep(2000);
                            }
                        }

                        if (CommonUi.AddonGatheringIsOpen()) {
                            Thread.Sleep(1000);
                            CommonUi.GatheringButton(GathingButton);
                        }

                        Thread.Sleep(500);
                        while (!DalamudApi.Condition[ConditionFlag.Gathering42])
                        {
                            KeyOperates.KeyMethod(Keys.num0_key);
                            Thread.Sleep(500);

                            if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                            {
                                PluginLog.Log($"YGathing stopping");
                                return;
                            }
                        }
                        if (DalamudApi.Condition[ConditionFlag.Gathering42])
                        {
                            count += 5;
                        }
                        else {
                            k--;
                        }
                        while (DalamudApi.Condition[ConditionFlag.Gathering42])
                        {
                            Thread.Sleep(1000);

                            if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
                            {
                                PluginLog.Log($"YGathing stopping");
                                return;
                            }
                        }
                        if (count >= 40) {
                            KeyOperates.KeyMethod(Keys.up_arrow_key, 175);
                            Thread.Sleep(500);
                            Game.UseSpellActionById(19700);
                            if (DalamudApi.Condition[ConditionFlag.Casting]) {
                                count -= 40;
                                Thread.Sleep(6800);
                            }
                            KeyOperates.KeyMethod(Keys.down_arrow_key, 175);
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
            KeyOperates.ForceStop();
        }

        public (Vector3[], int[], int[], int[]) GetArea(int area) {
            Vector3[] Area = Array.Empty<Vector3>();
            int[] index = Array.Empty<int>();
            int[] indexNum = Array.Empty<int>();
            int[] ABC = Array.Empty<int>();

            if (area == 1)
            {   // 1-稻槎草(园:68)
                Area = Position.TestArea;
                index = Position.TestIndex;
                indexNum = Position.TestIndexNum3;
                ABC = Position.TestABC;
            }
            else if (area == 2)
            {
                // 2-繁缕(园:68)
                Area = Position.TestArea;
                index = Position.TestIndex;
                indexNum = Position.TestIndexNum3;
                ABC = Position.TestABC;
            }
            else if (area == 3)
            {
                // 3-棕榈糖浆(园:82)
                Area = Position.Area3;
                index = Position.Index3;
                indexNum = Position.IndexNum3;
                ABC = Position.ABC3;
            }
            else if (area == 4)
            {
                // 4-葛根(园:65)
                Area = Position.Area4;
                index = Position.Index4;
                indexNum = Position.IndexNum4;
                ABC = Position.ABC4;
            }
            else if (area == 100)
            {
                // 100-火水晶(lv.68)
                Area = Position.Area3;
                index = Position.Index3;
                indexNum = Position.IndexNum3;
                ABC = Position.ABC3;
            }
            // TODO 5-大蜜蜂的巢(园:75) 6-巨人新薯(园:87) 7-山地小麦(园:73) 灵银矿(矿:53) 灵银沙(矿:51)

            return (Area, index, indexNum, ABC);
        }

        public (Vector3[], int[], int[], int) GetAreaByType(int type) {
            Vector3[] AreaPosition = Array.Empty<Vector3>();
            int[] Tp = Array.Empty<int>();
            int[] GatherPosition = Array.Empty<int>();
            int GathingButton = type;

            if (type <= 10)
            {
                AreaPosition = Position.AreaPositionA;
                Tp = Position.TpA;
                GatherPosition = Position.GatherPositionA;
                if (type >= 10) {
                    GathingButton = type - 10;
                }
            }
            else { 
                
            }
            return (AreaPosition, Tp, GatherPosition, GathingButton);
        }

        public int GetGathingButton(int area) {
            int GathingButton = 0;

            if (area == 1)
            {   // 1-稻槎草
                GathingButton = Position.Gatheing1Button;
            }
            else if (area == 2)
            {
                // 2-繁缕
                GathingButton = Position.Gatheing2Button;
            }
            else if (area == 3)
            {
                // 3-棕榈糖浆
                GathingButton = Position.Gatheing3Button;
            }
            else if (area == 4)
            {
                // 4-葛根
                GathingButton = Position.Gatheing4Button;
            }
            else if (area == 100)
            {
                // 100-火水晶
                GathingButton = Position.Gatheing100Button;
            }
            return GathingButton;
        }
    }
}