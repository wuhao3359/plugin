using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using AlphaProject.SeFunctions;
using AlphaProject.Time;
using AlphaProject.Ui;
using AlphaProject.Utility;
using AlphaProject.Helper;

namespace AlphaProject.Bot
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
            AlphaProject.GameData.CraftBot.Init();
            AlphaProject.GameData.CommonBot.Init();
            AlphaProject.GameData.KeyOperates.Init();
            AlphaProject.GameData.CollectionFishBot.Init();
        }

        public void StopScript()
        {
            closed = true;
            Teleporter.count = 0;
            AlphaProject.GameData.CommonBot.StopScript();
            AlphaProject.GameData.GatherBot.StopScript();
            AlphaProject.GameData.CollectionFishBot.StopScript();
            AlphaProject.GameData.MarketBot.StopScript();
        }

        public void DailyScript(string args)
        {
            Init();
            closed = false;
            try {
                // 参数解析
                string command = Tasks.Daily;
                AlphaProject.GameData.param = Util.CommandParse(command, args);

                if (AlphaProject.GameData.param.TryGetValue("otherTask", out var ot))
                {
                    otherTaskParam = ot;
                }

                string lv = "50";
                if (AlphaProject.GameData.param.TryGetValue("level", out var l)) {
                    lv = l;
                }

                if (AlphaProject.GameData.param.TryGetValue("duration", out var d))
                {
                    PluginLog.Log($"d: {d} ss[1]: {d}");
                    if (d == "1")
                    {
                        // 限时单次采集
                        LimitTimeSinglePlan(lv);
                    }
                    else if (d == "2")
                    {
                        // 限时多次采集
                        LimitTimeMultiPlan(lv);
                    }
                    else if (d == "3")
                    {
                        // 灵砂刺鱼
                        OnlySpearfishPlan();
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

            AlphaProject.Time.Update();
            hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
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
                            (string Name, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, uint Type) = LimitMaterials.GetMaterialById(id);
                            Teleporter.Teleport(Tp);
                            needTp = false;
                            break;
                        }
                    }

                    RunWaitTask(lv);
                    while (othetRun) {
                        AlphaProject.Time.Update();
                        hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
                        if (closed)
                        {
                            PluginLog.Log($"中途结束");
                            return;
                        }
                        PluginLog.Log($"当前时间{hour} wait to {et} ..");
                        Thread.Sleep(5000 + new Random().Next(2000, 5000));
                        if (hour == et) {
                           StopWaitTask();
                        }
                    }
                    AlphaProject.Time.Update();
                    hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
                    PluginLog.Log($"当前时间{hour} wait to {et} ..");
                }

                int num = 0;
                foreach (int id in ids) {
                    if (closed)
                    {
                        PluginLog.Log($"中途结束");
                        return;
                    }
                    
                    (string Name, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, uint Type) = LimitMaterials.GetMaterialById(id);
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
                    AlphaProject.GameData.CommonBot.UseItem();

                    PluginLog.Log($"开始执行任务, id: {id} ");
                    // 切换职业 
                    if (!CommonUi.CurrentJob(Job))
                    {
                        Thread.Sleep(2000);
                        CommandProcessorHelper.DoGearChange(JobName);
                        Thread.Sleep(200 + new Random().Next(300, 800));
                    }
                    Thread.Sleep(800 + new Random().Next(300, 800));
                    Vector3 position = AlphaProject.GameData.KeyOperates.MovePositions(Path, true);
                    // 找最近的采集点
                    ushort territoryType = DalamudApi.ClientState.TerritoryType;
                    ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(territoryType);
                    (GameObject go, Vector3 point) = Util.LimitTimePosCanGather(Points, SizeFactor);

                    if (go != null)
                    {
                        float x = Maths.GetCoordinate(go.Position.X, SizeFactor);
                        float y = Maths.GetCoordinate(go.Position.Y, SizeFactor);
                        float z = Maths.GetCoordinate(go.Position.Z, SizeFactor);
                        PluginLog.Log($"目标高度: {Maths.GetCoordinate(go.Position.Y, SizeFactor)} < ---> 辅助点高度: {y}");
                        Vector3 GatherPoint = new(x, y, z);
                        position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, point, territoryType, true, false);
                        position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, GatherPoint, territoryType, false, false);

                        var targetMgr = DalamudApi.TargetManager;
                        targetMgr.SetTarget(go);

                        int tt = 0;
                        while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 7)
                        {
                            if (tt >= 3)
                            {
                                AlphaProject.GameData.KeyOperates.KeyMethod(Keys.w_key, 200);
                            }
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.q_key);
                            Thread.Sleep(800 + new Random().Next(300, 800));
                            tt++;

                            if (closed)
                            {
                                PluginLog.Log($"dailyTask stopping");
                                return;
                            }
                        }

                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.down_arrow_key, 200);

                        if (Type == 2) {
                            PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                            uint gp = player.CurrentGp;
                            if (gp < 700)
                            {
                                Thread.Sleep(10000);
                            }
                        }

                        tt = 0;
                        while (!CommonUi.AddonGatheringIsOpen() && tt < 5)
                        {
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                            Thread.Sleep(200 + new Random().Next(300, 800));
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
                        Thread.Sleep(800 + new Random().Next(300, 800));

                        if (CommonUi.AddonGatheringIsOpen())
                        {
                            if (Type == 2)
                            {
                                AlphaProject.GameData.CommonBot.LimitMultiMaterialsMethod(Name);
                            }
                            else {
                                AlphaProject.GameData.CommonBot.LimitMaterialsMethod(Name);
                            }
                        }
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.up_arrow_key, 200);
                    }
                    else {
                        PluginLog.Log($"未知原因未找到数据, skip {id}..");
                        finishIds.Add(id);
                        Thread.Sleep(800 + new Random().Next(300, 800));
                        continue;
                    }
                    // finish work
                    finishIds.Add(id);
                    num++;
                    AlphaProject.Time.Update();
                    hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
                    Thread.Sleep(1800 + new Random().Next(300, 800));

                    int count = CommonUi.CanExtractMateria();
                    if (count >= 5)
                    {
                        AlphaProject.GameData.CommonBot.ExtractMateria(count);
                    }
                    if (CommonUi.NeedsRepair())
                    {
                        Teleporter.Teleport(Positions.ShopTp);
                        AlphaProject.GameData.KeyOperates.MovePositions(Positions.RepairNPC, false);
                        AlphaProject.GameData.CommonBot.NpcRepair("阿塔帕");
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
            AlphaProject.Time.Update();
            int hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
            int et = hour;
            while (!closed && n < 1000)
            {
                AlphaProject.GameData.MarketBot.RunScript(2);
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
                        if (closed)
                        {
                            PluginLog.Log($"中途结束");
                            return;
                        }
                        AlphaProject.Time.Update();
                        hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
                        PluginLog.Log($"当前时间{hour} wait to {et} ..");
                        Thread.Sleep(5000 + new Random().Next(0, 5000));
                        if (hour == et)
                        {
                            StopWaitTask();
                        }
                    }
                }

                (int Id, string Name, int MinEt, int MaxEt, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, int[] CanGatherIndex) = LimitMaterials.GetCollecMaterialById(id);
                AlphaProject.Time.Update();
                hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
                PluginLog.Log($"开始执行任务, id: {id} Name: {Name}, Job: {Job}, MinEt: {MinEt}, MaxEt: {MaxEt}..");
                if (Tp == 0)
                {
                    PluginLog.Log($"数据异常, skip {id}..");
                    break;
                }
                Teleporter.Teleport(Tp);
                
                AlphaProject.GameData.CommonBot.UseItem();
                ushort territoryType = DalamudApi.ClientState.TerritoryType;
                ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(territoryType);
                // 切换职业 
                if (!CommonUi.CurrentJob(Job))
                {
                    Thread.Sleep(1800 + new Random().Next(300, 800));
                    CommandProcessorHelper.DoGearChange(JobName);
                    Thread.Sleep(200 + new Random().Next(300, 800));
                }
                Thread.Sleep(500 + new Random().Next(300, 800));
                Vector3 position = AlphaProject.GameData.KeyOperates.MovePositions(Path, true);
                while (hour >= MinEt && hour <= MaxEt) {
                    AlphaProject.Time.Update();
                    hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
                    int minute = AlphaProject.Time.ServerTime.CurrentEorzeaMinute();
                    if (hour == MaxEt && minute >= 45)
                    {
                        PluginLog.Log($"时间不够...");
                        break;
                    }
                    for (int t = 0; t < Points.Length && hour >= MinEt && hour <= MaxEt; t++) {
                        AlphaProject.Time.Update();
                        hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
                        minute = AlphaProject.Time.ServerTime.CurrentEorzeaMinute();
                        if (hour == MaxEt && minute >= 45) {
                            PluginLog.Log($"时间不够...");
                            break;
                        }
                        if (closed)
                        {
                            PluginLog.Log($"中途结束");
                            return;
                        }
                        AlphaProject.GameData.KeyOperates.MoveToPoint(position, Points[t], territoryType, true, false);
                        if (Array.IndexOf(CanGatherIndex, t) != -1)
                        {
                            GameObject go = Util.CurrentPositionCanGather(Points[t], SizeFactor);
                            if (go != null)
                            {
                                float x = Maths.GetCoordinate(go.Position.X, SizeFactor);
                                float y = Maths.GetCoordinate(go.Position.Y, SizeFactor);
                                float z = Maths.GetCoordinate(go.Position.Z, SizeFactor);
                                PluginLog.Log($"目标高度: {Maths.GetCoordinate(go.Position.Y, SizeFactor)} <---> 辅助点高度: {y}");
                                Vector3 GatherPoint = new(x, y, z);
                                position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, GatherPoint, territoryType, false, false);

                                AlphaProject.Time.Update();
                                hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
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
                                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.w_key, 200);
                                    }
                                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.q_key);
                                    Thread.Sleep(500 + new Random().Next(300, 800));
                                    tt++;
                                }

                                AlphaProject.GameData.KeyOperates.KeyMethod(Keys.down_arrow_key, 200);
                                tt = 0;
                                while (!CommonUi.AddonGatheringIsOpen() && tt < 4)
                                {
                                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                                    Thread.Sleep(100 + new Random().Next(300, 800));
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
                                Thread.Sleep(800 + new Random().Next(300, 800));

                                if (CommonUi.AddonGatheringIsOpen())
                                {
                                    AlphaProject.GameData.CommonBot.LimitMultiMaterialsMethod(Name);
                                    AlphaProject.GameData.CommonBot.UseItem(0.65);
                                }
                                AlphaProject.GameData.KeyOperates.KeyMethod(Keys.up_arrow_key, 150);
                            }
                            else
                            {
                                PluginLog.Log($"未知原因未找到数据, skip {id}..");
                                t++;
                                Thread.Sleep(800 + new Random().Next(300, 800));
                                continue;
                            }
                            AlphaProject.Time.Update();
                            hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
                            Thread.Sleep(800 + new Random().Next(300, 800));
                        }
                    }
                }
                et = MaxEt;
                PluginLog.Log($"当前ET结束...");
                int count = CommonUi.CanExtractMateria();
                if (count >= 5)
                {
                    AlphaProject.GameData.CommonBot.ExtractMateria(count);
                }
                int CollectableCount = CommonUi.CanExtractMateriaCollectable();
                if (CollectableCount > 0)
                {
                    AlphaProject.GameData.CommonBot.ExtractMateriaCollectable(CollectableCount);
                }
                if (CommonUi.NeedsRepair())
                {
                    Teleporter.Teleport(Positions.ShopTp);
                    AlphaProject.GameData.KeyOperates.MovePositions(Positions.RepairNPC, false);
                    AlphaProject.GameData.CommonBot.NpcRepair("阿塔帕");
                }
                
                AlphaProject.Time.Update();
                hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
            }
        }

        public void OnlySpearfishPlan()
        {
            int n = 0;
            AlphaProject.Time.Update();
            int hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
            bool run = false;
            while (!closed && n < 1000)
            {
                if (hour == 0) {
                    run = true;
                }
                AlphaProject.GameData.MarketBot.RunScript(2);
                RunWaitTask("90");
                while (othetRun)
                {
                    if (closed) return;
                    AlphaProject.Time.Update();
                    hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
                    if (hour != 0)
                    {
                        run = false;
                    }
                    Thread.Sleep(4000 + new Random().Next(1000, 2000));
                    if (hour == 0 && !run)
                    {
                        StopWaitTask();
                    }
                }

                PluginLog.Log($"当前ez day结束...");
                int count = CommonUi.CanExtractMateria();
                if (count >= 5)
                {
                    AlphaProject.GameData.CommonBot.ExtractMateria(count);
                }
                int CollectableCount = CommonUi.CanExtractMateriaCollectable();
                if (CollectableCount > 0)
                {
                    AlphaProject.GameData.CommonBot.ExtractMateriaCollectable(CollectableCount);
                }
                AlphaProject.Time.Update();
                hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
            }
        }

        private void RunWaitTask(string lv) {
            AlphaProject.Time.Update();
            hour = AlphaProject.Time.ServerTime.CurrentEorzeaHour();
            int minute = AlphaProject.Time.ServerTime.CurrentEorzeaMinute();
            othetRun = true;
            
            if (otherTaskParam == "0") {
                othetRun = true;
                PluginLog.Log($"当前配置: {otherTaskParam}, 不执行其他任务");
                Task task = new(() =>
                {
                    Thread.Sleep(5000 + new Random().Next(2000, 4000));
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
                        Thread.Sleep(5000 + new Random().Next(2000, 4000));
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
                            AlphaProject.GameData.GatherBot.RunNormalScript(0, lv);
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
                        AlphaProject.GameData.CraftBot.RunCraftScript();
                    }
                    catch (Exception e)
                    {
                        PluginLog.Error($"其他任务, error!!!\n{e}");
                    }
                    if (RecipeNoteUi.RecipeNoteIsOpen())
                    {
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.esc_key);
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
                        Thread.Sleep(200 + new Random().Next(300, 800));
                        CommandProcessorHelper.DoGearChange("捕鱼人");
                        Thread.Sleep(200 + new Random().Next(300, 800));
                    }

                    if (CommonUi.CanRepair())
                    {
                        Teleporter.Teleport(Positions.ShopTp);
                        AlphaProject.GameData.KeyOperates.MovePositions(Positions.RepairNPC, false);
                        AlphaProject.GameData.CommonBot.NpcRepair("阿塔帕");
                        Thread.Sleep(500 + new Random().Next(100, 300));
                    }
                    try
                    {
                        Random r = new();
                        int t = r.Next(3, 5);
                        string args = "ftype:" + t + " fexchangeItem:0";
                        PluginLog.Log($"执行等待捕鱼灵砂任务: {args}");
                        AlphaProject.GameData.CollectionFishBot.CollectionFishScript(args);
                    }
                    catch (Exception e)
                    {
                        PluginLog.Error($"执行等待捕鱼灵砂任务, error!!!\n{e}");
                    }
                    PluginLog.Log($"执行等待捕鱼灵砂任务结束...");
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
                        Thread.Sleep(500 + new Random().Next(100, 300));
                        CommandProcessorHelper.DoGearChange("捕鱼人");
                        Thread.Sleep(500 + new Random().Next(100, 300));
                    }

                    if (CommonUi.CanRepair())
                    {
                        Teleporter.Teleport(Positions.ShopTp);
                        AlphaProject.GameData.KeyOperates.MovePositions(Positions.RepairNPC, false);
                        AlphaProject.GameData.CommonBot.NpcRepair("阿塔帕");
                        Thread.Sleep(500 + new Random().Next(100, 300));
                    }
                    try
                    {
                        string args = "ftype:5";
                        PluginLog.Log($"执行等待刺鱼灵砂任务: {args}");
                        AlphaProject.GameData.CollectionFishBot.SpearfishScript(args);
                    }
                    catch (Exception e)
                    {
                        PluginLog.Error($"执行等待刺鱼灵砂任务, error!!!\n{e}");
                    }
                    PluginLog.Log($"执行等待刺鱼灵砂任务结束...");
                    othetRun = false;
                });
                task.Start();
            }
        }

        private void StopWaitTask()
        {
            AlphaProject.GameData.GatherBot.StopScript();
            AlphaProject.GameData.CraftBot.StopScript();
            AlphaProject.GameData.CollectionFishBot.StopScript();
        }

        private Vector3 MovePositions(Vector3[] Path, bool UseMount)
        {
            ushort territoryType = DalamudApi.ClientState.TerritoryType;
            ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            Vector3 position = AlphaProject.GameData.KeyOperates.GetUserPosition(SizeFactor);
            for (int i = 0; i < Path.Length; i++)
            {
                if (closed)
                {
                    PluginLog.Log($"中途结束");
                    return AlphaProject.GameData.KeyOperates.GetUserPosition(SizeFactor);
                }
                position = AlphaProject.GameData.KeyOperates.MoveToPoint(position, Path[i], territoryType, UseMount, false);
            }
            return position;
        }

    }
}