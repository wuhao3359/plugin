using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using AlphaProject.Enums;
using AlphaProject.Interfaces;
using AlphaProject.Utility;
using GatheringType = AlphaProject.Enums.GatheringType;

namespace AlphaProject.Classes;

public class Gatherable : IComparable<Gatherable>, IGatherable
{
    public Item ItemData { get; }
    public GatheringItem GatheringData { get; }
    public MultiString Name { get; }
    public IList<GatheringNode> NodeList { get; } = new List<GatheringNode>();

    public int InternalLocationId { get; internal set; } = 0;

    public IEnumerable<ILocation> Locations
        => NodeList;

    public uint ItemId
        => ItemData.RowId;

    public ObjectType Type
        => ObjectType.Gatherable;

    public uint GatheringId
        => GatheringData.RowId;

    public NodeType NodeType { get; internal set; } = NodeType.Unknown;
    public GatheringType GatheringType { get; internal set; } = GatheringType.Unknown;

    public uint ExpansionIdx { get; internal set; } = uint.MaxValue;

    public Gatherable(GameData gameData, GatheringItem gatheringData)
    {
        GatheringData = gatheringData;
        var itemSheet = gameData.DataManager.GetExcelSheet<Item>();
        ItemData = itemSheet?.GetRow((uint)gatheringData.Item) ?? new Item();
        if (ItemData.RowId == 0)
            PluginLog.Error("Invalid item.");

        var levelData = gatheringData.GatheringItemLevel?.Value;
        _levelStars = levelData == null ? 0 : (levelData.GatheringItemLevel << 3) + levelData.Stars;
        Name = MultiString.FromItem(gameData.DataManager, (uint)gatheringData.Item);
    }

    public int Level
        => _levelStars >> 3;

    public int Stars
        => _levelStars & 0b111;

    public string StarsString()
        => StarsArray[Stars];

    public string LevelString()
        => $"{Level}{StarsString()}";

    public override string ToString()
        => $"{Name} ({Level}{StarsString()})";

    public int CompareTo(Gatherable? rhs)
        => ItemId.CompareTo(rhs?.ItemId ?? 0);

    private readonly int _levelStars;

    private static readonly string[] StarsArray =
    {
        "",
        "*",
        "**",
        "***",
        "****",
    };
}
