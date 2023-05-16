﻿using System;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Action = System.Action;

namespace AlphaProject.Time;

public class SeTime
{
    private static TimeStamp GetServerTime()
        => new(Framework.GetServerTime() * 1000);

    public TimeStamp ServerTime         { get; private set; }
    public TimeStamp EorzeaTime         { get; private set; }
    public long      EorzeaTotalMinute  { get; private set; }
    public long      EorzeaTotalHour    { get; private set; }
    public short     EorzeaMinuteOfDay  { get; private set; }
    public byte      EorzeaHourOfDay    { get; private set; }
    public byte      EorzeaMinuteOfHour { get; private set; }

    //public event Action? Updated;


    public SeTime()
    {
    }

    public void Dispose() { 
    }

    private unsafe TimeStamp GetEorzeaTime()
    {
        var framework = Framework.Instance();
        if (framework == null)
            return ServerTime.ConvertToEorzea();

        return Math.Abs(new TimeStamp(framework->ServerTime * 1000) - ServerTime) < 5000
            ? new TimeStamp(framework->EorzeaTime * 1000)
            : ServerTime.ConvertToEorzea();
    }

    public unsafe void Update()
    {
        ServerTime = GetServerTime();
        EorzeaTime = GetEorzeaTime();
        //var minute = EorzeaTime.TotalMinutes;
        //if (minute != EorzeaTotalMinute)
        //{
        //    EorzeaTotalMinute  = minute;
        //    EorzeaMinuteOfDay  = (short)(EorzeaTotalMinute % RealTime.MinutesPerDay);
        //    EorzeaMinuteOfHour = (byte)(EorzeaMinuteOfDay % RealTime.MinutesPerHour);
        //}

        //var hour = EorzeaTotalMinute / RealTime.MinutesPerHour;
        //if (hour != EorzeaTotalHour)
        //{
        //    // Sometimes the Eorzea time gets seemingly rounded up and triggers before the ServerTime.
        //    //ServerTime      = ServerTime.AddEorzeaMinutes(30).SyncToEorzeaHour();
        //    EorzeaTotalHour = hour;
        //    EorzeaHourOfDay = (byte)(EorzeaMinuteOfDay / RealTime.MinutesPerHour);
        //    HourChanged?.Invoke();
        //    if ((EorzeaHourOfDay & 0b111) == 0)
        //    {
        //        PluginLog.Log("Eorzea Hour and Weather Change triggered. {ServerTime} {EorzeaTime}", (long)ServerTime, (long)EorzeaTime);
        //        WeatherChanged?.Invoke();
        //    }
        //    else
        //    {
        //        PluginLog.Log("Eorzea Hour Change triggered. {ServerTime} {EorzeaTime}", (long)ServerTime, (long)EorzeaTime);
        //    }
        //}

        //Updated?.Invoke();
    }
}
