using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class GatherBot
    {
        private GameData GameData { get; init; }
        private KeyOperates KeyOperates { get; init; }

        private static bool closed = false;

        public GatherBot(GameData GameData)
        {
            this.GameData = GameData;
            KeyOperates = new KeyOperates(GameData);
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
            int[] ABC = Array.Empty<int>();

            (Area, index, ABC) = GetArea(area);

            // TODO 传送

            // 去起始点O 
            Vector3 position = KeyOperates.GetUserPosition(SizeFactor);
            PluginLog.Log($"采集 {position.X} {position.Y} {position.Z}");

            //KeyOperates.KeyMethod(Keys.q_key, 0, true);
            Thread.Sleep(1000);

            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            List< GameObject > gameObjects = new();
            List< int > gameObjectsIndex = new();
            for (int i = 0, j = 0, k = 0 ; i < Area.Length; i++)
            {
                if (closed)
                {
                    PluginLog.Log($"中途结束");
                    return false;
                }

                if (ABC[k] == i && gameObjects.ToArray().Length == 0 && j < index.Length) {
                    (gameObjects, gameObjectsIndex) = Util.GetCanGatherPosition(Area, index, j, SizeFactor);
                    j += 4;
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
                            position = KeyOperates.TestMoveToPoint(position, Area[i], territoryType, true, false);
                            PluginLog.Log($"到达点{i} {position.X} {position.Y} {position.Z}");
                            var targetMgr = DalamudApi.TargetManager;
                            if (go.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.GatheringPoint)
                            {
                                PluginLog.Log($"work: {go.ObjectId}");
                                targetMgr.SetTarget(go);
                            }

                            KeyOperates.KeyMethod(Keys.num0_key);
                            Thread.Sleep(1000);
                            KeyOperates.KeyMethod(Keys.num0_key);
                            Thread.Sleep(1000);
                            KeyOperates.KeyMethod(Keys.num0_key);

                            Thread.Sleep(12000);
                            KeyOperates.KeyMethod(Keys.q_key);
                            Thread.Sleep(2000);

                            if (gameObjects.ToArray().Length > 0) {
                                gameObjects.RemoveAt(0);
                                gameObjectsIndex.RemoveAt(0);
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
                    position = KeyOperates.TestMoveToPoint(position, Area[i], territoryType, true, false);
                    PluginLog.Log($"到达点{i} {position.X} {position.Y} {position.Z}");
                    Thread.Sleep(200);
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

        public (Vector3[], int[], int[]) GetArea(int area) {
            Vector3[] Area = Array.Empty<Vector3>();
            int[] index = Array.Empty<int>();
            int[] ABC = Array.Empty<int>();

            if (area == 1)
            {   // 1-
                Area = Position.TestArea;
                index = Position.TestIndex;
                ABC = Position.TestABC;
            }
            else if (area == 2) { 
                
            }

            return (Area, index, ABC);
        }
    }
}