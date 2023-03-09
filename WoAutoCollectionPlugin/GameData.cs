using Dalamud.Data;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using WoAutoCollectionPlugin.Bot;
using WoAutoCollectionPlugin.Classes;
using WoAutoCollectionPlugin.SeFunctions;
using Fish = WoAutoCollectionPlugin.Classes.Fish;
using FishingSpot = WoAutoCollectionPlugin.Classes.FishingSpot;

namespace WoAutoCollectionPlugin;

public class GameData
{
    internal DataManager DataManager { get; init; }
    public Dictionary<uint, Territory> Territories { get; init; } = new();
    public Dictionary<uint, TerritoryType> TerritoryType { get; init; } = new();
    public Dictionary<uint, Gatherable> Gatherables { get; init; } = new();
    public Dictionary<uint, Fish> Fishes { get; init; } = new();
    public Dictionary<uint, FishingSpot> FishingSpots { get; init; } = new();
    public Dictionary<uint, Status> Status { get; init; } = new();

    // TODO
    public Dictionary<uint, Recipe> Recipes { get; init; } = new();

    public EventFramework EventFramework { get; private set; } = null!;

    public MarketBot MarketBot { get; init; } = null!;
    public DailyBot DailyBot { get; init; } = null!;
    public FishBot FishBot { get; init; } = null!;
    public HFishBot HFishBot { get; init; } = null!;
    public CollectionFishBot CollectionFishBot { get; init; } = null!;
    public GatherBot GatherBot { get; init; } = null!;
    public CraftBot CraftBot { get; init; } = null!;
    public CommonBot CommonBot { get; init; } = null!;
    public KeyOperates KeyOperates { get; init; } = null!;

    public bool closed = false;

    public bool othetRun = false;

    public Dictionary<string, string> param = new();
    public GameData(DataManager gameData)
    {
        DataManager = gameData;
        try
        {
            TerritoryType = DalamudApi.DataManager.GetExcelSheet<TerritoryType>()?
                    .Where(a => a.RowId > 1)
                    .ToDictionary(a => a.RowId, a => a)
            ?? new Dictionary<uint, TerritoryType>();
            PluginLog.Log("Collected {NumAetherytes} different TerritoryType.", TerritoryType.Count);

            Gatherables = DataManager.GetExcelSheet<GatheringItem>()?
                    .Where(g => g.Item != 0 && g.Item < 1000000)
                    .GroupBy(g => g.Item)
                    .Select(group => group.First())
                    .ToDictionary(g => (uint)g.Item, g => new Gatherable(this, g))
             ?? new Dictionary<uint, Gatherable>();
            PluginLog.Log("Collected {NumGatherables} different gatherable items.", Gatherables.Count);

            Status = DataManager.GetExcelSheet<Status>()?
                .Where(a => a.RowId > 1)
                .ToDictionary(a => a.RowId, a => a)
                ?? new Dictionary<uint, Status>();
            PluginLog.Log("Collected {NumStatus} different Status.", Status.Count);

            //Recipes = DalamudApi.DataManager.GetExcelSheet<Recipe>()?
            //        .Where(a => a.RowId > 1)
            //        .ToDictionary(a => a.ItemResult.RawRow.RowId, a => a)
            //?? new Dictionary<uint, Recipe>();
            //PluginLog.Log("Collected {NumGatherables} different recipes items.", Recipes.Count);

            Fishes = DataManager.GetExcelSheet<FishParameter>()?
                .Where(f => f.Item != 0 && f.Item < 1000000)
                .Select(f => new Fish(DataManager, f))
                .Concat(DataManager.GetExcelSheet<SpearfishingItem>()?
                .Where(sf => sf.Item.Row != 0 && sf.Item.Row < 1000000)
                .Select(sf => new Fish(DataManager, sf))
                ?? Array.Empty<Fish>())
                .GroupBy(f => f.ItemId)
                .Select(group => group.First())
                .ToDictionary(f => f.ItemId, f => f)
                ?? new Dictionary<uint, Fish>();
            PluginLog.Log("Collected {NumFishes} different types of fish.", Fishes.Count);
            Data.Fish.Apply(this);

            FishingSpots = DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.FishingSpot>()?
                .Where(f => f.PlaceName.Row != 0 && (f.TerritoryType.Row > 0 || f.RowId == 10000 || f.RowId >= 10017))
                .Select(f => new FishingSpot(this, f))
                .Concat(
                    DataManager.GetExcelSheet<SpearfishingNotebook>()?
                        .Where(sf => sf.PlaceName.Row != 0 && sf.TerritoryType.Row > 0)
                        .Select(sf => new FishingSpot(this, sf))
                 ?? Array.Empty<FishingSpot>())
                .Where(f => f.Territory.Id != 0)
                .ToDictionary(f => f.Id, f => f)
            ?? new Dictionary<uint, FishingSpot>();
            PluginLog.Log("Collected {NumFishingSpots} different fishing spots.", FishingSpots.Count);

            EventFramework = new EventFramework(DalamudApi.SigScanner);

            KeyOperates = new(this);
            FishBot = new();
            HFishBot = new();
            CollectionFishBot = new();
            GatherBot = new();
            DailyBot = new();
            MarketBot = new();
            CraftBot = new();
            CommonBot = new();
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
