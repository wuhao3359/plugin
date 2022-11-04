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
        //private GameData GameData { get; init; }
        private KeyOperates KeyOperates { get; init; }

        private CommonBot? CommonBot;

        private GatherBot? GatherBot;

        private static bool closed = false;

        private bool othetRun = false;

        public Dictionary<string, string> param;

        public DailyBot(GameData GameData)
        {

            //this.GameData = GameData;
            KeyOperates = new KeyOperates(GameData);
            CommonBot = new CommonBot(KeyOperates);
            GatherBot = new GatherBot(GameData);
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

        // TODO
        public void DailyScript(string args)
        {
            closed = false;
            try {
                // 参数解析
                param = Util.CommandParse(args);

                uint lv = 50;
                if (param.TryGetValue("level", out var l)) {
                    lv = uint.Parse(l);
                }

                if (param.TryGetValue("duration", out var d))
                {
                    if (d == "1")
                    {
                        // 限时单次采集
                        LimitTimeSinglePlan(lv);
                    }
                    else {
                        // 限时多次采集
                        LimitTimeMultiPlan(lv);
                    }
                }
                else {
                    PluginLog.Error($"check params");
                }
            } catch (Exception ex) {
                PluginLog.Error($"error!!!\n{ex}");
            }
        }

        public void LimitTimeSinglePlan(uint lv)
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
                if (et >= 24) {
                    et = 0;
                    PluginLog.Log($"重置统计, 总共完成 {finishIds.Count}..");
                    finishIds.Clear();
                }
                PluginLog.Log($"start begin et: {et}");
                List<int> ids = LimitMaterials.GetMaterialIdsByEt(et, lv);
                PluginLog.Log($"当前et总共有 {ids.Count}  ..");

                while (ids.Count == 0 && et < 24)
                {
                    PluginLog.Log($"当前et没事干, skip et {et} ..");
                    et++;
                    ids = LimitMaterials.GetMaterialIdsByEt(et, lv);
                }
                if (et >= 24) {
                    continue;
                }

                while (hour != et) {
                    if (closed)
                    {
                        PluginLog.Log($"中途结束");
                        return;
                    }
                    RunWaitTask(lv);
                    while (othetRun) {
                        Time.Update();
                        hour = Time.ServerTime.CurrentEorzeaHour();
                        PluginLog.Log($"当前时间{hour} wait to {et} ..");
                        Thread.Sleep(5000);
                        if (hour == et) {
                           StopWaitTask();
                        }
                    }
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
                    (string Name, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points) = LimitMaterials.GetMaterialById(id);
                    PluginLog.Log($"当前完成任务: {finishIds.Count} 下个任务, id: {id} Name: {Name}, Job: {Job}, et: {et}..");

                    if (hour > et || (et >= 20 && hour < 10)) {
                        PluginLog.Log($"当前et已经结束, finish et {et} ..");
                        break;
                    }

                    if (Tp == 0) {
                        PluginLog.Log($"数据异常, skip {id}..");
                        finishIds.Add(id);
                        continue;
                    }

                    Teleporter.Teleport(Tp);
                    Thread.Sleep(12000);
                    PluginLog.Log($"开始执行任务, id: {id} ");
                    // 切换职业 
                    if (!CommonUi.CurrentJob(Job))
                    {
                        WoAutoCollectionPlugin.Executor.DoGearChange(JobName);
                        Thread.Sleep(500);
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
                        float y = Maths.GetCoordinate(go.Position.Y, SizeFactor) - 5;
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
                            CommonBot.LimitMaterialsMethod(Name);

                            PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                            uint gp = player.CurrentGp;
                            if (gp < player.MaxGp * 0.5)
                            {
                                Thread.Sleep(1000);
                                KeyOperates.KeyMethod(Keys.plus_key);
                                Thread.Sleep(2000);
                            }
                        }
                        KeyOperates.KeyMethod(Keys.up_arrow_key, 200);
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

        public void LimitTimeMultiPlan(uint lv)
        {
            int n = 0;
            bool first = true;
            SeTime Time = new SeTime();

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
                if (et >= 24)
                {
                    et = 0;
                }
                PluginLog.Log($"start begin et: {et}");
                int id = LimitMaterials.GetCollecMaterialIdByEt(et, lv);
                PluginLog.Log($"当前et任务Id {id}  ..");

                while (id == 0 && et < 24)
                {
                    PluginLog.Log($"当前et没事干, skip et {et} ..");
                    et++;
                    id = LimitMaterials.GetCollecMaterialIdByEt(et, lv);
                }
                if (et >= 24)
                {
                    continue;
                }
                
                while (hour != et)
                {
                    if (closed)
                    {
                        PluginLog.Log($"中途结束");
                        return;
                    }
                    RunWaitTask(lv);
                    while (othetRun)
                    {
                        Time.Update();
                        hour = Time.ServerTime.CurrentEorzeaHour();
                        PluginLog.Log($"当前时间{hour} wait to {et} ..");
                        Thread.Sleep(5000);
                        if (hour == et)
                        {
                            StopWaitTask();
                        }
                    }
                }

                (int Id, string Name, int MinEt, int MaxEt, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, int[] CanGatherIndex) = LimitMaterials.GetCollecMaterialById(id);
                Time.Update();
                hour = Time.ServerTime.CurrentEorzeaHour();
                PluginLog.Log($"开始执行任务, id: {id} Name: {Name}, Job: {Job}, MinEt: {MinEt}, MaxEt: {MaxEt}..");
                if (Tp == 0)
                {
                    PluginLog.Log($"数据异常, skip {id}..");
                    break;
                }
                Teleporter.Teleport(Tp);
                Thread.Sleep(12000);
                ushort territoryType = DalamudApi.ClientState.TerritoryType;
                ushort SizeFactor = WoAutoCollectionPlugin.GameData.GetSizeFactor(territoryType);
                // 切换职业 
                if (!CommonUi.CurrentJob(Job))
                {
                    WoAutoCollectionPlugin.Executor.DoGearChange(JobName);
                    Thread.Sleep(500);
                }
                Thread.Sleep(500);
                Vector3 position = MovePositions(Path, true);
                while (hour >= MinEt && hour <= MaxEt) {
                    for (int t = 0; t < Points.Length; t++) {
                        if (closed)
                        {
                            PluginLog.Log($"中途结束");
                            return;
                        }
                        if (Array.IndexOf(CanGatherIndex, t) != -1)
                        {
                            GameObject go = Util.CurrentPositionCanGather(Points[t], SizeFactor);
                            if (go != null)
                            {
                                float x = Maths.GetCoordinate(go.Position.X, SizeFactor);
                                float y = Maths.GetCoordinate(go.Position.Y, SizeFactor) - 5;
                                float z = Maths.GetCoordinate(go.Position.Z, SizeFactor);
                                Vector3 GatherPoint = new(x, y, z);
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
                                        PluginLog.Log($"stopping");
                                        return;
                                    }
                                }

                                KeyOperates.KeyMethod(Keys.down_arrow_key, 200);
                                tt = 0;
                                while (!CommonUi.AddonGatheringIsOpen() && tt < 5)
                                {
                                    KeyOperates.KeyMethod(Keys.num0_key);
                                    Thread.Sleep(300);
                                    if (closed)
                                    {
                                        PluginLog.Log($"stopping");
                                        return;
                                    }
                                    tt++;
                                }
                                if (tt >= 5)
                                {
                                    PluginLog.Log($"未打开采集面板, skip {id}..");
                                    continue;
                                }
                                Thread.Sleep(1000);

                                if (CommonUi.AddonGatheringIsOpen())
                                {
                                    CommonBot.LimitMultiMaterialsMethod(Name);
                                    
                                    PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                                    uint gp = player.CurrentGp;
                                    if (gp < player.MaxGp * 0.6)
                                    {
                                        Thread.Sleep(1000);
                                        KeyOperates.KeyMethod(Keys.plus_key);
                                        Thread.Sleep(2000);
                                    }
                                }
                                KeyOperates.KeyMethod(Keys.up_arrow_key, 200);
                            }
                            else
                            {
                                PluginLog.Log($"未知原因未找到数据, skip {id}..");
                                Thread.Sleep(1000);
                                continue;
                            }
                            Time.Update();
                            hour = Time.ServerTime.CurrentEorzeaHour();
                            Thread.Sleep(1000);
                        }
                    }
                }
                PluginLog.Log($"当前ET结束...");
            }
        }

        private void RunWaitTask(uint lv) {
            othetRun = true;
            Task task = new(() =>
            {
                PluginLog.Log($"执行等待任务...");
                try
                {
                    GatherBot.param = param;
                    GatherBot.RunNormalScript(0, lv);
                }
                catch (Exception e)
                {
                    PluginLog.Error($"其他任务, error!!!\n{e}");
                }
                PluginLog.Log($"其他任务结束...");
                othetRun = false;
            });
            task.Start();
        }

        private void StopWaitTask()
        {
            GatherBot.StopScript();
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

    }
}