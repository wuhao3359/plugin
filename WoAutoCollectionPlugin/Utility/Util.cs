using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using WoAutoCollectionPlugin.Utility;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace WoAutoCollectionPlugin
{
    public static unsafe class Util
    {
        public static (List<GameObject>, List<int>) GetCanGatherPosition(Vector3[] Points, int[] CanCollectPoint, int j, int UnknownPoints, ushort SizeFactor)
        {
            List<GameObject> gameObjects = new();
            List<int> gameObjectsIndex = new();
            for (int i = 0; i < UnknownPoints; i++, j++)
            {
                GameObject go = CurrentPositionCanGather(Points[CanCollectPoint[j]], SizeFactor);
                if (go != null) {
                    gameObjects.Add(go);
                    gameObjectsIndex.Add(CanCollectPoint[j]);
                }
                if (gameObjects.ToArray().Length >= 2) {
                    break;
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
                if (gameObject != null && gameObject.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.GatheringPoint)
                {
                    Vector3 v = new(Maths.GetCoordinate(gameObject.Position.X, SizeFactor), Maths.GetCoordinate(gameObject.Position.Y, SizeFactor), Maths.GetCoordinate(gameObject.Position.Z, SizeFactor));
                    double d = Maths.Distance(position, v);
                    if (d < distance)
                    {
                        distance = d;
                        nearestGo = gameObject;
                        index = i;
                    }
                }
            }

            if (nearestGo != null && CanGather(nearestGo))
            {
                PluginLog.Log($"最近1: {index}");
                return nearestGo;
            }
            else {
                PluginLog.Log($"没有找到最近的point1");
                return null;
            }
        }

        public static GameObject CurrentYPositionCanGather(Vector3 position, ushort SizeFactor)
        {
            GameObject nearestGo = null;
            double distance = 1000000000f;
            int index = 0;
            int length = DalamudApi.ObjectTable.Length;
            for (int i = 0; i < length; i++)
            {
                GameObject? gameObject = DalamudApi.ObjectTable[i];
                if (gameObject != null && gameObject.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.GatheringPoint && CanGather(gameObject))
                {
                    Vector3 v = new(Maths.GetCoordinate(gameObject.Position.X, SizeFactor), Maths.GetCoordinate(gameObject.Position.Y, SizeFactor), Maths.GetCoordinate(gameObject.Position.Z, SizeFactor));
                    double d = Maths.Distance(position, v);
                    if (d < distance)
                    {
                        distance = d;
                        nearestGo = gameObject;
                        index = i;
                    }
                }
            }

            if (nearestGo != null && CanGather(nearestGo))
            {
                PluginLog.Log($"最近2: {index}");
                return nearestGo;
            }
            else
            {
                PluginLog.Log($"没有找到最近的point2");
                return null;
            }
        }

        public static (GameObject, Vector3) LimitTimePosCanGather(Vector3[] positions, ushort SizeFactor)
        {
            GameObject nearestGo = null;
            double distance = 1000000000f;
            Vector3 position = positions[0];
            int length = DalamudApi.ObjectTable.Length;
            for (int i = 0; i < length; i++)
            {
                GameObject? gameObject = DalamudApi.ObjectTable[i];
                if (gameObject != null && CanGather(gameObject))
                {
                    if (gameObject.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.GatheringPoint)
                    {
                        if (gameObject.Name.ToString() == "未知的良材" || gameObject.Name.ToString() == "未知的草场"
                            || gameObject.Name.ToString() == "未知的矿脉" || gameObject.Name.ToString() == "未知的石场"
                            || gameObject.Name.ToString() == "传说的良材" || gameObject.Name.ToString() == "传说的草场"
                            || gameObject.Name.ToString() == "传说的矿脉" || gameObject.Name.ToString() == "传说的石场") {
                            Vector3 v = new(Maths.GetCoordinate(gameObject.Position.X, SizeFactor), Maths.GetCoordinate(gameObject.Position.Y, SizeFactor), Maths.GetCoordinate(gameObject.Position.Z, SizeFactor));
                            double d = 100000f;
                            foreach (Vector3 pos in positions)
                            {
                                d = Maths.Distance(pos, v);
                                if (d < distance)
                                {
                                    distance = d;
                                    nearestGo = gameObject;
                                    position = pos;
                                }
                            }
                        }
                    }
                }
            }

            if (nearestGo != null)
            {
                //PluginLog.Log($"最近, {nearestGo.DataId}");
                return (nearestGo, position);
            }
            else
            {
                PluginLog.Log($"没有找到最近的point3");
                return (null, position);
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

        public static void GenerateCallback(AtkUnitBase* unitBase, params object[] values)
        {
            var atkValues = CreateAtkValueArray(values);
            if (atkValues == null) return;
            try
            {
                unitBase->FireCallback(values.Length, atkValues);
            }
            finally
            {
                for (var i = 0; i < values.Length; i++)
                {
                    if (atkValues[i].Type == ValueType.String)
                    {
                        Marshal.FreeHGlobal(new IntPtr(atkValues[i].String));
                    }
                }
                Marshal.FreeHGlobal(new IntPtr(atkValues));
            }
        }

        public static AtkValue* CreateAtkValueArray(params object[] values)
        {
            var atkValues = (AtkValue*)Marshal.AllocHGlobal(values.Length * sizeof(AtkValue));
            if (atkValues == null) return null;
            try
            {
                for (var i = 0; i < values.Length; i++)
                {
                    var v = values[i];
                    switch (v)
                    {
                        case uint uintValue:
                            atkValues[i].Type = ValueType.UInt;
                            atkValues[i].UInt = uintValue;
                            break;
                        case int intValue:
                            atkValues[i].Type = ValueType.Int;
                            atkValues[i].Int = intValue;
                            break;
                        case float floatValue:
                            atkValues[i].Type = ValueType.Float;
                            atkValues[i].Float = floatValue;
                            break;
                        case bool boolValue:
                            atkValues[i].Type = ValueType.Bool;
                            atkValues[i].Byte = (byte)(boolValue ? 1 : 0);
                            break;
                        case string stringValue:
                            {
                                atkValues[i].Type = ValueType.String;
                                var stringBytes = System.Text.Encoding.UTF8.GetBytes(stringValue);
                                var stringAlloc = Marshal.AllocHGlobal(stringBytes.Length + 1);
                                Marshal.Copy(stringBytes, 0, stringAlloc, stringBytes.Length);
                                Marshal.WriteByte(stringAlloc, stringBytes.Length, 0);
                                atkValues[i].String = (byte*)stringAlloc;
                                break;
                            }
                        default:
                            throw new ArgumentException($"Unable to convert type {v.GetType()} to AtkValue");
                    }
                }
            }
            catch
            {
                return null;
            }

            return atkValues;
        }

        internal static bool TryParseRetainerName(string s, out string retainer)
        {
            retainer = default;
            //if (!P.retainerManager.Ready)
            //{
            //    return false;
            //}
            //for (var i = 0; i <= 2; i++)
            //{
            //    var r = P.retainerManager.Retainer(i);
            //    var rname = r.Name.ToString();
            //    if (s.Contains(rname) && (retainer == null || rname.Length > retainer.Length))
            //    {
            //        retainer = rname;
            //    }
            //}
            //return retainer != default;
            return false;
        }

        public static Dictionary<string, string> CommandParse(string command, string args) {
            string[] str = args.Split(" ");
            PluginLog.Log($"command: {command}, args: {args}, length: {str.Length}");

            Dictionary<string, string> dictionary = new();
            if (str.Length > 0) {
                CommandParams.TryGetValue(command, out var list);

                for (int i = 0; i < str.Length; i++)
                {
                    string s = str[i];
                    string[] ss = s.Split(":");
                    if (ss.Length == 2)
                    {
                        dictionary.Add(ss[0], ss[1]);
                    }
                }
                foreach (string s in list)
                {
                    if (!dictionary.TryGetValue(s, out var v))
                    {
                        DefaultValues.TryGetValue(s, out var dv);
                        dictionary.Add(s, dv);
                    };
                }
            }
            return dictionary;
        }

        /*
         * 命令解析
         * params   
         *  command:daily    主要用途(Daily-限时采集)
         *  duration:1       持续次数(1次或多次)
         *  level:0-50       等级区间(0到50)
         *  bagLimit:1       背包限制(1-有)
         *  example:    daily duration:1 level:0-50 bagLimit:1
         *  
         *  
         *  command:craft           主要用途 自动生产
         *  pressKey:1              宏按键
         *  type:1                  1-普通制作 2-收藏品制作 3-快速制作 4-重建制作
         *  recipeName:上级以太药   生产物品名称
         *  exchangeItem:1          交换物品id 收藏品专业
         *  hq:1                    hq物品index
         *  example:    craft pressKey:1 type:1 recipeName:上级以太药 exchangeItem:1
         *  
         *  
         *  command:collectionfish  主要用途 钓鱼获取白票 紫票 灵砂
         *  type:1                  1-紫票 2-白票 3-晓月灵砂 4-巨海灵砂
         *  exchangeItem:1          交换物品id 紫票 白票 兑换的物品
         *  example:    collectionfish type:2 exchangeItem:2
         */
        public static Dictionary<string, List<string>> CommandParams = new() {
            { "daily", new() { "duration", "level", "bagLimit", "otherTask", "repair", "extractMateria" } },
            { "craft", new() { "pressKey", "type", "recipeName", "hq", "exchangeItem", "repair", "extractMateria" } },
            { "collectionfish", new() { "ftype", "fexchangeItem"} }
        };

        public static Dictionary<string, string> DefaultValues = new()
        {
            // daily
            { "duration", "1" },
            { "level", "50" },
            { "bagLimit", "1" },
            { "otherTask", "0" },

            // craft
            { "pressKey", "1" },
            { "type", "1" },
            { "recipeName", "" },
            { "hq", "0-0" },
            { "exchangeItem", "0" },

            // fish
            { "ftype", "2" },
            { "fexchangeItem", "2" },

            // common
            { "repair", "0" },
            { "extractMateria", "0" },
        };

        public static (int, int) LevelSplit(string lv) {
            int lv0 = 0;
            int lv1;
            string[] lvstr = lv.Split("-");
            if (lvstr.Length > 1)
            {
                lv0 = int.Parse(lvstr[0]);
                lv1 = int.Parse(lvstr[1]);
            }
            else
            {
                lv1 = int.Parse(lvstr[0]);
            }
            return (lv0, lv1);
        }
    }
}
