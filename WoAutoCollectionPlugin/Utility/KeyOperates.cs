﻿using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using WoAutoCollectionPlugin;
using WoAutoCollectionPlugin.UseAction;
using WoAutoCollectionPlugin.Utility;

public class KeyOperates
{
    private GameData GameData { get; init; }
    private IntPtr hwnd;

    private bool closed = false;

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

    public KeyOperates(GameData GameData)
    {
        this.GameData = GameData;
        Process pro = Process.GetCurrentProcess();
        this.hwnd = Process.GetProcessById(pro.Id).MainWindowHandle;
    }

    public Vector3 GetUserPosition(ushort SizeFactor)
    {
        Vector3 playerPosition = DalamudApi.ClientState.LocalPlayer.Position;
        float x = Maths.GetCoordinate(playerPosition.X, SizeFactor);
        float y = Maths.GetCoordinate(playerPosition.Y, SizeFactor);
        float z = Maths.GetCoordinate(playerPosition.Z, SizeFactor);
        Vector3 position = new(x, y, z);
        return position;
    }

    public Vector3 MoveToPoint(Vector3 positionA, Vector3 positionB, ushort territoryType)
    {
        return MoveToPoint(positionA, positionB, territoryType, false, true);
    }

    public Vector3 MoveToPoint(Vector3 positionA, Vector3 positionB, ushort territoryType, bool UseMount)
    {
        return MoveToPoint(positionA, positionB, territoryType, UseMount, true);
    }

    public Vector3 MoveToPoint(Vector3 positionA, Vector3 positionB, ushort territoryType, bool UseMount, bool log)
    {
        Init();

        int turn = 0;
        int notMove = 0;

        int n = 0;
        while (UseMount && !DalamudApi.Condition[ConditionFlag.Mounted] && n < 3) {
            KeyMethod(Keys.q_key);
            Thread.Sleep(2500);
            n++;
        }

        double errorDisntance = 5;
        ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);

        positionA = ReviseNoTime(positionB);
        double distance = Maths.Distance(positionA, positionB);

        int index = 0;
        KeyDown(Keys.w_key);
        double height = Maths.Height(positionA, positionB, UseMount);

        while (distance > errorDisntance && index < 2400)
        {
            if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
            {
                PluginLog.Log($"移动中途结束 {closed} {territoryType} {DalamudApi.ClientState.TerritoryType}");
                Stop();
                break;
            }
            Thread.Sleep(40);

            Vector3 positionC = GetUserPosition(SizeFactor);
            double DirectionOfPoint = Maths.DirectionOfPoint(positionA, positionB, positionC);

            // 根据相对高度 上升或下降
            double beforeHeight = height;
            height = Maths.Height(positionC, positionB, UseMount);
            if (height < -1.2)
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
            else if (height > 1.2)
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

            if (Math.Abs(beforeDistance - distance) < 0.35) {
                notMove++;
            }

            if (notMove >= 8) {
                KeyMethod(Keys.d_key, 300);
                KeyDown(Keys.space_key);
                notMove = 0;
            } else if (notMove >= 5) {
                KeyMethod(Keys.a_key, 300);
                KeyMethod(Keys.space_key);
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
            // 旋转角度速度 100毫秒 30度左右 TODO 精确数据
            int time = Convert.ToInt32(angle / 30 * 100 - 100);
            if (time > -90 && turn < 10)
            {
                if (distance < 20)
                {
                    turn++;
                }
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
            }
            if (turn >= 6) {
                MoveStop();
                positionA = Revise(positionB, 600);
                turn = 0;
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
                errorDisntance = 4.7;
            } else if (!DalamudApi.Condition[ConditionFlag.Mounted]) {
                errorDisntance = 3.3;
            }
        }
        Stop();
        return GetUserPosition(SizeFactor);
    }

    public void AdjustHeight(Vector3 positionB) {
        ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
        Vector3 positionA = GetUserPosition(SizeFactor);
        double height = Maths.Height(positionA, positionB);
        int i = 0;
        while ((height < -1 || height > 1) && i < 10) {
            if (height < -1 && !DalamudApi.KeyState[Keys.space_key])
            {
                KeyDown(Keys.space_key);
            }
            else if (height > 1 && !DalamudApi.KeyState[Keys.num_sub_key])
            {
                KeyDown(Keys.num_sub_key);
            }
            positionA = GetUserPosition(SizeFactor);
            height = Maths.Height(positionA, positionB);
            PluginLog.Log($"height {height} <====> {positionA.Y} <---------> {positionB.Y}");
            Thread.Sleep(500);
            i++;
        }
        FlyStop();
    }


    public void Stop()
    {
        MoveStop();
        FlyStop();
        Init();
    }

    public void ForceStop()
    {
        Stop();
        closed = true;
    }

    public void Init() {
        closed = false;
        FlyStop();
        MoveStop();
    }

    private void MoveStop()
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

    private void FlyStop()
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

    public void KeyMethod(Byte key)
    {
        KeyMethod(key, 0, true);
    }

    public void KeyMethod(Byte key, int sleep)
    {
        KeyMethod(key, sleep, false);
    }

    public void KeyMethod(Byte key, int sleep, bool shortPress)
    {
        if (shortPress)
        {
            sleep = 100;
        }
        else
        {
            sleep = 100 + sleep;
        }

        if (sleep == 0) {
            return;
        }

        SendMessage(hwnd, Keys.WM_KEYDOWN, (IntPtr)key, (IntPtr)1);
        Thread.Sleep(sleep);
        SendMessage(hwnd, Keys.WM_KEYUP, (IntPtr)key, (IntPtr)1);
        if (shortPress)
        {
            Thread.Sleep(sleep + 200);
        }
    }

    public void KeyDown(Byte key)
    {
        SendMessage(hwnd, Keys.WM_KEYDOWN, (IntPtr)key, (IntPtr)1);
    }

    public void KeyUp(Byte key)
    {
        SendMessage(hwnd, Keys.WM_KEYUP, (IntPtr)key, (IntPtr)1);
    }

    public Vector3 ReviseNoTime(Vector3 positionB) {
        return Revise(positionB, 200);
    }

    public Vector3 Revise(Vector3 positionB, int tt) {
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
        int time = Convert.ToInt32(angle / 15 * 100 - 100);
        if (DirectionOfPoint < 0)
        {
            KeyMethod(Keys.a_key, time);
        }
        else if (DirectionOfPoint > 0)
        {
            KeyMethod(Keys.d_key, time);
        }
        Thread.Sleep(80);
        return positionC;
    }
}
