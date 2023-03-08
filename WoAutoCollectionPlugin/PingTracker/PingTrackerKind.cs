using System;

namespace WoAutoCollectionPlugin.PingTrackers
{
    public enum PingTrackerKind
    {
        Packets,
    }

    public static class PingTrackerKindExtensions
    {
        public static string FormatName(this PingTrackerKind kind)
        {
            return kind switch
            {
                PingTrackerKind.Packets => "Game packets",
                _ => throw new ArgumentOutOfRangeException(nameof(kind)),
            };
        }
    }
}