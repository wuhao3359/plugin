using Dalamud.Data;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using WoAutoCollectionPlugin.Classes;
using Aetheryte = WoAutoCollectionPlugin.Classes.Aetheryte;

namespace WoAutoCollectionPlugin;

public class GameData
{
    public Dictionary<uint, Territory> Territories { get; init; } = new();
    public Dictionary<uint, Aetheryte> Aetherytes { get; init; } = new();
    public Dictionary<uint, Gatherable> Gatherables { get; init; } = new();
    public Dictionary<uint, Gatherable> GatherablesByGatherId { get; init; } = new();
    public Dictionary<uint, GatheringNode> GatheringNodes { get; init; } = new();
    public Dictionary<uint, TerritoryType> TerritoryType { get; init; } = new();

    public GameData(DataManager gameData)
    {
        try
        {
            Aetherytes = DalamudApi.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Aetheryte>()?
                    .Where(a => a.IsAetheryte && a.RowId > 1 && a.PlaceName.Row != 0)
                    .ToDictionary(a => a.RowId, a => new Aetheryte(this, a))
             ?? new Dictionary<uint, Aetheryte>();
            PluginLog.Verbose("Collected {NumAetherytes} different aetherytes.", Aetherytes.Count);

            TerritoryType = DalamudApi.DataManager.GetExcelSheet<TerritoryType>()?
                    .Where(a => a.RowId > 1)
                    .ToDictionary(a => a.RowId, a => a)
            ?? new Dictionary<uint, TerritoryType>();
            PluginLog.Verbose("Collected {NumAetherytes} different TerritoryType.", TerritoryType.Count);

            Gatherables = new Dictionary<uint, Gatherable>();
            //Gatherables = DalamudApi.DataManager.GetExcelSheet<GatheringItem>()?
            //        .Where(g => g.Item != 0 && g.Item < 1000000)
            //        .GroupBy(g => g.Item)
            //        .Select(group => group.First())
            //        .ToDictionary(g => (uint)g.Item, g => new Gatherable(this, g))
            // ?? new Dictionary<uint, Gatherable>();
            //GatherablesByGatherId = Gatherables.Values.ToDictionary(g => g.GatheringId, g => g);
            //PluginLog.Verbose("Collected {NumGatherables} different gatherable items.", Gatherables.Count);
            //ForcedAetherytes.ApplyMissingAetherytes(this);

            GatheringNodes = new Dictionary<uint, GatheringNode>();
            //GatheringNodes = DalamudApi.DataManager.GetExcelSheet<GatheringPointBase>()?
            //        .Where(b => b.GatheringType.Row < (int)Enums.GatheringType.Spearfishing)
            //        .Select(b => new GatheringNode(this, b))
            //        .Where(n => n.Territory.Id > 1 && n.Items.Count > 0)
            //        .ToDictionary(n => n.Id, n => n)
            // ?? new Dictionary<uint, GatheringNode>();
            //PluginLog.Verbose("Collected {NumGatheringNodes} different gathering nodes", GatheringNodes.Count);

            //foreach (var gatherable in Gatherables.Values)
            //{
            //    if (gatherable.NodeType != NodeType.Unknown && !gatherable.NodeList.Any(n => n.Times.AlwaysUp()))
            //        gatherable.InternalLocationId = ++TimedGatherables;
            //    else if (gatherable.NodeList.Count > 1)
            //        gatherable.InternalLocationId = -++MultiNodeGatherables;
            //    GatherablesTrie.Add(gatherable.Name[gameData.Language].ToLowerInvariant(), gatherable);
            //}
        }
        catch (Exception e)
        {
            PluginLog.Error($"Error while setting up data:\n{e}");
        }
    }

    public Territory? FindOrAddTerritory(TerritoryType? t)
    {
        if (t == null || t.RowId < 2)
            return null;

        if (Territories.TryGetValue(t.RowId, out var territory))
            return territory;

        // Create territory if it does not exist.
        var aether = DalamudApi.DataManager.GetExcelSheet<TerritoryTypeTelepo>()?.GetRow(t.RowId);
        territory = new Territory(this, t, aether);
        Territories.Add(t.RowId, territory);
        return territory;
    }

    public ushort GetSizeFactor(ushort tt)
    {
        if (TerritoryType.TryGetValue(tt, out var type))
        {
            Map? value = type.Map.Value;
            if (value != null)
                return value.SizeFactor;
        }
        return 0;
    }
}
