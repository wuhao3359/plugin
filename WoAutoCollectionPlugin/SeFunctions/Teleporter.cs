using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System.Threading;

namespace WoAutoCollectionPlugin.SeFunctions
{
    public static unsafe class Teleporter
    {
        public static int count = 0;
        public static bool Teleport(uint aetheryte)
        {
            int ii = 0;
            while (DalamudApi.Condition[ConditionFlag.Gathering] || DalamudApi.Condition[ConditionFlag.Fishing])
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
            count++;
            Thread.Sleep(12000);
            return true;
        }
    }
}
