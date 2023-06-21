using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using AlphaProject;
using AlphaProject.Utility;
using System.Collections.Generic;

public static class KeyOperates
{
    private static GameData GameData;
    private static IntPtr Hwnd;

    private static bool Closed = false;

    public static Dictionary<Byte, DateTime> KeyTimes = new();

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

    public static void Initialize(GameData gameData)
    {
        GameData = gameData;
        Process pro = Process.GetCurrentProcess();
        Hwnd = Process.GetProcessById(pro.Id).MainWindowHandle;
    }

    public static Vector3 GetUserPosition(ushort SizeFactor)
    {
        Vector3 playerPosition = DalamudApi.ClientState.LocalPlayer.Position;
        float x = Maths.GetCoordinate(playerPosition.X, SizeFactor);
        float y = Maths.GetCoordinate(playerPosition.Y, SizeFactor);
        float z = Maths.GetCoordinate(playerPosition.Z, SizeFactor);
        Vector3 position = new(x, y, z);
        return position;
    }

    public static Vector3 MoveToPoint(Vector3 positionA, Vector3 positionB, ushort territoryType)
    {
        return MoveToPoint(positionA, positionB, territoryType, false, true);
    }

    public static Vector3 MoveToPoint(Vector3 positionA, Vector3 positionB, ushort territoryType, bool UseMount)
    {
        return MoveToPoint(positionA, positionB, territoryType, UseMount, true);
    }

    public static Vector3 MoveToPoint(Vector3 positionA, Vector3 positionB, ushort territoryType, bool UseMount, bool log)
    {
        Init();

        int notMove = 0;

        int n = 0;
        while (UseMount && !DalamudApi.Condition[ConditionFlag.Mounted] && n < 3) {
            KeyMethod(Keys.q_key);
            Thread.Sleep(2300 + new Random().Next(200, 500));
            n++;
        }

        double errorDisntance = 3.8;
        ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);

        positionA = ReviseNoTime(positionB);
        double distance = Maths.Distance(positionA, positionB);

        int index = 0;
        KeyDown(Keys.w_key);
        double height = Maths.Height(positionA, positionB, UseMount);

        while (distance > errorDisntance && index < 2400)
        {
            if (Closed || territoryType != DalamudApi.ClientState.TerritoryType || !Tasks.TaskRun)
            {
                PluginLog.Log($"移动中途结束 {Closed} {territoryType} {DalamudApi.ClientState.TerritoryType}");
                Stop();
                break;
            }
            Thread.Sleep(10);

            Vector3 positionC = GetUserPosition(SizeFactor);
            double DirectionOfPoint = Maths.DirectionOfPoint(positionA, positionB, positionC);

            // 根据相对高度 上升或下降
            double beforeHeight = height;
            height = Maths.Height(positionC, positionB, UseMount);
            if (height < -2)
            {
                if (DalamudApi.KeyState[Keys.num_sub_key] && (DalamudApi.Condition[ConditionFlag.InFlight] || DalamudApi.Condition[ConditionFlag.Diving]))
                {
                    KeyUp(Keys.num_sub_key);
                }
                if (!DalamudApi.KeyState[Keys.space_key] || !DalamudApi.Condition[ConditionFlag.InFlight])
                {
                    KeyDown(Keys.space_key);
                }
            }
            else if (height > 5)
            {
                if (DalamudApi.KeyState[Keys.space_key])
                {
                    KeyUp(Keys.space_key);
                }
                if (!DalamudApi.KeyState[Keys.num_sub_key] && (DalamudApi.Condition[ConditionFlag.InFlight] || DalamudApi.Condition[ConditionFlag.Diving]))
                {
                    KeyDown(Keys.num_sub_key);
                }
            }
            else
            {
                FlyStop();
            }

            double radians = Maths.Radians(positionA, positionB, positionC);
            double beforeDistance = distance;
            positionA = GetUserPosition(SizeFactor);
            distance = Maths.Distance(positionA, positionB);
            if (Math.Abs(beforeDistance - distance) < 0.05)
            {
                notMove++;
            }
            else
            {
                if (notMove > 0)
                {
                    notMove--;
                }
            }

            if (notMove >= 20)
            {
                KeyMethod(Keys.d_key, 800);
                if (!DalamudApi.KeyState[Keys.w_key])
                {
                    KeyDown(Keys.w_key);
                }
                KeyMethod(Keys.space_key, 1500);
                notMove = 0;
            }

            double angle = 0;
            if (radians > -1 && radians < 1)
            {
                angle = Maths.Angle(radians);
            }
            else if (radians >= 1)
            {
                angle = 180;
            }
            // 旋转角度速度 100毫秒 35度左右 6.3 = 360 100=0.62
            int time = Convert.ToInt32(angle / 33 * 100);
            //PluginLog.Log($"distance: {distance} angle: {angle} time: {time}");
            if (time > 60)
            {
                if (time > 200) {
                    if (DalamudApi.KeyState[Keys.w_key])
                    {
                        KeyUp(Keys.w_key);
                    }
                }
                time -= 30;
                if (DirectionOfPoint < 0)
                {
                    if (!DalamudApi.KeyState[Keys.a_key])
                    {
                        KeyMethod(Keys.a_key, time);
                    }
                }
                else if (DirectionOfPoint > 0)
                {
                    if (!DalamudApi.KeyState[Keys.d_key])
                    {
                        KeyMethod(Keys.d_key, time);
                    }
                }
                if (!DalamudApi.KeyState[Keys.w_key])
                {
                    KeyDown(Keys.w_key);
                }
            }

            if (log)
            {
                PluginLog.Log($"distance: {distance} angle: {angle} height: {height}");
            }
            index++;

            if (distance > errorDisntance && beforeDistance == distance)
            {
                if (!DalamudApi.KeyState[Keys.w_key])
                {
                    KeyDown(Keys.w_key);
                }
            }
            if (!DalamudApi.Condition[ConditionFlag.InFlight])
            {
                errorDisntance = 5;
            } else if (!DalamudApi.Condition[ConditionFlag.Mounted]) {
                errorDisntance = 3.8;
            }
        }
        Stop();
        return GetUserPosition(SizeFactor);
    }

    public static void AdjustHeight(Vector3 positionB) {
        ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
        Vector3 positionA = GetUserPosition(SizeFactor);
        double height = Maths.Height(positionA, positionB);
        int i = 0;
        while ((height < -2 || height > 2) && i < 10) {
            if (height < -2 && !DalamudApi.KeyState[Keys.space_key])
            {
                KeyDown(Keys.space_key);
            }
            else if (height > 2 && !DalamudApi.KeyState[Keys.num_sub_key])
            {
                KeyDown(Keys.num_sub_key);
            }
            positionA = GetUserPosition(SizeFactor);
            height = Maths.Height(positionA, positionB);
            PluginLog.Log($"height {height} <====> {positionA.Y} <---------> {positionB.Y}");
            Thread.Sleep(500 + new Random().Next(100, 200));
            i++;
        }
        FlyStop();
    }


    public static void Stop()
    {
        MoveStop();
        FlyStop();
        Init();
    }

    public static void ForceStop()
    {
        Stop();
        Closed = true;
    }

    public static void Init() {
        Closed = false;
        FlyStop();
        MoveStop();
    }

    private static void MoveStop()
    {
        if (DalamudApi.KeyState[Keys.w_key])
        {
            KeyUp(Keys.w_key);
        }
        if (DalamudApi.KeyState[Keys.a_key])
        {
            KeyUp(Keys.a_key);
        }
        if (DalamudApi.KeyState[Keys.d_key])
        {
            KeyUp(Keys.d_key);
        }
    }

    private static void FlyStop()
    {
        if (DalamudApi.KeyState[Keys.space_key])
        {
            KeyUp(Keys.space_key);
        }
        if (DalamudApi.KeyState[Keys.num_sub_key])
        {
            KeyUp(Keys.num_sub_key);
        }
    }

    public static void KeyMethod(Byte key)
    {
        KeyMethod(key, 100, true);
    }

    public static void KeyMethod(Byte key, int sleep)
    {
        KeyMethod(key, sleep, false);
    }

    public static void KeyMethod(Byte key, int sleep, bool shortPress)
    {
        if (shortPress)
        {
            sleep = 80 + new Random().Next(30, 140);
        }
        else {
            sleep += new Random().Next(60, 100);
        }

        if (sleep == 0) {
            return;
        }

        SendMessage(Hwnd, Keys.WM_KEYDOWN, (IntPtr)key, (IntPtr)1);
        Thread.Sleep(sleep);
        SendMessage(Hwnd, Keys.WM_KEYUP, (IntPtr)key, (IntPtr)1);
        if (shortPress)
        {
            Thread.Sleep(new Random().Next(100, 200));
        }
    }

    public static void KeyDown(Byte key)
    {
        KeyTimes.TryAdd(key, DateTime.Now);
        SendMessage(Hwnd, Keys.WM_KEYDOWN, (IntPtr)key, (IntPtr)1);
    }

    public static void KeyUp(Byte key)
    {
        if (KeyTimes.TryGetValue(key, out DateTime start)) {
            DateTime endTime = DateTime.Now;
            TimeSpan interval = endTime.Subtract(start);
            double milliseconds = interval.TotalMilliseconds;
            int sleep = 80 + new Random().Next(20, 50);
            if (milliseconds >= sleep)
            {
                SendMessage(Hwnd, Keys.WM_KEYUP, (IntPtr)key, (IntPtr)1);
            }
        }
    }

    public static Vector3 ReviseNoTime(Vector3 positionB) {
        return Revise(positionB, 20);
    }

    public static Vector3 Revise(Vector3 positionB, int tt) {
        ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
        Vector3 positionA = GetUserPosition(SizeFactor);
        KeyMethod(Keys.w_key, tt);

        Vector3 positionC = GetUserPosition(SizeFactor);
        double DirectionOfPoint = Maths.DirectionOfPoint(positionA, positionB, positionC);

        double radians = Maths.Radians(positionA, positionB, positionC);
        double angle = 0;
        if (radians > -1 && radians < 1)
        {
            angle = Maths.Angle(radians);
        }
        else if (radians >= 1)
        {
            angle = 180;
        }
        int time = Convert.ToInt32(angle / 33 * 100);
        if (DirectionOfPoint < 0)
        {
            KeyMethod(Keys.a_key, time);
        }
        else if (DirectionOfPoint > 0)
        {
            KeyMethod(Keys.d_key, time);
        }
        return positionC;
    }

    public static Vector3 MovePositions(Vector3[] ToArea, bool UseMount, ushort territoryType)
    {
        Closed = false;
        ushort SizeFactor = AlphaProject.AlphaProject.GameData.GetSizeFactor(territoryType);
        Vector3 position = KeyOperates.GetUserPosition(SizeFactor);
        for (int i = 0; i < ToArea.Length; i++)
        {
            if (Closed || territoryType != DalamudApi.ClientState.TerritoryType || !Tasks.TaskRun)
            {
                PluginLog.Log($"中途结束");
                return KeyOperates.GetUserPosition(SizeFactor);
            }
            position = KeyOperates.MoveToPoint(position, ToArea[i], territoryType, UseMount, false);
            //PluginLog.Log($"到达点{i} {position.X} {position.Y} {position.Z}");
            Thread.Sleep(500);
        }
        return position;
    }

    public static Vector3 MovePositions(Vector3[] Path, bool UseMount)
    {
        Closed = false;
        ushort territoryType = DalamudApi.ClientState.TerritoryType;
        ushort SizeFactor = AlphaProject.AlphaProject.GameData.GetSizeFactor(territoryType);
        Vector3 position = GetUserPosition(SizeFactor);
        for (int i = 0; i < Path.Length; i++)
        {
            if (Closed || !Tasks.TaskRun)
            {
                PluginLog.Log($"多路径移动 中途结束");
                return GetUserPosition(SizeFactor);
            }
            position = MoveToPoint(position, Path[i], territoryType, UseMount, false);
        }
        return position;
    }
}
