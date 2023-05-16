using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AlphaProject.Enums;
using AlphaProject.Interfaces;
using AlphaProject.Time;
using AlphaProject.Utility;
using GatheringType = AlphaProject.Enums.GatheringType;

namespace AlphaProject.Classes;

public partial class GatheringNode : IComparable<GatheringNode>, ILocation
{
    public NodeType NodeType { get; init; }
    public GatheringPointBase BaseNodeData { get; init; }
    public string Name { get; init; }
    public Vector3[] Markers { get; set; } = Array.Empty<Vector3>();
    public BitfieldUptime Times { get; init; }

    public uint Id
        => BaseNodeData.RowId;

    public IEnumerable<IGatherable> Gatherables
        => Items;

    public ObjectType Type
        => ObjectType.Gatherable;

    public int Level
        => BaseNodeData.GatheringLevel;

    public GatheringType GatheringType
        => (GatheringType)BaseNodeData.GatheringType.Row;

    public bool IsMiner
        => GatheringType.ToGroup() == GatheringType.Miner;

    public bool IsBotanist
        => GatheringType.ToGroup() == GatheringType.Botanist;

    public string Folklore { get; init; }

    public GatheringNode(GameData data, GatheringPointBase node)
    {
    }

    public int CompareTo(GatheringNode? obj)
        => Id.CompareTo(obj?.Id ?? 0);

    private static (BitfieldUptime, NodeType) GetTimes(GatheringPointTransient? row)
    {
        if (row == null)
            return (BitfieldUptime.AllHours, NodeType.Regular);

        // Check for ephemeral nodes
        if (row.GatheringRarePopTimeTable.Row == 0)
        {
            var time = new BitfieldUptime(row.EphemeralStartTime, row.EphemeralEndTime);
            return time.AlwaysUp() ? (time, NodeType.Regular) : (time, NodeType.Ephemeral);
        }
        // and for unspoiled
        else
        {
            var time = new BitfieldUptime(row.GatheringRarePopTimeTable.Value!);
            return time.AlwaysUp() ? (time, NodeType.Regular) : (time, NodeType.Unspoiled);
        }
    }
}
