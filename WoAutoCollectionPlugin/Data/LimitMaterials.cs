using System.Collections.Generic;
using System.Numerics;

namespace WoAutoCollectionPlugin.Utility
{
    public static class LimitMaterials
    {
        public static Vector3[] path0 = {
            new Vector3(2311, 2223, 913),
            new Vector3(2889, 2236, 1252),
            new Vector3(3142, 2228, 1333),
        };
        public static Vector3[] points0 = {
            new Vector3(3266, 2190, 1203),
            new Vector3(3164, 2130, 1391),
            new Vector3(3172, 2130, 1460),
        };
        public static Vector3[] path1 = {
        };
        public static Vector3[] path2 = {
        };
        public static Vector3[] path3 = {
            new Vector3(2311, 2223, 913),
            new Vector3(2692, 2210, 1110),
        };
        public static Vector3[] points3 = {
            new Vector3(2825, 2139, 1235),
            new Vector3(2743, 2145, 1078),
            new Vector3(2743, 2163, 990),
        };
        public static Vector3[] path6 = {
            new Vector3(2868, 2850, 1413),
            new Vector3(2741, 2777, 1279),
        };
        public static Vector3[] points6 = {
            new Vector3(2741, 2777, 1279),
            new Vector3(2963, 2828, 1285),
            new Vector3(3041, 2830, 1299),
        };

        public static Vector3[] path11 = {
            new Vector3(1907, 2075, 2579),
            new Vector3(1762, 2056, 2740),
            new Vector3(1652, 2044, 2747),
        };
        public static Vector3[] points11 = {
            new Vector3(1590, 2052, 2701),
            new Vector3(1605, 2046, 2811),
            new Vector3(1773, 2038, 2767),
        };

        public static Vector3[] path12 = {
            new Vector3(2445, 2388, 1631),
            new Vector3(1957, 2260, 1635),
        };
        public static Vector3[] points12 = {
            new Vector3(1948, 2243, 1616),
            new Vector3(1888, 2235, 1631),
            new Vector3(1770, 2232, 1682),
        };

        public static Vector3[] path17 = {
            new Vector3(1638, 2228, 2645),
            new Vector3(1730, 2173, 2315),
        };
        public static Vector3[] points17 = {
            new Vector3(1665, 2177, 2296),
            new Vector3(1759, 2170, 2286),
            new Vector3(1796, 2168, 2343),
        };

        // 限时点
        public static readonly (int Id, string Name, int MinEt, int MaxEt, string Job, uint tp, Vector3[] path, Vector3[] points)[] Materials =
        {
            (0, "暗物质晶簇|火之晶簇", 1, 1, "园艺工", 24, path0, points0),    // 摩杜纳
            (1, "暗物质晶簇|火之晶簇", 5, 5, "园艺工", 24, path0, points0),   // 摩杜纳
            (2, "暗物质晶簇|水之晶簇", 9, 9, "园艺工", 24, path0, points0),   // 摩杜纳
            (3, "暗物质晶簇|雷之晶簇", 13, 13, "采矿工", 24, path3, points3),    // 摩杜纳
            (4, "暗物质晶簇|雷之晶簇", 17, 17, "采矿工",24, path3, points3),    // 摩杜纳
            (5, "暗物质晶簇|雷之晶簇", 21, 21, "采矿工",24, path3, points3),   // 摩杜纳

            (6, "暗物质晶簇|云杉原木", 9, 9, "园艺工", 23, path1, points0), // 库尔札斯中央高地
            //(7, "暗物质晶簇", 9, 9, "园艺工", 11, path1, points0),  // 东拉诺西亚 x
            //(8, "暗物质晶簇", 1, 1, "园艺工", 4, path1, points0),   // 黑衣森林东部林区 x
            //(9, "暗物质晶簇", 5, 5, "园艺工", 11, path1, points0),  // 东拉诺西亚 x
            //(10, "暗物质晶簇", 13, 13, "园艺工", 18, path1, points0),    // 东萨纳兰 x
            (11, "暗物质晶簇|绯红树汁", 3, 3, "园艺工", 7, path11, points11),   // 黑衣森林北部林区
            (12, "黄杏|暗物质晶簇", 9, 9, "园艺工", 52, path12, points12),  // 中拉诺西亚
            //(13, "暗物质晶簇", 7, 7, "园艺工", 15, path1, points0), // 拉诺西亚高地 x
            //(14, "暗物质晶簇", 2, 2, "园艺工", 3, path1, points0),  // 黑衣森林中央林区 x
            //(15, "暗物质晶簇", 8, 8, "园艺工", 13, path1, points0), // 西拉诺西亚 x
            //(16, "暗物质晶簇", 6, 6, "园艺工", 10, path1, points0), // 拉诺西亚低地 x
            (17, "暗物质晶簇|黑衣香木", 2, 2, "园艺工", 6, path17, points17),  // 黑衣森林南部林区
            (18, "暗物质晶簇|高级黑衣香木", 6, 6, "园艺工", 3, path1, points0),  // 黑衣森林中央林区
            (19, "暗物质晶簇|白金矿", 4, 4, "采矿工", 19, path1, points0), // 南萨纳兰
            (20, "暗物质晶簇", 2, 2, "园艺工", 76, path1, points0), // 龙堡参天高地
            (21, "暗物质晶簇", 14, 14, "园艺工", 76, path1, points0),   // 龙堡参天高地
            (22, "暗物质晶簇|玄铁矿", 1, 1, "采矿工", 23, path1, points0), // 库尔札斯中央高地
            (23, "暗物质晶簇|金矿", 9, 9, "采矿工", 18, path1, points0), // 东萨纳兰
            (24, "暗物质晶簇|拉诺西亚岩盐", 17, 17, "采矿工", 11, path1, points0),   // 东拉诺西亚
            //(25, "暗物质晶簇", 21, 21, "采矿工", 23, path1, points0),   // 库尔札斯中央高地 x
            (26, "强灵性岩", 2, 2, "采矿工", 18, path1, points0), // 东萨纳兰
            //(27, "暗物质晶簇", 3, 3, "采矿工", 22, path1, points0), // 北萨纳兰 x
            //(28, "暗物质晶簇", 4, 4, "采矿工", 53, path1, points0), // 中萨纳兰 x
            //(29, "暗物质晶簇", 18, 18, "采矿工", 10, path1, points0),   // 拉诺西亚低地 x
            (30, "暗物质晶簇|火之晶簇", 19, 19, "采矿工", 52, path1, points0),   // 中拉诺西亚 
            (31, "暗物质晶簇|灵性岩", 6, 6, "采矿工", 6, path1, points0),  // 黑衣森林南部林区
            (32, "暗物质晶簇|水之晶簇", 5, 5, "采矿工", 17, path1, points0), // 西萨纳兰

            (33, "迷迭香", 17, 17, "园艺工", 4, path6, points0),   // 黑衣森林东部林区
            (34, "延龄花", 5, 5, "园艺工", 4, path6, points0),     // 黑衣森林东部林区
            (35, "黄杏", 9, 9, "园艺工", 52, path6, points0),     // 中拉诺西亚
        };

        public static List<int> GetMaterialIdsByEt(int et)
        {
            List<int> list = new List<int>();
            foreach ((int Id, string Name, int MinEt, int MaxEt, string Job, uint tp, Vector3[] path, Vector3[] points) in Materials)
            {
                if (et >= MinEt && et <= MaxEt)
                {
                    list.Add(Id);
                }
            }
            return list;
        }

        public static (string Name, string Job, uint tp, Vector3[] path, Vector3[] points) GetMaterialById(int id)
        {
            foreach ((int Id, string Name, int MinEt, int MaxEt, string Job, uint tp, Vector3[] path, Vector3[] points) in Materials)
            {
                if (id == Id)
                {
                    return (Name, Job, tp, path, points);
                }
            }
            return (null, null, 0, null, null);
        }
    }
}
