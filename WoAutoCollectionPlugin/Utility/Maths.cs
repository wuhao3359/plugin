using Dalamud.Game.ClientState.Conditions;
using System;
using System.Numerics;

namespace WoAutoCollectionPlugin.Utility;

public static class Maths
{
    public const int RIGHT = 1, LEFT = -1, ZERO = 0;
    public static int SquaredDistance(int x1, int y1, int x2, int y2)
    {
        x1 -= x2;
        y1 -= y2;
        return x1 * x1 + y1 * y1;
    }

    public static int SquaredDistance(int x1, int y1, int z1, int x2, int y2, int z2)
    {
        x1 -= x2;
        y1 -= y2;
        z1 -= z2;
        return x1 * x1 + y1 * y1 + z1 * z1;
    }

    // 得到C角度 cosC = (a^2 + b^2 -c^2) / 2ab
    public static double Radians(Vector3 positionA, Vector3 positionB, Vector3 positionC)
    {
        double c = Distance(positionA, positionB);
        double a = Distance(positionB, positionC);
        double b = Distance(positionA, positionC);
        return (a * a + b * b - c * c) / 2 / a / b;
    }

    // Math.Acos(radians)
    public static double Angle(double radians)
    {
        return 180 - (Math.Acos(radians) * (180 / Math.PI));
    }

    // 向量计算方法
    public static int DirectionOfPoint(Vector3 positionA, Vector3 positionB, Vector3 positionC)
    {
        positionC.X -= positionA.X;
        positionC.Z -= positionA.Z;
        positionB.X -= positionA.X;
        positionB.Z -= positionA.Z;

        float cross_product = positionC.X * positionB.Z - positionC.Z * positionB.X;

        if (cross_product > 0)
            return RIGHT;

        if (cross_product < 0)
            return LEFT;

        return ZERO;
    }

    // 计算两点之间的距离
    public static double Distance(Vector3 positionA, Vector3 positionB)
    {
        return Math.Round(Math.Sqrt(Math.Pow(positionA.X - positionB.X, 2) + Math.Pow(positionA.Z - positionB.Z, 2)), 2);
    }

    // 计算两点之间的高度

    public static double Height(Vector3 positionA, Vector3 positionB)
    {
        return Height(positionA, positionB, true);
    }
    public static double Height(Vector3 positionA, Vector3 positionB, bool UseMount)
    {
        if (UseMount || DalamudApi.Condition[ConditionFlag.Diving])
        {
            return positionA.Y - positionB.Y;
        }
        return 0;
    }

    public static float GetCoordinate(float c, ushort zid)
    {
        c *= 0.02f;
        switch (zid)
        {
            case 95:
                return (c + 22.5f) * 100f;
            case 100:
                return (c + 21.5f) * 100f;
            case 300:
                return (c + 7.5f) * 100f;
            case 400:
                return (c + 6) * 100f;
            case 200:
            case 0:
                return (c + 11.25f) * 100f;
            case 800:
                return (c + 3.5f) * 100f;
            default:
                return 0;
        }
    }
}
