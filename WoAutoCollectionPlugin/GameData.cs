using Dalamud.Data;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using WoAutoCollectionPlugin.Bot;
using WoAutoCollectionPlugin.Classes;
using WoAutoCollectionPlugin.SeFunctions;

namespace WoAutoCollectionPlugin;

public class GameData
{
    internal DataManager DataManager { get; init; }
    public Dictionary<uint, Weather.Weather> Weathers { get; init; } = new();
    internal Dictionary<byte, CumulativeWeatherRates> CumulativeWeatherRates = new();
    public Territory[] WeatherTerritories { get; init; } = Array.Empty<Territory>();
    public Dictionary<uint, Territory> Territories { get; init; } = new();
    public Dictionary<uint, TerritoryType> TerritoryType { get; init; } = new();
    public Dictionary<uint, Gatherable> Gatherables { get; init; } = new();

    public EventFramework EventFramework { get; private set; } = null!;

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
            Weathers = DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Weather>()!
                .ToDictionary(w => w.RowId, w => new Weather.Weather(w));
            PluginLog.Verbose("Collected {NumWeathers} different Weathers.", Weathers.Count);

            CumulativeWeatherRates = DataManager.GetExcelSheet<WeatherRate>()!
                .ToDictionary(w => (byte)w.RowId, w => new CumulativeWeatherRates(this, w));

            WeatherTerritories = DataManager.GetExcelSheet<TerritoryType>()?
                    .Where(t => t.PCSearch && t.WeatherRate != 0)
                    .Select(FindOrAddTerritory)
                    .Where(t => t != null && t.WeatherRates.Rates.Length > 1)
                    .Cast<Territory>()
                    .GroupBy(t => t.Name)
                    .Select(group => group.First())
                    .OrderBy(t => t.Name)
                    .ToArray()
             ?? Array.Empty<Territory>();
            PluginLog.Verbose("Collected {NumWeatherTerritories} different territories with dynamic weather.", WeatherTerritories.Length);

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

            EventFramework = new EventFramework(DalamudApi.SigScanner);

            KeyOperates = new(this);
            FishBot = new();
            HFishBot = new();
            CollectionFishBot = new();
            GatherBot = new();
            DailyBot = new();
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
