using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace WoAutoCollectionPlugin.Utility
{
    public static unsafe class Util
    {
        public static (List<GameObject>, List<int>) GetCanGatherPosition(Vector3[] Area, int[] index, int j, ushort SizeFactor)
        {
            List<GameObject> gameObjects = new();
            List<int> gameObjectsIndex = new();
            for (int i = 0; i < 4; i++, j++)
            {
                GameObject go = CurrentPositionCanGather(Area[index[j]], SizeFactor);
                if (go != null ) {
                    gameObjects.Add(go);
                    gameObjectsIndex.Add(index[j]);
                }
            }
            return (gameObjects, gameObjectsIndex);
        }

        public static GameObject CurrentPositionCanGather(Vector3 position, ushort SizeFactor)
        {
            GameObject nearestGo = null;
            double distance = 1000000000f;
            int index = 0;
            int length = DalamudApi.ObjectTable.Length;
            for (int i = 0; i < length; i++)
            {
                GameObject? gameObject = DalamudApi.ObjectTable[i];
                if (gameObject != null && CanGather(gameObject))
                {
                    if (gameObject.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.GatheringPoint)
                    {
                        Vector3 v = new(Maths.GetCoordinate(gameObject.Position.X, SizeFactor), Maths.GetCoordinate(gameObject.Position.Y, SizeFactor), Maths.GetCoordinate(gameObject.Position.Z, SizeFactor));
                        double d = Maths.Distance(position, v);
                        if (d < distance) {
                            distance = d;
                            nearestGo = gameObject;
                            index = i;
                        }
                    }
                }
            }

            if (nearestGo != null)
            {
                PluginLog.Log($"最近 {index}, {nearestGo.DataId}");
                return nearestGo;
            }
            else {
                PluginLog.Log($"没有找到最近的point");
                return null;
            }
        }

        public static bool CanGather(GameObject go) {
            FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject* obj = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)go.Address;
            if (obj->RenderFlags == 0) { 
                return true;
            }
            return false;
        }

        public static void SetTarget(string targetName) {
            var target = DalamudApi.ObjectTable.FirstOrDefault(obj => obj.Name.TextValue.ToLowerInvariant() == targetName);

            if (target == default)
                PluginLog.Error("Could not find target");

            DalamudApi.TargetManager.SetTarget(target);
        }
    }
}
