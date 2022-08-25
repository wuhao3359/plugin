using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using WoAutoCollectionPlugin;
using WoAutoCollectionPlugin.Utility;

public class KeyOperates
{
    private GameData GameData { get; init; }
    private IntPtr hwnd;

    private bool closed = false;
    private int moving = 0;
    private int flying = 0;

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

    public Vector3 TestMoveToPoint(Vector3 positionA, Vector3 positionB, ushort territoryType)
    {
        return TestMoveToPoint(positionA, positionB, territoryType, false, true);
    }

    public Vector3 TestMoveToPoint(Vector3 positionA, Vector3 positionB, ushort territoryType, bool UseMount)
    {
        return TestMoveToPoint(positionA, positionB, territoryType, UseMount, true);
    }

    public Vector3 TestMoveToPoint(Vector3 positionA, Vector3 positionB, ushort territoryType, bool UseMount, bool log)
    {
        double errorDisntance = 5.5;
        ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
        KeyMethod(Keys.w_key, 200);

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
        positionA = positionC;
        double distance = Maths.Distance(positionA, positionB);
        double height = Maths.Height(positionA, positionB, UseMount);

        int index = 0;
        moving = 1;
        KeyDown(Keys.w_key);

        if (height < -3)
        {
            flying = 1;
            KeyDown(Keys.space_key);
        }
        else if (height > 5)
        {
            flying = -1;
            KeyDown(Keys.num_sub_key);
        }

        while (distance > errorDisntance && index < 2400)
        {
            if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
            {
                PluginLog.Log($"移动中途结束 ${closed} ${territoryType} ${DalamudApi.ClientState.TerritoryType}");
                Stop();
                return GetUserPosition(SizeFactor);
            }
            Thread.Sleep(50);

            if (territoryType != DalamudApi.ClientState.TerritoryType)
            {
                break;
            }

            positionC = GetUserPosition(SizeFactor);
            DirectionOfPoint = Maths.DirectionOfPoint(positionA, positionB, positionC);

            // 根据相对高度 上升或下降
            double beforeHeight = height;
            height = Maths.Height(positionC, positionB, UseMount);

            if (height < -2)
            {
                if (flying < 0)
                {
                    if (DalamudApi.KeyState[Keys.num_sub_key]) {
                        KeyUp(Keys.num_sub_key);
                    }
                    if (!DalamudApi.KeyState[Keys.space_key])
                    {
                        KeyDown(Keys.space_key);
                    }
                    flying = 1;
                }
            }
            else if (height > 2)
            {
                if (flying > 0)
                {
                    if (DalamudApi.KeyState[Keys.space_key])
                    {
                        KeyUp(Keys.space_key);
                    }
                    if (!DalamudApi.KeyState[Keys.num_sub_key])
                    {
                        KeyDown(Keys.num_sub_key);
                    }
                    flying = -1;
                }
            }
            else
            {
                FlyStop();
            }

            radians = Maths.Radians(positionA, positionB, positionC);
            angle = 0;
            if (radians > -1 && radians < 1)
            {
                angle = Maths.Angle(radians);
            }
            else if (radians >= 1)
            {
                angle = 180;
            }
            // 旋转角度速度 100毫秒 30度左右 TODO 精确数据
            time = Convert.ToInt32(angle / 30 * 100 - 100);
            if (time > -80)
            {
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

            double beforeDistance = distance;
            positionA = GetUserPosition(SizeFactor);
            distance = Maths.Distance(positionA, positionB);
            if (log)
            {
                PluginLog.Log($"distance: {distance} height: {height} moving: {moving} flying: {flying}");
            }
            index++;

            if (distance > errorDisntance && beforeDistance == distance)
            {
                if (!DalamudApi.KeyState[Keys.w_key])
                {
                    KeyDown(Keys.w_key);
                }
                moving = 1;
            }
            if (height <= 2 && height >= -2)
            {
                FlyStop();
            }
            else if (beforeHeight == height)
            {
                if (height < -2)
                {
                    KeyDown(Keys.space_key);
                    flying = 1;
                }
                else if (height > 2)
                {
                    KeyDown(Keys.num_sub_key);
                    flying = -1;
                }
            }

            if (!DalamudApi.Condition[ConditionFlag.InFlight])
            {
                errorDisntance = 3.5;
            }
        }
        PluginLog.Log($"到附近distance: {distance} height: {height} moving: {moving} flying: {flying}");
        Stop();
        return GetUserPosition(SizeFactor);
    }
    
    public void Stop()
    {
        MoveStop();
        FlyStop();
        closed = false;
    }

    public void ForceStop()
    {
        Stop();
        closed = true;
    }

    private void MoveStop()
    {
        if (DalamudApi.KeyState[Keys.w_key])
        {
            KeyUp(Keys.w_key);
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
}
