﻿using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using System.Linq;
using System.Threading;
using WoAutoCollectionPlugin.Data;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class CommonBot
    {
        private KeyOperates KeyOperates { get; init; }

        private bool closed = false;

        public CommonBot(KeyOperates KeyOperates)
        {
            this.KeyOperates = KeyOperates;
            Init();
        }

        public void Init()
        {
            closed = false;
        }

        public void Closed()
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
            KeyOperates.KeyMethod(Keys.num0_key);
            Thread.Sleep(1000);
            if (CommonUi.AddonSelectStringIsOpen())
            {
                Thread.Sleep(500);
                CommonUi.SelectString1Button();
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
            if (closed)
            {
                PluginLog.Log($"CraftUploadAndExchange stopping");
                return true;
            }
            if (CraftUpload(craftName) && CraftExchange(exchangeItem)) {
                return true;
            }
            return false;
        }

        // TODO 关闭界面
        // 交收藏品
        public bool CraftUpload(string craftName)
        {
            PluginLog.Log($"CraftUploading");
            (uint Category, uint Sub) = Items.UploadApply(craftName);
            if (Category == 0 && Sub == 0) {
                return false;
            }

            Thread.Sleep(1000);
            KeyOperates.KeyMethod(Keys.num1_key);
            Thread.Sleep(500);
            KeyOperates.KeyMethod(Keys.num0_key);
            Thread.Sleep(3000);
            for (int i = 0; i < Category; i++) {
                KeyOperates.KeyMethod(Keys.num2_key);
            }

            for (int i = 0; i < Sub; i++)
            {
                KeyOperates.KeyMethod(Keys.num2_key);
            }

            int n = 0;
            while (!RecipeNoteUi.SelectYesnoIsOpen() && n < 15)
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
            KeyOperates.KeyMethod(Keys.num3_key);
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
            KeyOperates.KeyMethod(Keys.num0_key);
            return true;
        }
    }
}