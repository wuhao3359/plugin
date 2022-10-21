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
using WoAutoCollectionPlugin.UseAction;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class DailyBot
    {
        private KeyOperates KeyOperates { get; init; }

        private CommonBot? CommonBot;

        private GatherBot? GatherBot;

        private static bool closed = false;

        private string currentJob = "";

        private bool othetRun = false;

        public DailyBot()
        {
            KeyOperates = WoAutoCollectionPlugin.KeyOperates;
            GatherBot = WoAutoCollectionPlugin.GatherBot;
        }

        public void Init()
        {
            closed = false;
            CommonBot.Init();
        }

        public void StopScript()
        {
            closed = true;
            CommonBot.StopScript();
            GatherBot.StopScript();
        }

        // TODO 日常使用
        public void DailyScript(string args)
        {
            closed = false;
            try {
                string[] str = args.Split(' ');
                PluginLog.Log($"daily: {args} length: {args.Length}");

                uint lv = 80;
                if (str.Length >= 1) {
                    lv = uint.Parse(str[0]);
                }
                if (lv < 80) {
                    lv = 80;
                }
                TimePlan(lv);

            } catch (Exception ex) {
                PluginLog.Error($"error!!!\n{ex}");
            }
        }

        public void TimePlan(uint lv)
        {
            int n = 0;
            bool first = true;
            SeTime Time = new SeTime();
            // 每24个et内单个任务只允许被执行一遍
            List<int> finishIds = new();

            Time.Update();
            int hour = Time.ServerTime.CurrentEorzeaHour();
            int minute = Time.ServerTime.CurrentEorzeaMinute();
            int et = hour;
            while (!closed && n < 1000)
            {
                if (first)
                {
                    et--;
                    first = false;
                }

                et++;
                if (et>= 24 || et == 0) {
                    PluginLog.Log($"重置统计, 总共完成 {finishIds.Count}..");
                    finishIds.Clear();
                }
                PluginLog.Log($"start begin et: {et}");
                List<int> ids = LimitMaterials.GetMaterialIdsByEt(et, lv);
                PluginLog.Log($"当前et总共有 {ids.Count}  ..");

                while (ids.Count == 0) {
                    PluginLog.Log($"当前et没事干, skip et {et} ..");
                    et++;
                    if(et >= 24) {
                        et = 0;
                        finishIds.Clear();
                        PluginLog.Log($"重置统计, 总共完成 {finishIds.Count}..");
                        while (hour > et) {
                            if (closed)
                            {
                                PluginLog.Log($"中途结束");
                                return;
                            }
                            PluginLog.Log($"当前时间{hour} wait to {et} ..");
                            Thread.Sleep(10000);
                            Time.Update();
                            hour = Time.ServerTime.CurrentEorzeaHour();
                        }
                    }
                    ids = LimitMaterials.GetMaterialIdsByEt(et, lv);
                }

                int num = 0;
                foreach (int id in ids) {
                    if (closed)
                    {
                        PluginLog.Log($"中途结束");
                        return;
                    }
                    if (finishIds.Exists(t => t == id))
                    {
                        PluginLog.Log($"该任务当前周期已被执行, skip {id}..");
                        continue;
                    }
                    (string Names, string Job, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points) = LimitMaterials.GetMaterialById(id);
                    PluginLog.Log($"当前完成: {finishIds.Count} 下个任务, id: {id} Name: {Names}, Job: {Job}, et: {et}..");

                    //if (EvaluateTask(Names, Job)) {
                    //    PluginLog.Log($"预计收益低, 丢弃, id: {id} Name: {Names}..");
                    //    finishIds.Add(id);
                    //    continue;
                    //}

                    if (hour > et || (et >= 20 && hour < 10)) {
                        PluginLog.Log($"当前et已经结束, finish et {et} ..");
                        break;
                    }

                    if (Tp == 0) {
                        PluginLog.Log($"数据异常, skip {id}..");
                        finishIds.Add(id);
                        continue;
                    }

                    while (hour < et || othetRun)
                    {
                        if (closed)
                        {
                            PluginLog.Log($"中途结束");
                            return;
                        }
                        if (othetRun)
                        {
                            PluginLog.Log($"正在执行其他任务...");
                            if (hour >= et) { 
                                GatherBot.StopScript();
                            }
                        }
                        else {
                            PluginLog.Log($"当前et: {hour}, 未到时间, 等待执行任务, wait et {et}..");
                            WaitTask();
                        }
                        Thread.Sleep(10000);
                        Time.Update();
                        hour = Time.ServerTime.CurrentEorzeaHour();
                    }

                    Teleporter.Teleport(Tp);
                    Thread.Sleep(12000);
                    PluginLog.Log($"开始执行任务, id: {id} ");
                    // 切换职业 
                    if (currentJob != Job) {
                        WoAutoCollectionPlugin.Executor.DoGearChange(Job);
                        currentJob = Job;
                    }
                    Thread.Sleep(1000);
                    Vector3 position = MovePositions(Path, true);
                    // 找最近的采集点
                    ushort territoryType = DalamudApi.ClientState.TerritoryType;
                    ushort SizeFactor = WoAutoCollectionPlugin.GameData.GetSizeFactor(territoryType);
                    (GameObject go, Vector3 point) = Util.LimitTimePosCanGather(Points, SizeFactor);

                    if (go != null)
                    {
                        float x = Maths.GetCoordinate(go.Position.X, SizeFactor);
                        float y = Maths.GetCoordinate(go.Position.Y, SizeFactor);
                        float z = Maths.GetCoordinate(go.Position.Z, SizeFactor);
                        Vector3 GatherPoint = new(x, y, z);
                        position = KeyOperates.MoveToPoint(position, point, territoryType, true, false);
                        position = KeyOperates.MoveToPoint(position, GatherPoint, territoryType, false, false);

                        var targetMgr = DalamudApi.TargetManager;
                        targetMgr.SetTarget(go);

                        int tt = 0;
                        while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 7)
                        {
                            if (tt >= 3)
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

                        PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                        uint gp = player.CurrentGp;
                        if (gp < player.MaxGp * 0.45)
                        {
                            KeyOperates.KeyMethod(Keys.plus_key);
                            Thread.Sleep(2000);
                        }

                        KeyOperates.KeyMethod(Keys.down_arrow_key, 200);
                        tt = 0;
                        while (!CommonUi.AddonGatheringIsOpen() && tt < 5)
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
                        if (tt >= 5)
                        {
                            PluginLog.Log($"未打开采集面板, skip {id}..");
                            finishIds.Add(id);
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
                        finishIds.Add(id);
                        Thread.Sleep(1000);
                        continue;
                    }
                    // finish work
                    finishIds.Add(id);
                    num++;
                    Time.Update();
                    hour = Time.ServerTime.CurrentEorzeaHour();
                    Thread.Sleep(1000);

                    // TODO 修理装备
                    // TODO魔晶石精制
                }
                PluginLog.Log($"当前et: {et}, 总共{ids.Count}, 成功执行{num}个任务..");
            }
        }

        private void WaitTask() {
            othetRun = true;
            Task task = new(() =>
            {
                //GatherBot.RunNormalScript(1);
                Thread.Sleep(20000);
                PluginLog.Log($"其他任务结束...");
                othetRun = false;
            });
            task.Start();
        }

        private Vector3 MovePositions(Vector3[] Path, bool UseMount)
        {
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            ushort SizeFactor = WoAutoCollectionPlugin.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
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
            }
            return position;
        }

        private bool EvaluateTask(string Name, string Job) {
            uint GivingLandActionId = 4589;
            if (Job == "园艺工")
            {
                GivingLandActionId = 4590;
            }
            bool r = false;
            if (Name.Contains("雷之") || Name.Contains("火之") || Name.Contains("风之") || Name.Contains("水之") || Name.Contains("冰之") || Name.Contains("土之"))
            {
                PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                uint gp = player.CurrentGp;
                int level = player.Level;
                if (level < 74 || Game.GetSpellActionRecastTimeElapsed(GivingLandActionId) != 0 || gp < 200)
                {
                    r = true;
                }
            }
            return r;
        }
    }
}