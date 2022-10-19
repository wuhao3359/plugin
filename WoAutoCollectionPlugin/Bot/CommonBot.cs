using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Resolvers;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
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
        private GameData GameData { get; init; }
        private KeyOperates KeyOperates { get; init; }

        private bool closed = false;

        public CommonBot(KeyOperates KeyOperates)
        {
            this.KeyOperates = KeyOperates;
            Init();
        }

        public CommonBot(KeyOperates KeyOperates, GameData GameData)
        {
            this.KeyOperates = KeyOperates;
            this.GameData = GameData;
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

        public void RepairAndExtractMateria() {
            // 判断是否需要修理
            if (RepairUi.NeedsRepair())
            {
                Repair();
            }

            // 判断是否需要精制
            int count = RepairUi.CanExtractMateria();
            if (count >= 5)
            {
                ExtractMateria(count);
            }

            int n = 0;
            while (RepairUi.AddonRepairIsOpen() && n < 3) {
                KeyOperates.KeyMethod(Keys.esc_key);
            }
        }

        // 修理
        public bool Repair() {
            int n = 0;
            while (DalamudApi.Condition[ConditionFlag.Mounted])
            {
                if (n >= 3)
                {
                    KeyOperates.KeyMethod(Keys.w_key, 200);
                }
                KeyOperates.KeyMethod(Keys.q_key);
                Thread.Sleep(1000);
                n++;

                if (closed)
                {
                    PluginLog.Log($"YGathing stopping");
                    return true;
                }
            }
            bool flag = true;
            if (closed)
            {
                PluginLog.Log($"Repair stopping");
                return flag;
            }
            KeyOperates.KeyMethod(Keys.F12_key);
            Thread.Sleep(1000);
            if (RepairUi.AllRepairButton())
            {
                Thread.Sleep(800);
                CommonUi.SelectYesButton();
                Thread.Sleep(3500);
            }
            else {
                flag = false;
            }
            KeyOperates.KeyMethod(Keys.esc_key);
            Thread.Sleep(500);
            return flag;
        }

        public bool NpcRepair()
        {
            int n = 0;
            while (DalamudApi.Condition[ConditionFlag.Mounted])
            {
                if (n >= 3)
                {
                    KeyOperates.KeyMethod(Keys.q_key, 200);
                }
                KeyOperates.KeyMethod(Keys.q_key);
                Thread.Sleep(1000);
                n++;

                if (closed)
                {
                    PluginLog.Log($"YGathing stopping");
                    return true;
                }
            }
            bool flag = true;
            SetTarget("修理工");
            Thread.Sleep(1200);
            if (CommonUi.AddonSelectIconStringIsOpen())
            {
                CommonUi.SelectIconString2Button();
                Thread.Sleep(1500);
            }
            
            if (RepairUi.AddonRepairIsOpen() && RepairUi.AllRepairButton())
            {
                Thread.Sleep(800);
                CommonUi.SelectYesButton();
                Thread.Sleep(800);
            }
            else
            {
                flag = false;
            }
            KeyOperates.KeyMethod(Keys.esc_key);
            Thread.Sleep(500);
            return flag;
        }

        // 精制
        public bool ExtractMateria(int count)
        {
            int n = 0;
            while (DalamudApi.Condition[ConditionFlag.Mounted])
            {
                if (n >= 3)
                {
                    KeyOperates.KeyMethod(Keys.q_key, 200);
                }
                KeyOperates.KeyMethod(Keys.q_key);
                Thread.Sleep(1000);
                n++;

                if (closed)
                {
                    PluginLog.Log($"YGathing stopping");
                    return true;
                }
            }
            if (closed)
            {
                PluginLog.Log($"ExtractMateria stopping");
                return true;
            }
            KeyOperates.KeyMethod(Keys.F11_key);
            Thread.Sleep(1000);
            for (int i = 0; i < count; i++) {
                KeyOperates.KeyMethod(Keys.num0_key);
                Thread.Sleep(1000);
                CommonUi.SelectMaterializeDialogYesButton();
                Thread.Sleep(3500);
            }
            KeyOperates.KeyMethod(Keys.esc_key);
            Thread.Sleep(500);
            return true;
        }

        public bool CraftUploadAndExchange(string craftName, int exchangeItem)
        {
            (uint Category, uint Sub, uint ItemId) = Items.UploadApply(craftName);
            while (BagManager.GetInventoryItemCount(ItemId) > 0) {
                if (closed)
                {
                    PluginLog.Log($"CraftUploadAndExchange stopping");
                    return true;
                }
                CraftUpload(Category, Sub, ItemId);
                CraftExchange(exchangeItem);
            }
            return true;
        }

        // TODO 关闭界面
        // 交收藏品
        public bool CraftUpload(uint Category, uint Sub, uint ItemId)
        {
            PluginLog.Log($"CraftUploading");
            if (Category == 0 && Sub == 0) {
                return false;
            }

            Thread.Sleep(1000);
            SetTarget("收藏品交易员");
            Thread.Sleep(2500);
            for (int i = 1; i < Category; i++) {
                KeyOperates.KeyMethod(Keys.num2_key);
            }

            for (int i = 0; i < Sub; i++)
            {
                KeyOperates.KeyMethod(Keys.num2_key);
            }

            int n = 0;
            while (!RecipeNoteUi.SelectYesnoIsOpen() && n < 10 && BagManager.GetInventoryItemCount(ItemId) > 0)
            {
                KeyOperates.KeyMethod(Keys.num0_key);
                Thread.Sleep(500);
                n++;
                if (closed)
                {
                    PluginLog.Log($"upload stopping");
                    return true;
                }
            }

            KeyOperates.KeyMethod(Keys.num0_key);
            KeyOperates.KeyMethod(Keys.esc_key);
            Thread.Sleep(1000);
            return true;
        }

        // TODO 关闭界面
        // 交换道具
        public bool CraftExchange(int item)
        {
            PluginLog.Log($"CraftExchanging");
            (uint Category, uint Sub) = Items.ExchangeApply(item);
            if (Category == 0 && Sub == 0)
            {
                return false;
            }

            Thread.Sleep(2000);
            SetTarget("工票交易员");
            Thread.Sleep(500);
            KeyOperates.KeyMethod(Keys.num0_key);
            KeyOperates.KeyMethod(Keys.num0_key);
            Thread.Sleep(2000);

            if (closed)
            {
                PluginLog.Log($"exchange stopping");
                return true;
            }

            if (item == 1) {
                KeyOperates.KeyMethod(Keys.num0_key);
                KeyOperates.KeyMethod(Keys.num8_key);
                KeyOperates.KeyMethod(Keys.num0_key);

                for (int i = 0; i < Sub; i++)
                {
                    KeyOperates.KeyMethod(Keys.num2_key);
                }
            }

            KeyOperates.KeyMethod(Keys.num6_key);
            KeyOperates.KeyMethod(Keys.num9_key);
            KeyOperates.KeyMethod(Keys.num4_key);
            KeyOperates.KeyMethod(Keys.num0_key);
            KeyOperates.KeyMethod(Keys.num4_key);
            KeyOperates.KeyMethod(Keys.num0_key);

            KeyOperates.KeyMethod(Keys.esc_key);
            Thread.Sleep(1000);
            return true;
        }

        public bool SetTarget(string targetName) {
            var target = DalamudApi.ObjectTable.FirstOrDefault(obj => obj.Name.TextValue.ToLowerInvariant() == targetName);
            if (target == default) {
                return false;
            }

            DalamudApi.TargetManager.SetTarget(target);
            Thread.Sleep(200);
            KeyOperates.KeyMethod(Keys.num0_key);
            Thread.Sleep(800);
            return true;
        }

        // 限时材料采集手法
        public bool LimitMaterialsMethod(string Names, string job) {
            // lv.74 4589-采矿 大地的恩惠 4590-园艺 大地的恩惠
            uint GivingLandActionId = 4589;
            if (job == "园艺工")
            {
                GivingLandActionId = 4590;
            }
            PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
            uint gp = player.CurrentGp;
            int level = player.Level;

            List<string> list = new();
            string[] names = Names.Split('|');
            foreach (string na in names) {
                if (na.Contains("雷之") || na.Contains("火之") || na.Contains("风之") || na.Contains("水之") || na.Contains("冰之") || na.Contains("土之"))
                {
                    if (level >= 74 && Game.GetSpellActionRecastTimeElapsed(GivingLandActionId) == 0 && gp >= 200)
                    {
                        list.Insert(0, na);
                        PluginLog.Log($"{na}优先...");
                    }
                }
                else {
                    list.Add(na);
                }
            }

            (int GatherIndex, string name) = CommonUi.GetGatheringIndex(list, GameData);
            PluginLog.Log($"开始采集: {name}");
            if (name.Contains("雷之") || name.Contains("火之") || name.Contains("风之") || name.Contains("水之") || name.Contains("冰之") || name.Contains("土之")) {
                if (gp >= 200)
                {
                    KeyOperates.KeyMethod(Keys.F4_key);
                    gp -= 200;
                }
                else if(gp >= 150) {
                    KeyOperates.KeyMethod(Keys.F3_key);
                    gp -= 150;
                }
            } else {
                if (level >= 50)
                {
                    if (gp >= 500)
                    {
                        KeyOperates.KeyMethod(Keys.F2_key);
                        gp -= 500;
                        Thread.Sleep(2000);
                    }
                }
                else
                {
                    if (gp >= 400)
                    {
                        KeyOperates.KeyMethod(Keys.F1_key);
                        gp -= 400;
                        Thread.Sleep(2000);
                    }
                }
            }

            int tt = 0;
            while (CommonUi.AddonGatheringIsOpen() && tt < 12)
            {
                CommonUi.GatheringButton(GatherIndex);
                Thread.Sleep(2000);
                tt++;
                if (tt == 4)
                {
                    gp = player.CurrentGp;
                    level = player.Level;
                    if (gp >= 300)
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
            }

            return true;
        }
    }
}