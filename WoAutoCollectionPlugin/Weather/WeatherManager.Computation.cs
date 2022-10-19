using Dalamud.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using WoAutoCollectionPlugin.Classes;
using WoAutoCollectionPlugin.Time;

namespace WoAutoCollectionPlugin.Weather;

public partial class WeatherManager
{
    private static TimeStamp GetRootTime(TimeStamp timestamp)
        => timestamp.SyncToEorzeaWeather();

    private static byte CalculateTarget(TimeStamp timestamp)
    {
        var seconds     = timestamp.TotalSeconds;
        var hour        = seconds / EorzeaTimeStampExtensions.SecondsPerEorzeaHour;
        var shiftedHour = (uint)(hour + 8 - hour % 8) % RealTime.HoursPerDay;
        var day         = seconds / EorzeaTimeStampExtensions.SecondsPerEorzeaDay;

        var ret = (uint)day * 100 + shiftedHour;
        ret =  (ret << 11) ^ ret;
        ret =  (ret >> 8) ^ ret;
        ret %= 100;
        return (byte)ret;
    }

    private static Weather GetWeather(byte target, CumulativeWeatherRates rates)
    {
        Debug.Assert(target < 100, "Call error, target weather rate above 100.");
        foreach (var (w, r) in rates.Rates)
        {
            if (r > target)
                return w;
        }

        return rates.Rates[^1].Weather;
    }

    public static WeatherListing[] GetForecast(Territory territory, uint amount, TimeStamp timestamp)
    {
        if (amount == 0)
            return Array.Empty<WeatherListing>();

        if (territory.WeatherRates.Rates.Length == 0)
        {
            PluginLog.Log($"Trying to get forecast for territory {territory.Id} which has no weather rates.");
            return Array.Empty<WeatherListing>();
        }

        var ret  = new WeatherListing[amount];
        var root = GetRootTime(timestamp);
        for (var i = 0; i < amount; ++i)
        {
            var target  = CalculateTarget(root);
            var weather = GetWeather(target, territory.WeatherRates);
            ret[i] =  new WeatherListing(weather, root);
            root   += EorzeaTimeStampExtensions.MillisecondsPerEorzeaWeather;
        }

        return ret;
    }

    public static WeatherListing[] GetForecast(Territory territory, uint amount)
        => GetForecast(territory, amount, WoAutoCollectionPlugin.Time.ServerTime);

    public static WeatherListing GetForecast(Territory territory, TimeStamp timestamp)
        => GetForecast(territory, 1, timestamp)[0];

    public static WeatherListing[] GetForecastOffset(Territory territory, uint amount, long millisecondOffset)
        => GetForecast(territory, amount, WoAutoCollectionPlugin.Time.ServerTime.AddMilliseconds(millisecondOffset));
}
