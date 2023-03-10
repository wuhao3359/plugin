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
        private bool closed = false;

        private bool othetRun = false;

        private bool needTp = true;

        private string otherTaskParam = "0";

        int hour = 0;

        int et = 0;

        public DailyBot()
        {}

        public void Init()
        {
            closed = false;
            Teleporter.count = 0;
            WoAutoCollectionPlugin.GameData.CraftBot.Init();
            WoAutoCollectionPlugin.GameData.CommonBot.Init();
        }

        public void StopScript()
        {
            closed = true;
            Teleporter.count = 0;
            WoAutoCollectionPlugin.GameData.CommonBot.StopScript();
            WoAutoCollectionPlugin.GameData.GatherBot.StopScript();
            WoAutoCollectionPlugin.GameData.CollectionFishBot.StopScript();
        }

        public void DailyScript(string args)
        {
            Init();
            closed = false;
            try {
                // 参数解析
                string command = "daily";
                WoAutoCollectionPlugin.GameData.param = Util.CommandParse(command, args);

                if (WoAutoCollectionPlugin.GameData.param.TryGetValue("otherTask", out var ot))
                {
                    otherTaskParam = ot;
                }

                string lv = "50";
                if (WoAutoCollectionPlugin.GameData.param.TryGetValue("level", out var l)) {
                    lv = l;
                }

                if (WoAutoCollectionPlugin.GameData.param.TryGetValue("duration", out var d))
                {
                    PluginLog.Log($"d: {d} ss[1]: {d}");
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

        public void LimitTimeSinglePlan(string lv)
        {
            int n = 0;
            
            // 每24个et内单个任务只允许被执行一遍
            List<int> finishIds = new();

            WoAutoCollectionPlugin.Time.Update();
            hour = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaHour();
            et = hour;
            while (!closed && n < 1000)
            {
                if (et >= 24) {
                    et = 0;
                    PluginLog.Log($"重置统计, 总共完成 {finishIds.Count}..");
                    finishIds.Clear();
                }
                PluginLog.Log($"start begin et: {et}");
                List<int> ids = LimitMaterials.GetMaterialIdsByEtAndFinishId(et, lv, finishIds);
                PluginLog.Log($"当前et总共有 {ids.Count}  ..");

                while (ids.Count == 0 && et < 24)
                {
                    PluginLog.Log($"当前et没事干, skip et {et} ..");
                    et++;
                    ids = LimitMaterials.GetMaterialIdsByEtAndFinishId(et, lv, finishIds);
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

                    if (otherTaskParam != "1" && needTp)
                    {
                        foreach (int id in ids)
                        {
                            (string Name, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points) = LimitMaterials.GetMaterialById(id);
                            Teleporter.Teleport(Tp);
                            needTp = false;
                            break;
                        }
                    }

                    RunWaitTask(lv);
                    while (othetRun) {
                        WoAutoCollectionPlugin.Time.Update();
                        hour = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaHour();
                        if (closed)
                        {
                            PluginLog.Log($"中途结束");
                            return;
                        }
                        PluginLog.Log($"当前时间{hour} wait to {et} ..");
                        Thread.Sleep(7000);
                        if (hour == et) {
                           StopWaitTask();
                        }
                    }
                    WoAutoCollectionPlugin.Time.Update();
                    hour = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaHour();
                    PluginLog.Log($"当前时间{hour} wait to {et} ..");
                }

                int num = 0;
                foreach (int id in ids) {
                    if (closed)
                    {
                        PluginLog.Log($"中途结束");
                        return;
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

                    if (needTp)
                    {
                        Teleporter.Teleport(Tp);
                    }
                    else {
                        needTp = true;
                    }
                    WoAutoCollectionPlugin.GameData.CommonBot.UseItem();

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
                        float y = go.Position.Y - 5;
                        float z = Maths.GetCoordinate(go.Position.Z, SizeFactor);
                        Vector3 GatherPoint = new(x, y, z);
                        position = WoAutoCollectionPlugin.GameData.KeyOperates.MoveToPoint(position, point, territoryType, true, false);
                        position = WoAutoCollectionPlugin.GameData.KeyOperates.MoveToPoint(position, GatherPoint, territoryType, false, false);

                        var targetMgr = DalamudApi.TargetManager;
                        targetMgr.SetTarget(go);

                        int tt = 0;
                        while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 7)
                        {
                            if (tt >= 3)
                            {
                                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.w_key, 200);
                            }
                            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.q_key);
                            Thread.Sleep(1000);
                            tt++;

                            if (closed)
                            {
                                PluginLog.Log($"dailyTask stopping");
                                return;
                            }
                        }

                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.down_arrow_key, 200);
                        tt = 0;
                        while (!CommonUi.AddonGatheringIsOpen() && tt < 5)
                        {
                            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
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
                            WoAutoCollectionPlugin.GameData.CommonBot.LimitMaterialsMethod(Name);
                        }
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.up_arrow_key, 200);
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
                    WoAutoCollectionPlugin.Time.Update();
                    hour = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaHour();
                    Thread.Sleep(2000);

                    int count = CommonUi.CanExtractMateria();
                    if (count >= 5)
                    {
                        WoAutoCollectionPlugin.GameData.CommonBot.ExtractMateria(count);
                    }
                    if (CommonUi.NeedsRepair())
                    {
                        Teleporter.Teleport(Positions.ShopTp);
                        MovePositions(Positions.RepairNPC, false);
                        WoAutoCollectionPlugin.GameData.CommonBot.NpcRepair("阿塔帕");
                    }
                }
                PluginLog.Log($"当前et: {et}, 总共{ids.Count}, 成功执行{num}个任务..");
                et++;
            }
        }

        public void LimitTimeMultiPlan(string lv)
        {
            int n = 0;
            bool first = true;
            WoAutoCollectionPlugin.Time.Update();
            int hour = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaHour();
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
                        WoAutoCollectionPlugin.Time.Update();
                        hour = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaHour();
                        PluginLog.Log($"当前时间{hour} wait to {et} ..");
                        Thread.Sleep(5000);
                        if (hour == et)
                        {
                            StopWaitTask();
                        }
                    }
                }

                (int Id, string Name, int MinEt, int MaxEt, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, int[] CanGatherIndex) = LimitMaterials.GetCollecMaterialById(id);
                WoAutoCollectionPlugin.Time.Update();
                hour = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaHour();
                PluginLog.Log($"开始执行任务, id: {id} Name: {Name}, Job: {Job}, MinEt: {MinEt}, MaxEt: {MaxEt}..");
                if (Tp == 0)
                {
                    PluginLog.Log($"数据异常, skip {id}..");
                    break;
                }
                Teleporter.Teleport(Tp);
                
                WoAutoCollectionPlugin.GameData.CommonBot.UseItem();
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
                    WoAutoCollectionPlugin.Time.Update();
                    hour = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaHour();
                    int minute = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaMinute();
                    if (hour == MaxEt && minute >= 45)
                    {
                        PluginLog.Log($"时间不够...");
                        break;
                    }
                    for (int t = 0; t < Points.Length && hour >= MinEt && hour <= MaxEt; t++) {
                        WoAutoCollectionPlugin.Time.Update();
                        hour = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaHour();
                        minute = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaMinute();
                        if (hour == MaxEt && minute >= 45) {
                            PluginLog.Log($"时间不够...");
                            break;
                        }
                        if (closed)
                        {
                            PluginLog.Log($"中途结束");
                            return;
                        }
                        WoAutoCollectionPlugin.GameData.KeyOperates.MoveToPoint(position, Points[t], territoryType, true, false);
                        if (Array.IndexOf(CanGatherIndex, t) != -1)
                        {
                            GameObject go = Util.CurrentPositionCanGather(Points[t], SizeFactor);
                            if (go != null)
                            {
                                float x = Maths.GetCoordinate(go.Position.X, SizeFactor);
                                float y = go.Position.Y;
                                float z = Maths.GetCoordinate(go.Position.Z, SizeFactor);
                                Vector3 GatherPoint = new(x, y, z);
                                position = WoAutoCollectionPlugin.GameData.KeyOperates.MoveToPoint(position, GatherPoint, territoryType, false, false);

                                WoAutoCollectionPlugin.Time.Update();
                                hour = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaHour();
                                if (!(hour >= MinEt && hour <= MaxEt)) {
                                    PluginLog.Log($"时间结束...");
                                    break;
                                }

                                var targetMgr = DalamudApi.TargetManager;
                                targetMgr.SetTarget(go);

                                int tt = 0;
                                while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 7)
                                {
                                    if (tt >= 3)
                                    {
                                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.w_key, 200);
                                    }
                                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.q_key);
                                    Thread.Sleep(800);
                                    tt++;
                                }

                                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.down_arrow_key, 200);
                                tt = 0;
                                while (!CommonUi.AddonGatheringIsOpen() && tt < 4)
                                {
                                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                                    Thread.Sleep(300);
                                    if (closed)
                                    {
                                        PluginLog.Log($"stopping");
                                        return;
                                    }
                                    tt++;
                                }
                                if (tt >= 4)
                                {
                                    PluginLog.Log($"未打开采集面板, skip {id}..");
                                    continue;
                                }
                                Thread.Sleep(1000);

                                if (CommonUi.AddonGatheringIsOpen())
                                {
                                    WoAutoCollectionPlugin.GameData.CommonBot.LimitMultiMaterialsMethod(Name);
                                    WoAutoCollectionPlugin.GameData.CommonBot.UseItem(0.65);
                                }
                                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.up_arrow_key, 150);
                            }
                            else
                            {
                                PluginLog.Log($"未知原因未找到数据, skip {id}..");
                                t++;
                                Thread.Sleep(1000);
                                continue;
                            }
                            WoAutoCollectionPlugin.Time.Update();
                            hour = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaHour();
                            Thread.Sleep(1000);
                        }
                    }
                }
                et = MaxEt;
                PluginLog.Log($"当前ET结束...");
                int count = CommonUi.CanExtractMateria();
                if (count >= 5)
                {
                    WoAutoCollectionPlugin.GameData.CommonBot.ExtractMateria(count);
                }
                int CollectableCount = CommonUi.CanExtractMateriaCollectable();
                if (CollectableCount > 0)
                {
                    WoAutoCollectionPlugin.GameData.CommonBot.ExtractMateriaCollectable(CollectableCount);
                }
                if (CommonUi.NeedsRepair())
                {
                    Teleporter.Teleport(Positions.ShopTp);
                    MovePositions(Positions.RepairNPC, false);
                    WoAutoCollectionPlugin.GameData.CommonBot.NpcRepair("阿塔帕");
                }
                WoAutoCollectionPlugin.Time.Update();
                hour = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaHour();
            }
        }

        private void RunWaitTask(string lv) {
            WoAutoCollectionPlugin.Time.Update();
            hour = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaHour();
            int minute = WoAutoCollectionPlugin.Time.ServerTime.CurrentEorzeaMinute();
            othetRun = true;
            
            if (otherTaskParam == "0") {
                othetRun = true;
                PluginLog.Log($"当前配置: {otherTaskParam}, 不执行其他任务");
                Task task = new(() =>
                {
                    Thread.Sleep(7000);
                    othetRun = false;
                });
                task.Start();
            } else if (otherTaskParam == "1") {
                PluginLog.Log($"当前配置: {otherTaskParam}, 采集任务");
                if ((et - hour == 1 && minute > 20) || (hour - et > 3 && 24 - hour == 1 && minute > 20))
                {
                    PluginLog.Log($"间隔时间短暂, 不执行其他任务 hour: {hour}, minute: {minute}, 下个et: {et}");
                    Task task = new(() =>
                    {
                        Thread.Sleep(7000);
                        othetRun = false;
                    });
                    task.Start();
                }
                else {
                    needTp = true;
                    Task task = new(() =>
                    {
                        PluginLog.Log($"执行等待采集任务...");
                        try
                        {
                            WoAutoCollectionPlugin.GameData.GatherBot.RunNormalScript(0, lv);
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
            } else if (otherTaskParam == "2") {
                othetRun = true;
                PluginLog.Log($"当前配置: {otherTaskParam}, 快速制作任务");
                Task task = new(() =>
                {
                    PluginLog.Log($"执行等待快速制作任务...");
                    try
                    {
                        WoAutoCollectionPlugin.GameData.CraftBot.RunCraftScript();
                    }
                    catch (Exception e)
                    {
                        PluginLog.Error($"其他任务, error!!!\n{e}");
                    }
                    if (RecipeNoteUi.RecipeNoteIsOpen())
                    {
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                    }
                    PluginLog.Log($"其他任务结束...");
                    othetRun = false;
                });
                task.Start();
            }
            else if (otherTaskParam == "4")
            {
                othetRun = true;
                PluginLog.Log($"当前配置: {otherTaskParam}, 捕鱼灵砂任务");
                Task task = new(() =>
                {
                    if (!CommonUi.CurrentJob(18))
                    {
                        Thread.Sleep(500);
                        WoAutoCollectionPlugin.Executor.DoGearChange("捕鱼人");
                        Thread.Sleep(500);
                    }

                    if (CommonUi.CanRepair())
                    {
                        Teleporter.Teleport(Positions.ShopTp);
                        MovePositions(Positions.RepairNPC, false);
                        WoAutoCollectionPlugin.GameData.CommonBot.NpcRepair("阿塔帕");
                        Thread.Sleep(500);
                    }
                    PluginLog.Log($"执行等待捕鱼灵砂任务...");
                    try
                    {
                        Random r = new();
                        int t = r.Next(3, 5);
                        string args = "ftype:" + t + " fexchangeItem:0";
                        PluginLog.Log($"执行等待捕鱼灵砂任务: {args}");
                        WoAutoCollectionPlugin.GameData.CollectionFishBot.CollectionFishScript(args);
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
            else if (otherTaskParam == "5")
            {
                othetRun = true;
                PluginLog.Log($"当前配置: {otherTaskParam}, 刺鱼灵砂任务");
                Task task = new(() =>
                {
                    if (!CommonUi.CurrentJob(18))
                    {
                        Thread.Sleep(500);
                        WoAutoCollectionPlugin.Executor.DoGearChange("捕鱼人");
                        Thread.Sleep(500);
                    }

                    if (CommonUi.CanRepair())
                    {
                        Teleporter.Teleport(Positions.ShopTp);
                        MovePositions(Positions.RepairNPC, false);
                        WoAutoCollectionPlugin.GameData.CommonBot.NpcRepair("阿塔帕");
                        Thread.Sleep(500);
                    }
                    PluginLog.Log($"执行等待刺鱼灵砂任务...");
                    try
                    {
                        string args = "ftype:5";
                        PluginLog.Log($"执行等待刺鱼灵砂任务: {args}");
                        WoAutoCollectionPlugin.GameData.CollectionFishBot.SpearfishScript(args);
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
        }

        private void StopWaitTask()
        {
            WoAutoCollectionPlugin.GameData.GatherBot.StopScript();
            WoAutoCollectionPlugin.GameData.CraftBot.StopScript();
            WoAutoCollectionPlugin.GameData.CollectionFishBot.StopScript();
        }

        private Vector3 MovePositions(Vector3[] Path, bool UseMount)
        {
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            ushort SizeFactor = WoAutoCollectionPlugin.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            Vector3 position = WoAutoCollectionPlugin.GameData.KeyOperates.GetUserPosition(SizeFactor);
            for (int i = 0; i < Path.Length; i++)
            {
                if (closed)
                {
                    PluginLog.Log($"中途结束");
                    return WoAutoCollectionPlugin.GameData.KeyOperates.GetUserPosition(SizeFactor);
                }
                position = WoAutoCollectionPlugin.GameData.KeyOperates.MoveToPoint(position, Path[i], territoryType, UseMount, false);
            }
            return position;
        }

    }
}