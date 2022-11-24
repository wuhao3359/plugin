using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace WoAutoCollectionPlugin.SeFunctions
{
    public static unsafe class Teleporter
    {
        public static int count = 0;
        public static bool Teleport(uint aetheryte)
        {

            Telepo.Instance()->Teleport(aetheryte, 0);
            count++;
            PluginLog.Log($"传送累计次数: {count}");
            return true;

        }
    }
}
