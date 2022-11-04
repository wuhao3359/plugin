using Dalamud.Game.ClientState.Conditions;
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
    private int moving = 0;
    private int flying = 0;

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern int mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

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

        if (UseMount && !DalamudApi.Condition[ConditionFlag.Mounted]) {
            KeyMethod(Keys.q_key);
            Thread.Sleep(2000);
        }

        double errorDisntance = 5.5;
        ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);

        positionA = ReviseNoTime(positionB);
        double distance = Maths.Distance(positionA, positionB);
        double height = Maths.Height(positionA, positionB, UseMount);

        int index = 0;
        moving = 1;
        KeyDown(Keys.w_key);

        if (UseMount && DalamudApi.Condition[ConditionFlag.Mounted])
        {
            if (height < -2) {
                KeyDown(Keys.space_key);
                flying = 1;
            }
        }

        while (distance > errorDisntance && index < 2400)
        {
            if (closed || territoryType != DalamudApi.ClientState.TerritoryType)
            {
                PluginLog.Log($"移动中途结束 {closed} {territoryType} {DalamudApi.ClientState.TerritoryType}");
                Stop();
                break;
            }
            Thread.Sleep(50);

            if (territoryType != DalamudApi.ClientState.TerritoryType)
            {
                break;
            }

            Vector3 positionC = GetUserPosition(SizeFactor);
            double DirectionOfPoint = Maths.DirectionOfPoint(positionA, positionB, positionC);

            // 根据相对高度 上升或下降
            double beforeHeight = height;
            height = Maths.Height(positionC, positionB, UseMount);
            if (height < -1)
            {
                if (flying <= 0)
                {
                    if (DalamudApi.KeyState[Keys.num_sub_key] && DalamudApi.Condition[ConditionFlag.InFlight])
                    {
                        KeyUp(Keys.num_sub_key);
                    }
                    if (!DalamudApi.KeyState[Keys.space_key] || !DalamudApi.Condition[ConditionFlag.InFlight])
                    {
                        KeyDown(Keys.space_key);
                    }
                    flying = 1;
                }
            }
            else if (height > 2)
            {
                if (flying >= 0)
                {
                    if (!DalamudApi.KeyState[Keys.num_sub_key] && DalamudApi.Condition[ConditionFlag.InFlight])
                    {
                        KeyDown(Keys.num_sub_key);
                    }
                    //if (DalamudApi.KeyState[Keys.space_key])
                    //{
                    //    KeyUp(Keys.space_key);
                    //}
                    flying = -1;
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

            if (notMove >= 10) {
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
            if (turn >= 8) {
                MoveStop();
                positionA = Revise(positionB, 800);
                turn = 0;
            }

            if (log)
            {
                PluginLog.Log($"distance: {distance} angle: {angle} height: {height} moving: {moving} flying: {flying}");
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
            if (height <= 2 && height >= -1)
            {
                FlyStop();
            }
            else if (beforeHeight == height)
            {
                if (height < -1)
                {
                    KeyDown(Keys.space_key);
                    flying = 1;
                }
                else if (height > 2)
                {
                    if (DalamudApi.Condition[ConditionFlag.InFlight]) {
                        KeyDown(Keys.num_sub_key);
                    }
                    flying = -1;
                }
            }

            if (!DalamudApi.Condition[ConditionFlag.InFlight])
            {
                errorDisntance = 4.3;
            } else if (!DalamudApi.Condition[ConditionFlag.Mounted]) {
                errorDisntance = 3;
            }
        }
        //PluginLog.Log($"到附近distance: {distance} height: {height} moving: {moving} flying: {flying}");
        Stop();
        return GetUserPosition(SizeFactor);
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
        moving = 0;
        flying = 0;
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

    public void ClickMouseLeft(int x, int y)
    {
        //int lparam = (y << 16) + x + 31 * 2;
        int lparam = (y << 16) | x;
        SendMessage(hwnd, Keys.WM_LBUTTONDOWN, (IntPtr)0, (IntPtr)lparam);
        Thread.Sleep(100);
        SendMessage(hwnd, Keys.WM_LBUTTONUP, (IntPtr)0, (IntPtr)0);
    }

    public void MouseMove(int x, int y)
    {
        // (y<<16) | x
        //int lparam = (y << 16) + x + 31 * 2;
        //int lparam = (y << 16) | x;
        //IntPtr lparam = new IntPtr((y << 16) | x);
        //SendMessage(hwnd, Keys.WM_MOUSEMOVE, (IntPtr)0, lparam);
        Thread.Sleep(3000);
        mouse_event(Keys.MOUSEEVENTF_MOVE, x, y, 0, 0);
    }

    public Vector3 ReviseNoTime(Vector3 positionB) {
        return Revise(positionB, 250);
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
