using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AlphaProject.Managers;
using AlphaProject.Ui;
using AlphaProject.UseAction;
using AlphaProject.Utility;
using Npc = AlphaProject.Data.Npc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using AlphaProject.Helper;

namespace AlphaProject.Bot
{
    public unsafe static class CommonBot
    {
        private static bool Closed = false;

        // 捕鱼嘉惠
        private static bool useNaturesBounty { get; set; }

        // 刺鱼
        private static bool useGig { get; set; }

        public static void Init()
        {
            Closed = false;
            useNaturesBounty = true;
        }

        public static void StopScript()
        {
            Closed = true;
        }

        // TODO 关闭所有可能打开的ui
        public static void CloseAllIfOpen() {
            if (RecipeNoteUi.RecipeNoteIsOpen())
            {
                Thread.Sleep(300 + new Random().Next(100, 200));
                KeyOperates.KeyMethod(Keys.esc_key);
                Thread.Sleep(300 + new Random().Next(100, 200));
            }
            if (CommonUi.AddonMaterializeDialogIsOpen())
            {
                Thread.Sleep(300 + new Random().Next(100, 200));
                KeyOperates.KeyMethod(Keys.esc_key);
                Thread.Sleep(300 + new Random().Next(100, 200));
            }
        }

        // 修理
        public static bool NpcRepair(string npc)
        {
            if (npc == "") {
                AutoChooseNpc(out bool succeed, out npc);
                if (!succeed)
                {
                    PluginLog.Log($"not find repair npc");
                    return false;
                }
                else
                {
                    PluginLog.Log($"find repair npc: {npc}");
                }
            }
            int n = 0;
            while (DalamudApi.Condition[ConditionFlag.Mounted])
            {
                if (n >= 3)
                {
                    KeyOperates.KeyMethod(Keys.q_key, 200);
                }
                KeyOperates.KeyMethod(Keys.q_key);
                Thread.Sleep(1000 + new Random().Next(100, 200));
                n++;

                if (Closed)
                {
                    PluginLog.Log($"NpcRepair stopping");
                    return true;
                }
            }
            bool flag = true;
            CommonHelper.SetTarget(npc);
            Thread.Sleep(1500);
            if (CommonUi.AddonSelectIconStringIsOpen())
            {
                CommonUi.SelectIconString2Button();
                Thread.Sleep(1500);
            }
            
            if (CommonUi.AddonRepairIsOpen() && CommonUi.AllRepairButton())
            {
                Thread.Sleep(800 + new Random().Next(200, 300));
                CommonUi.SelectYesButton();
                Thread.Sleep(800 + new Random().Next(200, 300));
            }
            else
            {
                flag = false;
            }
            KeyOperates.KeyMethod(Keys.esc_key);
            Thread.Sleep(1000);
            return flag;
        }

        public static void AutoChooseNpc(out bool succeed, out string npc) {
            succeed = false;
            npc = "";
            List<string> names = Npc.NpcNames;
            foreach (string name in names) {
                if (Closed) return;
                var target = DalamudApi.ObjectTable.FirstOrDefault(obj => obj.Name.TextValue.ToLowerInvariant() == name);
                if (target != default)
                {
                    succeed = true;
                    npc = name;
                    return;
                }
            }
        }

        // 精制
        public unsafe static bool ExtractMateria(int count)
        {
            if (count < 2)
            {
                PluginLog.Log($"count: {count}, 不需要精制");
                return true;
            }

            Init();
            int n = 0;
            while (DalamudApi.Condition[ConditionFlag.Mounted])
            {
                if (n >= 3)
                {
                    KeyOperates.KeyMethod(Keys.q_key, 200);
                }
                KeyOperates.KeyMethod(Keys.q_key);
                Thread.Sleep(1000 + new Random().Next(100, 200));
                n++;

                if (Closed)
                {
                    PluginLog.Log($"ExtractMateria stopping");
                    return true;
                }
            }
            if (Closed)
            {
                PluginLog.Log($"ExtractMateria stopping");
                return true;
            }
            if (RecipeNoteUi.RecipeNoteIsOpen())
            {
                KeyOperates.KeyMethod(Keys.esc_key);
                Thread.Sleep(2000 + new Random().Next(100, 200));
            }
            KeyOperates.KeyMethod(Keys.F11_key);
            Thread.Sleep(1500 + new Random().Next(100, 200));
            for (int i = 0; i < count; i++) {
                if (!GenericHelper.TryGetAddonByName<AtkUnitBase>("Materialize", out var addon) || !addon->IsVisible)
                {
                    return true;
                }

                KeyOperates.KeyMethod(Keys.num0_key);
                Thread.Sleep(1000 + new Random().Next(100, 200));
                if (GenericHelper.TryGetAddonByName<AtkUnitBase>("MaterializeDialog", out addon) && addon->IsVisible) {
                    CommonUi.SelectMaterializeDialogYesButton();
                    Thread.Sleep(3000 + new Random().Next(100, 200));
                }
                Thread.Sleep(500 + new Random().Next(100, 200));
            }

            if (GenericHelper.TryGetAddonByName<AtkUnitBase>("MaterializeDialog", out var materializeDialog) && materializeDialog->IsVisible) {
                KeyOperates.KeyMethod(Keys.esc_key);
            }

            if (GenericHelper.TryGetAddonByName<AtkUnitBase>("Materialize", out var materialize) && materialize->IsVisible)
            {
                KeyOperates.KeyMethod(Keys.esc_key);
            }

            Thread.Sleep(500 + new Random().Next(100, 200));
            return true;
        }

        // 精选
        public unsafe static bool ExtractMateriaCollectable(int CollectableCount)
        {
            int tt = 0;
            while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 5)
            {
                if (tt >= 3)
                {
                    KeyOperates.KeyMethod(Keys.w_key, 200);
                }
                KeyOperates.KeyMethod(Keys.q_key);
                Thread.Sleep(900 + new Random().Next(50, 80));
                tt++;
            }
            KeyOperates.KeyMethod(Keys.F10_key);
            for (int i = 0; i < CollectableCount; i++) {
                if (DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing]) {
                    KeyOperates.KeyMethod(Keys.F1_key);
                    continue;
                }
                if (!CommonUi.AddonPurifyItemSelectorIsOpen())
                {
                    return true;
                }
                KeyOperates.KeyMethod(Keys.num0_key);
                KeyOperates.KeyMethod(Keys.num0_key);
                Thread.Sleep(2800 + new Random().Next(100, 300));
            }
            while (CommonUi.AddonPurifyItemSelectorIsOpen() || CommonUi.AddonPurifyResultIsOpen()) {
                KeyOperates.KeyMethod(Keys.esc_key);
                Thread.Sleep(500 + new Random().Next(100, 200));
            }
            return true;
        }
        
        public static bool UseItem() {
            //PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
            //uint gp = player.CurrentGp;
            //if (gp < player.MaxGp * 0.6)
            //{
            //    KeyOperates.KeyMethod(Keys.plus_key);
            //    Thread.Sleep(2000);
            //}
            return true;
        }

        public static bool UseItem(double factor)
        {
            PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
            uint gp = player.CurrentGp;
            if (gp < player.MaxGp * factor)
            {
                KeyOperates.KeyMethod(Keys.plus_key);
                Thread.Sleep(2000);
            }
            return true;
        }

        // 限时材料采集手法
        public static bool LimitMaterialsMethod(string Names) {
            PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;

            uint gp = player.CurrentGp;
            int level = player.Level;

            List<string> list = new();
            string[] names = Names.Split('|');
            PluginLog.Log($"准备采集: {Names}");
            foreach (string na in names) {
                list.Add(na);
            }
            (int GatherIndex, string name) = CommonUi.GetGatheringIndex(list);
            int action = 0;
            if (name.Contains("雷之") || name.Contains("火之") || name.Contains("风之") || name.Contains("水之") || name.Contains("冰之") || name.Contains("土之")) {
                if (gp >= 200)
                {
                    KeyOperates.KeyMethod(Keys.F4_key);
                    gp -= 200;
                    action++;
                }
                else if (gp >= 150) {
                    KeyOperates.KeyMethod(Keys.F3_key);
                    gp -= 150;
                    action++;
                }
            } else if (name.Contains("土壤")) {
                action++;
                // 不用技能
            } else {
                if (level >= 50)
                {
                    if (gp >= 500)
                    {
                        KeyOperates.KeyMethod(Keys.F2_key);
                        gp -= 500;
                        action++;
                        Thread.Sleep(2000 + new Random().Next(100, 300));
                    }
                }
                else
                {
                    if (gp >= 400)
                    {
                        KeyOperates.KeyMethod(Keys.F1_key);
                        gp -= 400;
                        action++;
                        Thread.Sleep(2000 + new Random().Next(100, 300));
                    }
                }
            }

            int tt = 0;
            while (CommonUi.AddonGatheringIsOpen() && tt < 15)
            {
                CommonUi.GatheringButton(GatherIndex);
                Thread.Sleep(2000 + new Random().Next(100, 300));
                tt++;
                if (tt == 4)
                {
                    gp = player.CurrentGp;
                    level = player.Level;
                    if (gp >= 300 && action > 0)
                    {
                        if (level >= 25)
                        {
                            KeyOperates.KeyMethod(Keys.n3_key);
                            Thread.Sleep(1500);
                            if (level >= 90)
                            {
                                KeyOperates.KeyMethod(Keys.n4_key);
                                Thread.Sleep(1000 + new Random().Next(200, 400));
                            }
                        }
                    }
                } else if (tt == 2) {
                    if (name.Contains("土壤") || name.Contains("地图"))
                    {
                        (GatherIndex, name) = CommonUi.GetGatheringIndex(list);
                    }
                }
                
            }

            return true;
        }

        // 普通材料采集手法 TODO GetRecastTimeElapsed 技能CD测试
        public static bool NormalMaterialsMethod(string Names)
        {
            PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;

            uint gp = player.CurrentGp;
            int level = player.Level;

            List<string> list = new();
            string[] names = Names.Split('|');
            PluginLog.Log($"准备采集: {Names}");
            foreach (string na in names)
            {
                list.Add(na);
            }
            var jobId = DalamudApi.ClientState.LocalPlayer?.ClassJob.Id;
            // 4589 - 采矿 大地的恩惠 4590 - 园艺 大地的恩惠
            int skillId = 4589;
            if (jobId == 17) {
                skillId = 4590;
            }
            bool coolDown = false;
            if (Game.GetSpellActionRecastTimeElapsed((uint)skillId) == 0) {
                coolDown = true;
            }
            
            (int GatherIndex, string name) = CommonUi.GetNormalGatheringIndex(list, coolDown);
            PluginLog.Log($"预计采集: {Names}, {coolDown} {GatherIndex}");
            int action = 0;
            if (name.Contains("之水晶"))
            {
                int id = NormalItems.GetNormalItemId(name);
                int count = BagManager.GetInventoryItemCount((uint)id);
                if (count < 9000 && gp >= 200 && !name.Contains("冰之水晶"))
                {
                    KeyOperates.KeyMethod(Keys.F4_key);
                    gp -= 200;
                    action++;
                }
                else {
                    (GatherIndex, name) = CommonUi.GetNormalGatheringIndex(list, false);
                }
            }
            
            if (!name.Contains("之水晶") && level >= 50)
            {
                if (gp >= 500)
                {
                    KeyOperates.KeyMethod(Keys.F2_key);
                    gp -= 500;
                    action++;
                    Thread.Sleep(2000 + new Random().Next(100, 300));
                }
            }
            else
            {
                if (gp >= 400)
                {
                    KeyOperates.KeyMethod(Keys.F1_key);
                    gp -= 400;
                    action++;
                    Thread.Sleep(2000 + new Random().Next(100, 300));
                }
            }

            PluginLog.Log($"开始采集: {name}");
            int tt = 0;
            while (CommonUi.AddonGatheringIsOpen() && tt < 15)
            {
                CommonUi.GatheringButton(GatherIndex);
                Thread.Sleep(2000 + new Random().Next(100, 300));
                tt++;
                if (tt == 4)
                {
                    gp = player.CurrentGp;
                    level = player.Level;
                    if (gp >= 300 && action > 0)
                    {
                        if (level >= 25)
                        {
                            KeyOperates.KeyMethod(Keys.n3_key);
                            Thread.Sleep(1500);
                            if (level >= 90)
                            {
                                KeyOperates.KeyMethod(Keys.n4_key);
                                Thread.Sleep(1000);
                            }
                        }
                    }
                }
                else if (tt == 2)
                {
                    if (name.Contains("地图"))
                    {
                        (GatherIndex, name) = CommonUi.GetNormalGatheringIndex(list, false);
                    }
                }
            }
            return true;
        }

        // 限时收藏品采集手法
        public static bool LimitMultiMaterialsMethod(string Name)
        {
            PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
            uint gp = player.CurrentGp;
            List<string> list = new();
            list.Add(Name);
            PluginLog.Log($"开始采集: {Name}");
           
            (int GatherIndex, string name) = CommonUi.GetGatheringIndex(list);
            CommonUi.GatheringButton(GatherIndex);
            Thread.Sleep(2000 + new Random().Next(100, 300));

            int action = 0;
            if (gp >= 700)
            {
                action++;
                KeyOperates.KeyMethod(Keys.F6_key);
                Thread.Sleep(1800 + new Random().Next(200, 500));
                KeyOperates.KeyMethod(Keys.F7_key);
                Thread.Sleep(2400 + new Random().Next(200, 500));
                KeyOperates.KeyMethod(Keys.F6_key);
                Thread.Sleep(1800 + new Random().Next(200, 500));
                KeyOperates.KeyMethod(Keys.F7_key);
                Thread.Sleep(2400 + new Random().Next(200, 500));
                KeyOperates.KeyMethod(Keys.F9_key);
                Thread.Sleep(2400 + new Random().Next(200, 500));

                if (gp >= 300 && action > 0)
                {
                    KeyOperates.KeyMethod(Keys.n3_key);
                    Thread.Sleep(1500 + new Random().Next(0, 200));
                    KeyOperates.KeyMethod(Keys.n4_key);
                    Thread.Sleep(1000 + new Random().Next(0, 200));
                }
            }
            else {
                KeyOperates.KeyMethod(Keys.F8_key);
                Thread.Sleep(2400 + new Random().Next(200, 500));
                KeyOperates.KeyMethod(Keys.F8_key);
                Thread.Sleep(2400 + new Random().Next(200, 500));
                KeyOperates.KeyMethod(Keys.F8_key);
                Thread.Sleep(2400 + new Random().Next(200, 500));
            }
            int tt = 0;
            while (CommonUi.AddonGatheringMasterpieceIsOpen() && tt < 7)
            {
                KeyOperates.KeyMethod(Keys.F5_key);
                Thread.Sleep(2500 + new Random().Next(300, 800));
                tt++;
            }
            return true;
        }

        // 刺鱼
        public static bool SpearfishMethod() {
            int n = 0;
            PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
            while (CommonUi.AddonSpearFishingIsOpen()) {
                Thread.Sleep(10);

                if (useGig)
                {
                    KeyOperates.KeyMethod(Keys.r_key);
                    CommonBot.canUseGig(false);
                    Thread.Sleep(new Random().Next(100, 200));
                }
                if (CommonUi.HasStatus("刺鱼人的直觉") && player.CurrentGp >= 300) {
                    KeyOperates.KeyMethod(Keys.F9_key);
                    Thread.Sleep(new Random().Next(200, 250));
                }
                if (n < 200) {
                    if (useNaturesBounty && !CommonUi.HasStatus("嘉惠") && player.CurrentGp >= 100)
                    {
                        KeyOperates.KeyMethod(Keys.t_key);
                        Thread.Sleep(new Random().Next(300, 350));
                    }
                }
                n++;
            }
            return true;
        }

        public static void canUseNaturesBounty(bool b) {
            useNaturesBounty = b;
        }

        public static void canUseGig(bool b)
        {
            useGig = b;
        }
    }
}