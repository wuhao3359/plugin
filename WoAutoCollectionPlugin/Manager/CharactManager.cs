
using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace WoAutoCollectionPlugin.Managers;


internal class CharactManager
{
    public CharactManager()
    {
    }

    public static unsafe Character GetBattleCharaByObjectId(int objectId) {
        var cm = CharacterManager.Instance()->LookupBattleCharaByObjectId(objectId);
        return cm->Character;
    }

}
