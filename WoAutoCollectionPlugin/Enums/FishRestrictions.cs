using System;

namespace WoAutoCollectionPlugin.Enums;

[Flags]
public enum FishRestrictions : byte
{
    None           = 0,
    Time           = 1,
    Weather        = 2,
    TimeAndWeather = Time | Weather,
}
