using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System.Threading;

namespace AlphaProject.SeFunctions
{
    public static unsafe class Teleporter
    {
        public static int count = 0;

        public static int retry = 0;
        public static bool Teleport(uint aetheryte)
        {
            return Teleport(aetheryte, 0);
        }

        public static bool Teleport(uint aetheryte, int r)
        {
            retry = r;
            bool flag = true;
            int ii = 0;
            while (DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing] || DalamudApi.Condition[ConditionFlag.Casting])
            {
                PluginLog.Log($"当前状态无法TP, 等待1s...");
                Thread.Sleep(1000);
                if (ii > 10)
                {
                    break;
                }
                ii++;
            }
            PluginLog.Log($"开始传送, 累计次数: {count}");
            Telepo.Instance()->Teleport(aetheryte, 0);
            Thread.Sleep(2500);
            count++;
            if (!DalamudApi.Condition[ConditionFlag.Casting])
            {
                PluginLog.Log($"传送失败, 重试一次");
                flag = false;
                if (retry < 1)
                {
                    retry++;
                    Thread.Sleep(1000);
                    Teleport(aetheryte, 1);
                }
            }
            Thread.Sleep(15000);
            return flag;
        }
    }
}
