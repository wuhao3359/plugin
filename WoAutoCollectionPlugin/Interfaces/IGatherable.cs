using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Interfaces;

public enum ObjectType : byte
{
    Invalid,
    Gatherable,
    Fish,
}

public interface IGatherable
{
    public MultiString Name { get; }
    public IEnumerable<ILocation> Locations { get; }
    public int InternalLocationId { get; }
    public uint ItemId { get; }
    public Item ItemData { get; }
    public ObjectType Type { get; }
}
