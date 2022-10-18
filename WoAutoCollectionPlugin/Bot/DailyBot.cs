using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using WoAutoCollectionPlugin.SeFunctions;
using WoAutoCollectionPlugin.Time;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class DailyBot
    {
        private GameData GameData { get; init; }
        private KeyOperates KeyOperates { get; init; }

        private CommonBot? CommonBot;
        private CraftBot? CraftBot;

        private static bool closed = false;

        // 0-默认
        private int current = 0;
        private int next = 0;
        public DailyBot(GameData GameData)
        {
            KeyOperates = new KeyOperates(GameData);
            CraftBot = new CraftBot(GameData);
            CommonBot = new CommonBot(KeyOperates);
            this.GameData = GameData;
        }

        public void Init()
        {
            closed = false;
            CraftBot.Init();
            CommonBot.Init();
        }

        public void StopScript()
        {
            closed = true;
            CraftBot.StopScript();
            CommonBot.StopScript();
        }

        // TODO 日常使用
        public void DailyScript()
        {
            closed = false;
            TimePlan();
        }

        public void TimePlan()
        {
            int n = 0;
            SeTime Time = new SeTime();
            while (!closed && n < 8000)
            {
                // 每24个et内单个任务只允许被执行一遍
                List<int> finishIds = new List<int>();
                // 判断时间段 不同时间干不同事情
                Time.Update();
                int hour = Time.ServerTime.CurrentEorzeaHour();
                int minute = Time.ServerTime.CurrentEorzeaMinute();
                PluginLog.Log($"{hour} {minute}");

                int et = hour + 1;
                if (et == 24) {
                    PluginLog.Log($"重置统计, 总共完成 {finishIds.Count}..");
                    finishIds.Clear();
                }
                PluginLog.Log($"start begin {et}");
                List<int> ids = LimitMaterials.GetMaterialIdsByEt(et);

                while (ids.Count == 0) {
                    PluginLog.Log($"当前et没事干, skip et {et} ..");
                    et++;
                    ids = LimitMaterials.GetMaterialIdsByEt(et);
                }
                
                // TODO 修理装备

                int num = 0;
                foreach (int id in ids) {
                    if (finishIds.Exists(t => t == id)) {
                        PluginLog.Log($"该任务当前周期已被执行, skip {id}..");
                        Thread.Sleep(3000);
                        continue;
                    }
                    (string Names, string Job, uint tp, Vector3[] path, Vector3[] points) = LimitMaterials.GetMaterialById(id);

                    if (hour > et) {
                        PluginLog.Log($"当前et已经结束, finish et {et} ..");
                        Thread.Sleep(3000);
                        break;
                    }

                    if (tp == 0) {
                        PluginLog.Log($"数据异常, skip {id}..");
                        Thread.Sleep(3);
                        continue;
                    }
                    Teleporter.Teleport(tp);
                    Thread.Sleep(15000);

                    while (hour < et)
                    {
                        PluginLog.Log($"未到时间, 等待执行任务, wait {et}..");
                        Thread.Sleep(10000);
                        Time.Update();
                        hour = Time.ServerTime.CurrentEorzeaHour();
                    }
                    // 切换职业 TODO
                    Thread.Sleep(1000);
                    Vector3 position = MovePositions(path, true);
                    // 找最近的采集点
                    ushort territoryType = DalamudApi.ClientState.TerritoryType;
                    ushort SizeFactor = GameData.GetSizeFactor(territoryType);
                    (GameObject go, Vector3 point) = Util.LimitTimePosCanGather(points, SizeFactor);

                    if (go != null)
                    {
                        float x = Maths.GetCoordinate(go.Position.X, SizeFactor);
                        float y = Maths.GetCoordinate(go.Position.Y, SizeFactor);
                        float z = Maths.GetCoordinate(go.Position.Z, SizeFactor);
                        Vector3 GatherPoint = new Vector3(x, y, z);
                        position = KeyOperates.MoveToPoint(position, point, territoryType, true, false);
                        position = KeyOperates.MoveToPoint(position, GatherPoint, territoryType, false, false);

                        var targetMgr = DalamudApi.TargetManager;
                        targetMgr.SetTarget(go);

                        int tt = 0;
                        while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 7)
                        {
                            if (tt >= 2)
                            {
                                KeyOperates.KeyMethod(Keys.w_key, 200);
                            }
                            KeyOperates.KeyMethod(Keys.q_key);
                            Thread.Sleep(1000);
                            tt++;

                            if (closed)
                            {
                                PluginLog.Log($"dailyTask stopping");
                                return;
                            }
                        }

                        tt = 0;
                        while (!CommonUi.AddonGatheringIsOpen() && tt < 7)
                        {
                            KeyOperates.KeyMethod(Keys.num0_key);
                            Thread.Sleep(500);
                            if (closed)
                            {
                                PluginLog.Log($"dailyTask stopping");
                                return;
                            }
                            tt++;
                        }
                        if (tt >= 7)
                        {
                            PluginLog.Log($"未打开采集面板, skip {id}..");
                            Thread.Sleep(3000);
                            continue;
                        }
                        Thread.Sleep(1000);
                        if (CommonUi.AddonGatheringIsOpen())
                        {
                            CommonBot.LimitMaterialsMethod(Names, Job);
                        }
                    }
                    else {
                        PluginLog.Log($"未知原因未找到数据, skip {id}..");
                        Thread.Sleep(3000);
                        continue;
                    }
                    // finish work
                    finishIds.Add(id);
                    num++;
                    Time.Update();
                    hour = Time.ServerTime.CurrentEorzeaHour();
                    Thread.Sleep(3000);

                    // TODO魔晶石精制
                }
                PluginLog.Log($"当前et成功执行{num}个任务..");
            }
        }

        private Vector3 MovePositions(Vector3[] Path, bool UseMount)
        {
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            Vector3 position = KeyOperates.GetUserPosition(SizeFactor);
            for (int i = 0; i < Path.Length; i++)
            {
                if (closed)
                {
                    PluginLog.Log($"中途结束");
                    return KeyOperates.GetUserPosition(SizeFactor);
                }
                position = KeyOperates.MoveToPoint(position, Path[i], territoryType, UseMount, false);
                PluginLog.Log($"到达点{i} {position.X} {position.Y} {position.Z}");
                Thread.Sleep(800);
            }
            return position;
        }
    }
}