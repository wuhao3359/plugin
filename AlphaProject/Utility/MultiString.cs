using System;
using Dalamud.Data;
using Lumina.Text;

namespace AlphaProject.Utility;

public readonly struct MultiString
{
    public static string ParseSeStringLumina(SeString? luminaString)
        => luminaString == null ? string.Empty : Dalamud.Game.Text.SeStringHandling.SeString.Parse(luminaString.RawData).TextValue;

    public readonly string ChineseSimplified;

    public string this[ClientLanguage lang]
        => Name(lang);

    public override string ToString()
        => Name(ClientLanguage.ChineseSimplified);

    public string ToWholeString()
        => $"{ChineseSimplified}";

    public MultiString(string zh)
    {
        ChineseSimplified = zh;
    }

    public static MultiString FromPlaceName(DataManager gameData, uint id)
    {
        var zh = ParseSeStringLumina(gameData.GetExcelSheet<Lumina.Excel.GeneratedSheets.PlaceName>(ClientLanguage.ChineseSimplified)!.GetRow(id)?.Name);
        return new MultiString(zh);
    }

    public static MultiString FromItem(DataManager gameData, uint id)
    {
        var zh = ParseSeStringLumina(gameData.GetExcelSheet<Lumina.Excel.GeneratedSheets.Item>(ClientLanguage.ChineseSimplified)!.GetRow(id)?.Name);
        return new MultiString(zh);
    }

    private string Name(ClientLanguage lang)
        => lang switch
        {
            ClientLanguage.ChineseSimplified => ChineseSimplified,
            _ => throw new ArgumentException(),
        };

    public static readonly MultiString Empty = new(string.Empty);
}
