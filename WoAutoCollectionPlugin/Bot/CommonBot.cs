using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using WoAutoCollectionPlugin.Data;
using WoAutoCollectionPlugin.Managers;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.UseAction;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class CommonBot
    {
        private bool closed = false;

        public CommonBot()
        {
            Init();
        }

        public void Init()
        {
            closed = false;
        }

        public void StopScript()
        {
            closed = true;
        }

        // TODO 关闭所有可能打开的ui
        public void CloseAllIfOpen() {
            if (RecipeNoteUi.RecipeNoteIsOpen())
            {
                Thread.Sleep(300);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                Thread.Sleep(300);
            }
            if (CommonUi.AddonMaterializeDialogIsOpen())
            {
                Thread.Sleep(300);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                Thread.Sleep(300);
            }
        }

        // 修理
        public bool Repair() {
            int n = 0;
            while (DalamudApi.Condition[ConditionFlag.Mounted])
            {
                if (n >= 3)
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.w_key, 200);
                }
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.q_key);
                Thread.Sleep(1000);
                n++;

                if (closed)
                {
                    PluginLog.Log($"Repair stopping");
                    return true;
                }
            }
            bool flag = true;
            if (closed)
            {
                PluginLog.Log($"Repair stopping");
                return flag;
            }
            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F12_key);
            Thread.Sleep(1000);
            if (CommonUi.AllRepairButton())
            {
                Thread.Sleep(800);
                CommonUi.SelectYesButton();
                Thread.Sleep(3500);
            }
            else {
                flag = false;
            }

            n = 0;
            while (CommonUi.AddonRepairIsOpen() && n < 3)
            {
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
            }
            Thread.Sleep(500);
            return flag;
        }

        public bool NpcRepair(string npc)
        {
            int n = 0;
            while (DalamudApi.Condition[ConditionFlag.Mounted])
            {
                if (n >= 3)
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.q_key, 200);
                }
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.q_key);
                Thread.Sleep(1000);
                n++;

                if (closed)
                {
                    PluginLog.Log($"YGathing stopping");
                    return true;
                }
            }
            bool flag = true;
            SetTarget(npc);
            Thread.Sleep(1200);
            if (CommonUi.AddonSelectIconStringIsOpen())
            {
                CommonUi.SelectIconString2Button();
                Thread.Sleep(1500);
            }
            
            if (CommonUi.AddonRepairIsOpen() && CommonUi.AllRepairButton())
            {
                Thread.Sleep(800);
                CommonUi.SelectYesButton();
                Thread.Sleep(800);
            }
            else
            {
                flag = false;
            }
            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
            Thread.Sleep(500);
            return flag;
        }

        // 精制
        public bool ExtractMateria(int count)
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
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.q_key, 200);
                }
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.q_key);
                Thread.Sleep(1000);
                n++;

                if (closed)
                {
                    PluginLog.Log($"ExtractMateria stopping");
                    return true;
                }
            }
            if (closed)
            {
                PluginLog.Log($"ExtractMateria stopping");
                return true;
            }
            if (RecipeNoteUi.RecipeNoteIsOpen())
            {
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                Thread.Sleep(2000);
            }
            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F11_key);
            Thread.Sleep(1500);
            for (int i = 0; i < count; i++) {
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                Thread.Sleep(1000);
                if (CommonUi.AddonMaterializeDialogIsOpen()) {
                    CommonUi.SelectMaterializeDialogYesButton();
                    Thread.Sleep(3000);
                }
                Thread.Sleep(500);
            }

            if (CommonUi.AddonMaterializeDialogIsOpen()) {
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
            }

            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
            Thread.Sleep(500);
            return true;
        }

        // 精选
        public unsafe bool ExtractMateriaCollectable(int CollectableCount)
        {
            int tt = 0;
            while (DalamudApi.Condition[ConditionFlag.Mounted] && tt < 5)
            {
                if (tt >= 3)
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.w_key, 200);
                }
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.q_key);
                Thread.Sleep(800);
                tt++;
            }
            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F10_key);
            for (int i = 0; i < CollectableCount; i++) {
                if (DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing]) {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F1_key);
                }
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                Thread.Sleep(2600);
            }
            return true;
        }

        public bool CraftUploadAndExchange()
        {
            WoAutoCollectionPlugin.GameData.param.TryGetValue("recipeName", out var r);
            WoAutoCollectionPlugin.GameData.param.TryGetValue("exchangeItem", out var e);
            WoAutoCollectionPlugin.GameData.param.TryGetValue("uploadNPC", out var up);
            if (up == "1") {
                MovePositions(Positions.UploadNPCA, false);
            }

            (uint Category, uint Sub, uint ItemId) = RecipeItems.UploadApply(r);
            int tt = 0;
            while (BagManager.GetInventoryItemCountById(ItemId) > 0 && tt < 5) {
                if (closed)
                {
                    PluginLog.Log($"CraftUploadAndExchange stopping");
                    return true;
                }
                CraftUpload(Category, Sub, ItemId);
                CraftExchange(int.Parse(e));
                tt++;
            }
            return true;
        }

        // 交收藏品
        public bool CraftUpload(uint Category, uint Sub, uint ItemId)
        {
            PluginLog.Log($"CraftUploading");
            if (Category == 0 && Sub == 0) {
                return true;
            }
            bool flag = true;
            Thread.Sleep(1000);
            SetTarget("收藏品交易员");
            Thread.Sleep(2500);
            if (CommonUi.AddonCollectablesShopIsOpen()) {
                for (int i = 1; i < Category; i++)
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                }

                for (int i = 0; i < Sub; i++)
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                }

                int n = 0;
                int count = BagManager.GetInventoryItemCountById(ItemId);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                Thread.Sleep(1000);
                while (!RecipeNoteUi.SelectYesnoIsOpen() && n < 25 && count > 0)
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    Thread.Sleep(200);
                    if (RecipeNoteUi.SelectYesnoIsOpen()) {
                        break;
                    }
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    Thread.Sleep(700);
                    n++;
                    if (closed)
                    {
                        PluginLog.Log($"upload stopping");
                        return true;
                    }
                    if (count == BagManager.GetInventoryItemCountById(ItemId))
                    {
                        flag = false;
                        break;
                    }
                    else
                    {
                        count = BagManager.GetInventoryItemCountById(ItemId);
                    }
                }

                if (RecipeNoteUi.SelectYesnoIsOpen())
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                }
                while (CommonUi.AddonCollectablesShopIsOpen())
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                }
                Thread.Sleep(3000);
            }
            return flag;
        }

        // TODO 关闭界面
        // 交换道具
        public bool CraftExchange(int item)
        {
            PluginLog.Log($"CraftExchanging");
            Thread.Sleep(2000);
            SetTarget("工票交易员");
            Thread.Sleep(500);
            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
            Thread.Sleep(2000);

            if (closed)
            {
                PluginLog.Log($"exchange stopping");
                return true;
            }

            if (CommonUi.AddonInclusionShopIsOpen()) {
                // 1 
                if (item == 1)
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                }
                // 2-大地白票 随机采集魔晶石
                else if (item == 2)
                {
                    Random rd = new();
                    int r = rd.Next(19);
                    if (r <= 0) {
                        r = 0;
                    }
                    if (r >= 18)
                    {
                        r = 18;
                    }
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    for (int k = 0; k < r; k++) {
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    }
                }
                else if (item == 72)
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                }
                else if (item >= 80 && item <= 99)
                {
                    int r = item - 80;
                    if (r <= 0)
                    {
                        r = 0;
                    }
                    if (r >= 18)
                    {
                        r = 18;
                    }
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    for (int k = 0; k < r; k++)
                    {
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    }
                }
                else if (item == 101)
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                }
                else if (item == 102)
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                }
                else if (item >= 1000 && item <= 1100)
                {
                    int r = item - 1000;
                    if (r <= 0)
                    {
                        r = 0;
                    }
                    if (r >= 1006)
                    {
                        r = 5;
                    }
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    for (int k = 0; k < r; k++)
                    {
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    }
                }

                if (closed)
                {
                    PluginLog.Log($"exchange stopping");
                    return true;
                }

                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num6_key);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num9_key);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num4_key);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num4_key);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                while (CommonUi.AddonInclusionShopIsOpen())
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                    Thread.Sleep(1000);
                }
            }
            return true;
        }
        
        public bool UseItem() {
            PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
            uint gp = player.CurrentGp;
            if (gp < player.MaxGp * 0.6)
            {
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.plus_key);
                Thread.Sleep(2000);
            }
            return true;
        }

        public bool UseItem(double factor)
        {
            PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
            uint gp = player.CurrentGp;
            if (gp < player.MaxGp * factor)
            {
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.plus_key);
                Thread.Sleep(2000);
            }
            return true;
        }

        public bool SetTarget(string targetName) {
            var target = DalamudApi.ObjectTable.FirstOrDefault(obj => obj.Name.TextValue.ToLowerInvariant() == targetName);
            if (target == default) {
                return false;
            }

            DalamudApi.TargetManager.SetTarget(target);
            Thread.Sleep(200);
            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.num0_key);
            Thread.Sleep(800);
            return true;
        }

        // 限时材料采集手法
        public bool LimitMaterialsMethod(string Names) {
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
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F4_key);
                    gp -= 200;
                    action++;
                }
                else if (gp >= 150) {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F3_key);
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
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F2_key);
                        gp -= 500;
                        action++;
                        Thread.Sleep(2000);
                    }
                }
                else
                {
                    if (gp >= 400)
                    {
                        WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F1_key);
                        gp -= 400;
                        action++;
                        Thread.Sleep(2000);
                    }
                }
            }

            int tt = 0;
            while (CommonUi.AddonGatheringIsOpen() && tt < 15)
            {
                CommonUi.GatheringButton(GatherIndex);
                Thread.Sleep(2000);
                tt++;
                if (tt == 4)
                {
                    gp = player.CurrentGp;
                    level = player.Level;
                    if (gp >= 300 && action > 0)
                    {
                        if (level >= 25)
                        {
                            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n3_key);
                            Thread.Sleep(1500);
                            if (level >= 90)
                            {
                                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n4_key);
                                Thread.Sleep(1000);
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
        public bool NormalMaterialsMethod(string Names)
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
            PluginLog.Log($"开始采集: {Names}, {coolDown}");
            int action = 0;
            if (name.Contains("之水晶"))
            {
                int id = NormalItems.GetNormalItemId(name);
                int count = BagManager.GetInventoryItemCount((uint)id);
                PluginLog.Log($"开始采集: {Names}, {count}");
                if (count < 8000 && gp >= 200)
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F4_key);
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
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F2_key);
                    gp -= 500;
                    action++;
                    Thread.Sleep(2000);
                }
            }
            else
            {
                if (gp >= 400)
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F1_key);
                    gp -= 400;
                    action++;
                    Thread.Sleep(2000);
                }
            }

            int tt = 0;
            while (CommonUi.AddonGatheringIsOpen() && tt < 15)
            {
                CommonUi.GatheringButton(GatherIndex);
                Thread.Sleep(2000);
                tt++;
                if (tt == 4)
                {
                    gp = player.CurrentGp;
                    level = player.Level;
                    if (gp >= 300 && action > 0)
                    {
                        if (level >= 25)
                        {
                            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n3_key);
                            Thread.Sleep(1500);
                            if (level >= 90)
                            {
                                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n4_key);
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
        public bool LimitMultiMaterialsMethod(string Name)
        {
            PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
            uint gp = player.CurrentGp;
            List<string> list = new();
            list.Add(Name);
            PluginLog.Log($"开始采集: {Name}");
           
            (int GatherIndex, string name) = CommonUi.GetGatheringIndex(list);
            CommonUi.GatheringButton(GatherIndex);
            Thread.Sleep(2000);

            int action = 0;
            if (gp >= 700)
            {
                action++;
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F6_key);
                Thread.Sleep(2000);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F7_key);
                Thread.Sleep(2500);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F6_key);
                Thread.Sleep(2000);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F7_key);
                Thread.Sleep(2500);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F9_key);
                Thread.Sleep(2500);

                if (gp >= 300 && action > 0)
                {
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n3_key);
                    Thread.Sleep(1500);
                    WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.n4_key);
                    Thread.Sleep(1000);
                }
            }
            else {
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F8_key);
                Thread.Sleep(2500);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F8_key);
                Thread.Sleep(2500);
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F8_key);
                Thread.Sleep(2500);
            }
            int tt = 0;
            while (CommonUi.AddonGatheringMasterpieceIsOpen() && tt < 7)
            {
                WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.F5_key);
                Thread.Sleep(2800);
                tt++;
            }
            return true;
        }

        // 刺鱼
        public bool SpearfishMethod() {
            WoAutoCollectionPlugin.GameData.KeyOperates.KeyMethod(Keys.t_key);
            return true;
        }

        public Vector3 MovePositions(Vector3[] Path, bool UseMount)
        {
            ushort SizeFactor = WoAutoCollectionPlugin.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
            Vector3 position = WoAutoCollectionPlugin.GameData.KeyOperates.GetUserPosition(SizeFactor);
            for (int i = 0; i < Path.Length; i++)
            {
                if (closed)
                {
                    PluginLog.Log($"中途结束");
                    return WoAutoCollectionPlugin.GameData.KeyOperates.GetUserPosition(SizeFactor);
                }
                position = WoAutoCollectionPlugin.GameData.KeyOperates.MoveToPoint(position, Path[i], DalamudApi.ClientState.TerritoryType, UseMount, false);
            }
            return position;
        }
    }
}