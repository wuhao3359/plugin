using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class GatherBot
    {
        private GameData GameData { get; init; }
        private KeyOperates KeyOperates { get; init; }

        private CommonBot? CommonBot;

        private static bool closed = false;

        public GatherBot(GameData GameData)
        {
            this.GameData = GameData;
            KeyOperates = new KeyOperates(GameData);
            CommonBot = new CommonBot(KeyOperates);
        }

        public void Init()
        {
            closed = false;
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

            //KeyOperates.KeyMethod(Keys.q_key, 0, true);
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
                            position = KeyOperates.MoveToPoint(position, Area[i], territoryType, true, false);
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
                                int n = 0;
                                while (!CommonUi.AddonGatheringIsOpen() && n < 5) {
                                    KeyOperates.KeyMethod(Keys.num0_key);
                                    Thread.Sleep(500);
                                    n++;
                                    if (closed)
                                    {
                                        PluginLog.Log($"Gathing stopping");
                                        return true;
                                    }
                                }
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
                                    KeyOperates.KeyMethod(Keys.num0_key);
                                    KeyOperates.KeyMethod(Keys.num0_key);
                                    Thread.Sleep(500);
                                }

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
                    if (!DalamudApi.Condition[ConditionFlag.Mounted])
                    {
                        KeyOperates.KeyMethod(Keys.q_key);
                        Thread.Sleep(2000);
                    }

                    position = KeyOperates.MoveToPoint(position, Area[i], territoryType, true, false);
                    PluginLog.Log($"到达点{i} {position.X} {position.Y} {position.Z}");
                    PluginLog.Log($"not work point {i}");
                }
            }
            return true;
        }

        public void StopNormalScript()
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
            {   // 1-稻槎草(lv.68)
                Area = Position.TestArea;
                index = Position.TestIndex;
                indexNum = Position.TestIndexNum3;
                ABC = Position.TestABC;
            }
            else if (area == 2)
            {
                // 2-繁缕(lv.68)
                Area = Position.TestArea;
                index = Position.TestIndex;
                indexNum = Position.TestIndexNum3;
                ABC = Position.TestABC;
            }
            else if (area == 3)
            {
                // 3-棕榈糖浆(lv.82)
                Area = Position.Area3;
                index = Position.Index3;
                indexNum = Position.IndexNum3;
                ABC = Position.ABC3;
            }
            else if (area == 4)
            {
                // 4-葛根(lv.65)
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
            // TODO 5-大蜜蜂的巢(lv.75) 6-巨人新薯(lv.87) 7-山地小麦(lv.73)

            return (Area, index, indexNum, ABC);
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