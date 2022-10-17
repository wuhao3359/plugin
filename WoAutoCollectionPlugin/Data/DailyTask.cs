using System.Collections.Generic;
using System.Numerics;

namespace WoAutoCollectionPlugin.Utility
{
    public static class DailyTask
    {
        public static Vector3[] path0 = {
            new Vector3(2297f, 2270f, 874f),
            new Vector3(3123f, 2156f, 1449f),
        };
        public static Vector3[] path1 = {
            new Vector3(2274f, 2262f, 852f),
            new Vector3(2944f, 2203f, 1316f),
            new Vector3(3272f, 2188f, 1150f),
        };
        public static Vector3[] path2 = {
            new Vector3(2271f, 2261f, 855f),
            new Vector3(3157f, 2158f, 1419f),
        };
        public static Vector3[] points0 = {
            new Vector3(2297f, 2270f, 874f),
            new Vector3(3123f, 2156f, 1449f),
        };
        public static Vector3[] path3 = {
            new Vector3(2265f, 2265f, 856f),
            new Vector3(2682f, 2195f, 1100f),
        };
        public static Vector3[] path4 = {
            new Vector3(2265f, 2265f, 856f),
            new Vector3(2682f, 2195f, 1100f),
        };
        public static Vector3[] path5 = {
            new Vector3(2265f, 2265f, 856f),
            new Vector3(2682f, 2195f, 1100f),
        };
        public static Vector3[] path6 = {
            new Vector3(2375f, 2349f, 3254f),
            new Vector3(2369f, 2336f, 2302f),
            new Vector3(2406f, 2221f, 2223f),
        };
        public static Vector3[] path7 = {
            new Vector3(2563f, 2416f, 1758f),
            new Vector3(2469f, 2293f, 2712f),
        };
        public static Vector3[] path8 = {
            new Vector3(1640f, 2216f, 2868f),
            new Vector3(1563f, 2159f, 2912f),
        };

        // 限时点
        public static readonly (int Id, string Name, int MinEt, int MaxEt, string Job, int GatherIndex, uint tp, Vector3[] path, Vector3[] points)[] Materials =
        {
            (0, "暗物质晶簇", 1, 1, "园艺工", 8, 24, path0, points0),    // 摩杜纳 1
            (1, "火晶", 5, 5, "园艺工", 8, 24, path1, points0),   // 摩杜纳 1
            (2, "暗物质晶簇", 9, 9, "园艺工", 1, 24, path2, points0),   // 摩杜纳 1
            (3, "暗物质晶簇", 13, 13, "采矿工", 1, 24, path3, points0),    // 摩杜纳 1
            (4, "暗物质晶簇", 17, 17, "采矿工", 8, 24, path4, points0),    // 摩杜纳 1
            (5, "暗物质晶簇", 21, 21, "采矿工", 1, 24, path5, points0),   // 摩杜纳 1
            (6, "迷迭香", 17, 17, "园艺工", 10, 4, path6, points0),   // 黑衣森林东部林区
            (7, "延龄花", 5, 5, "园艺工", 10, 4, path7, points0),     // 黑衣森林东部林区
            (8, "黄杏", 9, 9, "园艺工", 10, 52, path8, points0),     // 中拉诺西亚
        };

        public static List<int> GetMaterialIdsByEt(int et)
        {
            List<int> list = new List<int>();
            foreach ((int Id, string Name, int MinEt, int MaxEt, string Job, int GatherIndex, uint tp, Vector3[] path, Vector3[] points) in Materials)
            {
                if (et >= MinEt && et <= MaxEt)
                {
                    list.Add(Id);
                }
            }
            return list;
        }

        public static (string Name, string Job, int GatherIndex, uint tp, Vector3[] path, Vector3[] points) GetMaterialById(int id)
        {
            foreach ((int Id, string Name, int MinEt, int MaxEt, string Job, int GatherIndex, uint tp, Vector3[] path, Vector3[] points) in Materials)
            {
                if (id == Id)
                {
                    return (Name, Job, GatherIndex, tp, path, points);
                }
            }
            return (null, null, -1, 0, null, null);
        }
    }
}
