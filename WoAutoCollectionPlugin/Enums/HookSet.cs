﻿namespace WoAutoCollectionPlugin.Enums
{
    public enum HookSet : byte
    {
        Unknown = 0,
        Precise = 1,
        Powerful = 2,
        Hook = 3,
        DoubleHook = 4,
        TripleHook = 5,
        None = 255,
    }

    public static class HookSetExtensions
    {
        public static string ToName(this HookSet value)
            => value switch
            {
                HookSet.Unknown => "Unknown",
                HookSet.Precise => "Precise",
                HookSet.Powerful => "Powerful",
                HookSet.Hook => "Regular",
                HookSet.DoubleHook => "Double",
                HookSet.TripleHook => "Triple",
                HookSet.None => "None",
                _ => "Invalid",
            };
    }
}
