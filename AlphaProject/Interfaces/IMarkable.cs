using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AlphaProject.Classes;

namespace AlphaProject.Interfaces;

public interface IMarkable
{
    public const int CoordMin = 100;
    public const int CoordMax = 4200;
    public const int MarkersMax = 8;

    public string Name { get; }
    public Territory Territory { get; }
    public int IntegralXCoord { get; set; }
    public int IntegralYCoord { get; set; }

    public int DefaultXCoord { get; }
    public int DefaultYCoord { get; }

    public Vector3[] Markers { get; set; }

    public bool SetMarkers(IEnumerable<Vector3> markers)
    {
        var tmpMarkers = markers.Take(MarkersMax).ToArray();
        if (Markers.SequenceEqual(tmpMarkers))
            return false;

        Markers = tmpMarkers;
        return true;
    }

    public bool SetXCoord(int xCoord)
    {
        if (xCoord is < CoordMin or > CoordMax)
            return false;

        IntegralXCoord = xCoord;
        return true;
    }

    public bool SetYCoord(int yCoord)
    {
        if (yCoord is < CoordMin or > CoordMax)
            return false;

        IntegralYCoord = yCoord;
        return true;
    }
}
