using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System;
using System.Threading;

namespace AlphaProject.SeFunctions
{
    public static unsafe class Teleporter
    {
        public static int Count = 0;

        public static int Retry = 0;
        public static bool Teleport(uint aetheryte)
        {
            return Teleport(aetheryte, 0);
        }

        public static bool Teleport(uint aetheryte, int r)
        {
            if (!DalamudApi.ClientState.IsLoggedIn)
            {
                return false;
            }
            Retry = r;
            bool flag = true;
            int ii = 0;
            while (DalamudApi.Condition[ConditionFlag.Gathering] 
                || DalamudApi.Condition[ConditionFlag.Fishing] 
                || DalamudApi.Condition[ConditionFlag.Casting]
                || DalamudApi.Condition[ConditionFlag.Crafting]
                || DalamudApi.Condition[ConditionFlag.PreparingToCraft]
                || DalamudApi.Condition[ConditionFlag.Crafting40])
            {
                PluginLog.Log($"当前状态无法TP, 等待一次...");
                Thread.Sleep(1000 + new Random().Next(500, 2000));
                if (ii > 20)
                {
                    break;
                }
                ii++;
            }
            PluginLog.Log($"开始传送, 累计次数: {Count}");
            Telepo.Instance()->Teleport(aetheryte, 0);
            Thread.Sleep(2500);
            Count++;
            if (!DalamudApi.Condition[ConditionFlag.Casting])
            {
                PluginLog.Log($"传送失败, 重试一次");
                flag = false;
                if (Retry < 1)
                {
                    Thread.Sleep(500 + new Random().Next(500, 1500));
                    Teleport(aetheryte, 1);
                }
            }
            Thread.Sleep(12000 + new Random().Next(2000, 3000));
            return flag;
        }
    }
}
