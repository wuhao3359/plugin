using System.Collections.Generic;
using WoAutoCollectionPlugin.Enums;

namespace WoAutoCollectionPlugin.Interfaces;

public interface ILocation : IMarkable, ITeleportable
{
    public uint Id { get; }
    public ObjectType Type { get; }
    public GatheringType GatheringType { get; }
    public IEnumerable<IGatherable> Gatherables { get; }
    public int AetheryteDistance()
        => ClosestAetheryte?.WorldDistance(Territory.Id, IntegralXCoord, IntegralYCoord) ?? int.MaxValue;

}
