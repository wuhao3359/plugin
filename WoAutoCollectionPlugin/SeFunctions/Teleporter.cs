using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace WoAutoCollectionPlugin.SeFunctions
{
    public static unsafe class Teleporter
    {
        public static bool Teleport(uint aetheryte)
        {

            Telepo.Instance()->Teleport(aetheryte, 0);
            return true;

        }
    }
}
