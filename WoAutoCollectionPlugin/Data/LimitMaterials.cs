using System.Collections.Generic;
using System.Numerics;

namespace WoAutoCollectionPlugin.Utility
{
    public static class LimitMaterials
    {
        // 16-采矿工 17-园艺工 18-捕鱼人
        // 8-刻木匠 9-锻铁匠 10-铸甲匠 11-雕金匠 12-制革匠 13-裁衣匠
        // 14-炼金术士 15-烹调师

        public static Vector3[] path0 = {
            new Vector3(2268, 2225, 866),
            new Vector3(2311, 2228, 913),
            new Vector3(2889, 2236, 1252),
            new Vector3(3142, 2228, 1333),
        };
        public static Vector3[] points0 = {
            new Vector3(3266, 2190, 1203),
            new Vector3(3164, 2130, 1391),
            new Vector3(3172, 2130, 1460),
        };
        public static Vector3[] path3 = {
            new Vector3(2268, 2225, 866),
            new Vector3(2311, 2228, 913),
            new Vector3(2692, 2212, 1110),
        };
        public static Vector3[] points3 = {
            new Vector3(2825, 2139, 1235),
            new Vector3(2743, 2145, 1078),
            new Vector3(2743, 2163, 990),
        };
        public static Vector3[] path6 = {
            new Vector3(2868, 2850, 1413),
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

        public static Vector3[] path17 = {
            new Vector3(1638, 2238, 2645),
            new Vector3(1730, 2233, 2315),
        };
        public static Vector3[] points17 = {
            new Vector3(1665, 2177, 2296),
            new Vector3(1759, 2170, 2286),
            new Vector3(1796, 2168, 2343),
        };

        public static Vector3[] path18 = {
            new Vector3(2981, 2220, 2010),
        };
        public static Vector3[] points18 = {
            new Vector3(3085, 2199, 1932),
            new Vector3(3059, 2188, 2001),
            new Vector3(3024, 2216, 2109),
        };

        public static Vector3[] path19 = {
            new Vector3 (1853, 2242, 1287),
            new Vector3 (1861, 2248, 1217),
            new Vector3(1741, 2253, 1092),
        };
        public static Vector3[] points19 = {
            new Vector3(1893, 2191, 925),
            new Vector3(1691, 2193, 1093),
            new Vector3(1598, 2225, 1205),
        };

        public static Vector3[] path20 = {
            new Vector3(3206, 2297, 2042),
            new Vector3(2733, 2292, 1483),
        };
        public static Vector3[] points20 = {
            new Vector3(2663, 2189, 1388),
            new Vector3(2633, 2200, 1294),
            new Vector3(2672, 2196, 1328),
        };

        public static Vector3[] path22 = {
            new Vector3(2681, 2807, 1973),
        };
        public static Vector3[] points22 = {
            new Vector3(2738, 2761, 1966),
            new Vector3(2766, 2764, 1992),
            new Vector3(2757, 2761, 2035),
        };

        public static Vector3[] path23 = {
            new Vector3(1360, 2100, 2337),
            new Vector3(2689, 2189, 2139),
            new Vector3(2813, 2179, 2127),
        };
        public static Vector3[] points23 = {
            new Vector3(2765, 2170, 2127),
            new Vector3(2823, 2176, 2139),
            new Vector3(2817, 2190, 2238),
        };

        public static Vector3[] path24 = {
            new Vector3(2418, 2422, 3206),
            new Vector3(2151, 2350, 3225),
            new Vector3(1961, 2279, 3365),
        };
        public static Vector3[] points24 = {
            new Vector3(1902, 2250, 3226),
            new Vector3(1776, 2262, 3428),
            new Vector3(2148, 2270, 3275),
        };

        public static Vector3[] path26 = {
            new Vector3(1376, 2136, 2330),
            new Vector3(2648, 2200, 1839),
        };
        public static Vector3[] points26 = {
            new Vector3(2623, 2165, 1920),
            new Vector3(2676, 2182, 1850),
            new Vector3(2753, 2160, 1801),
        };

        public static Vector3[] path30 = {
            new Vector3(2464, 2381, 1674),
            new Vector3(2447, 2368, 2125),
            new Vector3(2432, 2302, 2614),
        };
        public static Vector3[] points30 = {
            new Vector3(2392, 2268, 2632),
            new Vector3(2424, 2272, 2701),
            new Vector3(2483, 2278, 2749),
        };

        public static Vector3[] path31 = {
            new Vector3(1662, 2243, 3101),
        };
        public static Vector3[] points31 = {
            new Vector3(1533, 2153, 2950),
            new Vector3(1659, 2149, 3133),
            new Vector3(1843, 2159, 3126),
        };

        public static Vector3[] path32 = {
            new Vector3(2219, 2356, 2077),
            new Vector3(1915, 2295, 2826),
        };
        public static Vector3[] points32 = {
            new Vector3(1924, 2249, 2789),
            new Vector3(1800, 2225, 2846),
            new Vector3(1966, 2267, 2898),
        };

        public static Vector3[] path33 = {
            new Vector3(1892, 2194, 2833),
            new Vector3(2203, 2182, 3033),
        };
        public static Vector3[] points33 = {
            new Vector3(2188, 2171, 3082),
            new Vector3(2248, 2165, 3009),
            new Vector3(2369, 2171, 3074),
        };

        public static Vector3[] path34 = {
            new Vector3(1751, 2257, 2034),
        };
        public static Vector3[] points34 = {
            new Vector3(1770, 2231, 1970),
            new Vector3(1784, 2255, 1919),
            new Vector3(1753, 2257, 1918),
        };

        public static Vector3[] path35 = {
            new Vector3(2420, 2372, 1628),
            new Vector3(1973, 2287, 1610),
        };
        public static Vector3[] points35 = {
            new Vector3(1948, 2243, 1608),
            new Vector3(1887, 2235, 1629),
            new Vector3(1767, 2235, 1691),
        };

        public static Vector3[] path36 = {
            new Vector3(1904, 2233, 2788),
        };
        public static Vector3[] points36 = {
            new Vector3(1991, 2189, 2700),
            new Vector3(1997, 2183, 2729),
            new Vector3(2044, 2172, 2774),
        };

        public static Vector3[] path362151 = {
            new Vector3(1610, 2319, 3161),
            new Vector3(2775, 2333, 3385),
            new Vector3(3169, 2125, 3482)
        };
        public static Vector3[] points362151 = {
            new Vector3(3218, 2115, 3506),
            new Vector3(3206, 2118, 3487),
            new Vector3(3221, 2118, 3470),
        };

        public static Vector3[] path362152 = {
            new Vector3(1020, 2600, 2918),
            new Vector3(963, 2526, 2424)
        };
        public static Vector3[] points362152 = {
            new Vector3(927, 2443, 2358),
            new Vector3(1003, 2433, 2374),
            new Vector3(915, 2443, 2297),
        };

        public static Vector3[] path362153 = {
            new Vector3(1252, 2765, 1089),
            new Vector3(1354, 2815, 761)
        };
        public static Vector3[] points362153 = {
            new Vector3(1349, 2805, 732),
            new Vector3(1372, 2805, 723),
            new Vector3(1353, 2815, 707),
        };

        public static Vector3[] path378191 = {
            new Vector3(2936, 2219, 1762),
            new Vector3(2890, 2229, 2477),
        };
        public static Vector3[] points378191 = {
            new Vector3(2826, 2169, 2624),
            new Vector3(2874, 2169, 2750),
            new Vector3(3021, 2170, 2651),
        };

        public static Vector3[] path378181 = {
            new Vector3(968, 2129, 3073),
            new Vector3(1007, 2127, 3030),
        };
        public static Vector3[] points378181 = {
            new Vector3(1087, 2097, 3052),
            new Vector3(930, 2097, 2971),
            new Vector3(933, 2086, 2930),
        };

        public static Vector3[] path378171 = {
            new Vector3(3159, 2086, 1678),
            new Vector3(3189, 2091, 1614),
            new Vector3(3175, 2119, 1544),
            new Vector3(3150, 2151, 1462),
            new Vector3(3156, 2196, 1263),
            new Vector3(3174, 2217, 1304),
            new Vector3(3244, 2259, 1688),
        };
        public static Vector3[] points378171 = {
            new Vector3(3288, 2185, 1683),
            new Vector3(3137, 2182, 1660),
            new Vector3(3153, 2186, 1852),
        };

        public static Vector3[] path378201 = {
            new Vector3(2152, 2096, 1359),
            new Vector3(2146, 2124, 1649),
            new Vector3(2139, 2399, 1969),
            new Vector3(2899, 2438, 2308),
        };
        public static Vector3[] points378201 = {
            new Vector3(3048, 2414, 2352),
            new Vector3(2957, 2417, 2241),
            new Vector3(2871, 2412, 2139),
        };

        public static Vector3[] path378211 = {
            new Vector3(2872, 2524, 1148),
        };
        public static Vector3[] points378211 = {
            new Vector3(2837, 2481, 1162),
            new Vector3(2816, 2488, 1133),
            new Vector3(2799, 2481, 1163),
        };

        // 16-采矿工 17-园艺工 18-捕鱼人
        // 优先级排序
        // type 类型 1-普通限时 2-  3-  4-灵砂 5-高难限时
        public static (int Id, string Name, int MinEt, int MaxEt, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, uint type)[] Materials =
        {
            // 6.2版本
            //(37694, "不定性铁陨石", 0, 1, 16, "采矿工", 90, 172, path362151, points362151), // 加雷马
            //(37691, "不定性结晶花", 0, 1, 16, "采矿工", 90, 172, path362151, points362151), // 天外天垓 

            (378191, "诃子", 0, 1, 17, "园艺工", 90, 171, path378191, points378191, 1), // 萨维奈岛 护法村
            (378192, "诃子", 12, 13, 17, "园艺工", 90, 171, path378191, points378191, 1), // 萨维奈岛 护法村
            (378181, "仁面木原木", 2, 3, 17, "园艺工", 90, 177, path378181, points378181, 1), // 厄尔庇斯 十二奇园
            (378181, "仁面木原木", 14, 15, 17, "园艺工", 90, 177, path378181, points378181, 1), // 厄尔庇斯 十二奇园
            (378171, "发晶原石", 6, 7, 16, "采矿工", 90, 173, path378171, points378171, 1), // 加雷马 第三站
            (378172, "发晶原石", 18, 19, 16, "采矿工", 90, 173, path378171, points378171, 1), // 加雷马 第三站
            (378201, "钛铁矿", 8, 9, 16, "采矿工", 90, 175, path378201, points378201, 1), // 叹息海 威兔洞
            (378202, "钛铁矿", 20, 21, 16, "采矿工", 90, 175, path378201, points378201, 1), // 叹息海 威兔洞
            (378211, "杨梅", 4, 5, 17, "园艺工", 90, 166, path378211, points378211, 1), // 迷津 保管院
            (378212, "杨梅", 16, 17, 17, "园艺工", 90, 166, path378211, points378211, 1), // 迷津 保管院

            // 6.0版本
            //(3621511, "极硬水", 0, 1, 16, "采矿工", 90, 172, path362151, points362151), // 加雷马
            //(3621512, "极硬水", 12, 13, 16, "采矿工", 90, 172, path362151, points362151), // 加雷马
            //(3621521, "极硬水", 2, 3, 16, "采矿工", 90, 174, path362152, points362152, 1), // 叹息海
            //(3621522, "极硬水", 14, 15, 16, "采矿工", 90, 174, path362152, points362152, 1), // 叹息海
            //(3621531, "极硬水", 4, 5, 16, "采矿工", 90, 178, path362153, points362153, 1), // 厄尔庇斯 
            //(3621532, "极硬水", 16, 17, 16, "采矿工", 90, 178, path362153, points362153, 1), // 厄尔庇斯 

            (33, "迷迭香", 17, 18, 17, "园艺工", 50, 4, path33, points33, 1),   // 黑衣森林东部林区
            (34, "延龄花", 5, 6, 17, "园艺工", 50, 4, path34, points34, 1),     // 黑衣森林东部林区
            (35, "黄杏", 9, 10, 17, "园艺工", 50, 52, path35, points35, 1),     // 中拉诺西亚
            (36, "小柠檬", 6, 7, 17, "园艺工", 80, 138, path36, points36, 1),     // 珂露西亚岛
            (37, "小柠檬", 18, 19, 17, "园艺工", 80, 138, path36, points36, 1),    // 珂露西亚岛

            (0, "暗物质晶簇|水之晶簇", 1, 2, 17, "园艺工", 50, 24, path0, points0, 1),    // 摩杜纳
            (1, "暗物质晶簇|水之晶簇", 5, 6, 17, "园艺工", 50, 24, path0, points0, 1),   // 摩杜纳
            (2, "暗物质晶簇|水之晶簇", 9, 10, 17, "园艺工", 50, 24, path0, points0, 1),   // 摩杜纳
            (3, "暗物质晶簇|水之晶簇", 13, 14, 16, "采矿工", 50, 24, path3, points3, 1),    // 摩杜纳
            (4, "暗物质晶簇|火之晶簇", 17, 18, 16, "采矿工", 50,24, path3, points3, 1),    // 摩杜纳
            (5, "暗物质晶簇|雷之晶簇", 21, 22, 16, "采矿工", 50, 24, path3, points3, 1),   // 摩杜纳
            (6, "暗物质晶簇|云杉原木", 9, 10, 17, "园艺工", 50, 23, path6, points6, 1), // 库尔札斯中央高地
            (11, "暗物质晶簇|绯红树汁", 3, 4, 17, "园艺工", 50, 7, path11, points11, 1),   // 黑衣森林北部林区
            // (17, "暗物质晶簇|黑衣香木", 2, 4, 17, "园艺工", 50, 6, path17, points17, 1),  // 黑衣森林南部林区
            // (18, "暗物质晶簇|高级黑衣香木", 6, 7, 16, "园艺工", 50, 3, path18, points18, 1),  // 黑衣森林中央林区
            (19, "白金矿|暗物质晶簇", 4, 5, 16, "采矿工", 50, 19, path19, points19, 1), // 南萨纳兰
            (20, "暗物质晶簇", 2, 3, 17, "园艺工", 50, 76, path20, points20, 1), // 龙堡参天高地
            (21, "暗物质晶簇", 14, 15, 17, "园艺工", 50, 76, path20, points20, 1),   // 龙堡参天高地
            (22, "玄铁矿|暗物质晶簇", 1, 2, 16, "采矿工", 50, 23, path22, points22, 1), // 库尔札斯中央高地
            (23, "金矿|暗物质晶簇", 9, 10, 16, "采矿工", 50, 18, path23, points23, 1), // 东萨纳兰
            //(24, "暗物质晶簇|拉诺西亚岩盐", 17, 18, 16, "采矿工", 50, 11, path24, points24),   // 东拉诺西亚 
            // (25, "暗物质晶簇|钨铁矿", 3, 4, 16, "采矿工", 50, 11, path24, points24),   // 翻云雾海
            //(26, "强灵性岩", 2, 3, 16, "采矿工", 50, 18, path26, points26), // 东萨纳兰
            // (27, "光银矿", 2, 3, 16, "采矿工", 60, 18, path26, points26), // 翻云雾海
            // (28, "皇金矿", 14, 15, 16, "采矿工", 60, 18, path26, points26), // 翻云雾海
            
            (30, "暗物质晶簇|火之晶簇", 19, 21, 16, "采矿工", 50, 52, path30, points30, 1),   // 中拉诺西亚
            // (31, "暗物质晶簇|灵性岩", 6, 8, 16, "采矿工", 50, 6, path31, points31, 1),  // 黑衣森林南部林区
            (32, "暗物质晶簇|水之晶簇", 5, 7, 16, "采矿工", 50, 17, path32, points32, 1), // 西萨纳兰

        };

        public static Vector3[] path36285 = {
            new Vector3(1446, 2526, 3421),
            new Vector3(1821, 2440, 3467),
            new Vector3(1987, 2353, 3505),
        };
        public static Vector3[] points36285 = {
            new Vector3(2094, 2321, 3440),
            new Vector3(2000, 2325, 3466),
            new Vector3(2129, 2386, 3575),
            new Vector3(2187, 2369, 3622),
            new Vector3(2233, 2383, 3501),
            new Vector3(2312, 2290, 3330),
        };
        public static int[] gatherIndex36285 = {
            1, 3, 5
        };

        public static Vector3[] path36286 = {
            new Vector3(1294, 2402, 2492),
            new Vector3(1229, 2290, 1547),
        };
        public static Vector3[] points36286 = {
            new Vector3(1092, 2206, 1674),
            new Vector3(1168, 2236, 1561),
            new Vector3(862, 2237, 1569),
            new Vector3(1090, 2205, 1358),
            new Vector3(1179, 2233, 1605),
        };
        public static int[] gatherIndex36286 = {
            0, 2, 3
        };

        public static Vector3[] path36287 = {
            new Vector3(782, 2146, 3003),
            new Vector3(929, 2096, 3257),
        };
        public static Vector3[] points36287 = {
            new Vector3(942, 2097, 3332),
            new Vector3(1087, 2116, 3404),
            new Vector3(1174, 2092, 3443),
            new Vector3(1099, 2139, 3581),
            new Vector3(1058, 2116, 3662),
            new Vector3(981, 2124, 3417),
        };
        public static int[] gatherIndex36287 = {
            0, 2, 4
        };

        public static Vector3[] path36288 = {
            new Vector3(2676, 2185, 1795),
        };
        public static Vector3[] points36288 = {
            new Vector3(2693, 2161, 1741),
            new Vector3(2541, 2168, 1625),
            new Vector3(2491, 2165, 1600),
            new Vector3(2797, 2189, 1416),
            new Vector3(2843, 2163, 1359),
            new Vector3(2696, 2181, 1692),
        };
        public static int[] gatherIndex36288 = {
            0, 2, 4
        };

        // 收藏品 需要分解或者上交
        public static (int Id, string Name, int MinEt, int MaxEt, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, int[] CanGatherIndex)[] CollecMaterials = {
            (36285, "月石英", 0, 3, 16, "采矿工", 90, 174, path36285, points36285, gatherIndex36285),    //叹息海
            //(36286, "暗性岩", 20, 23, 16, "采矿工", 90, 172, path36286, points36286, gatherIndex36286),    // 碎璃营地
            (36287, "水瓶土", 8, 11, 17, "园艺工", 90, 168, path36287, points36287, gatherIndex36287),    // 迷津
            //(36288, "棕榈碎皮", 12, 15, 17, "园艺工", 90, 171, path36288, points36288, gatherIndex36288), // 波洛伽护法村
        };

        public static (int Id, string Name, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, int[] CanGatherIndex)[] Spearfishs = {
            (36525, "顶髻鱼", 18, "捕鱼人", 90, 174, path36288, points36288, gatherIndex36288),    // TODO
        };

        // 6.0版本灵砂获取途径 随大版本更新
        public static (int, string)[] CollecMaterialItems = {
            (36285, "月石英"),
            (36286, "暗性岩"),
            (36287, "水瓶土"),
            (36288, "棕榈碎皮"),
            (36408, "红弓鳍鱼"),
            (36525, "顶髻鱼"),
        };

        public static bool IsNeedSpearfish(string text)
        {
            if (Spearfishs.Length > 0 && text.Contains(Spearfishs[0].Name))
            {
                return true;
            }
            return false;
        }

        public static (int, string, uint, string, uint, uint, Vector3[], Vector3[], int[]) GetSpearfish()
        {
            foreach ((int Id, string Name, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, int[] CanGatherIndex) in Spearfishs)
            {
                if (Spearfishs.Length > 0)
                {
                    return Spearfishs[0];
                }
            }
            return (0, null, 0, null, 0, 0, null, null, null);
        }

        public static List<int> GetMaterialIdsByEtAndFinishId(int et, string lv, List<int> finishIds)
        {
            List<int> list = new();
            //Dictionary<int, int> sort = new();
            (int lv0, int lv1) = Util.LevelSplit(lv);
            foreach ((int Id, string Name, int MinEt, int MaxEt, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, uint type) in Materials)
            {
                if (et >= MinEt && et <= MaxEt && Lv >= lv0 && Lv <= lv1)
                {
                    if (!finishIds.Exists(t => t == Id)) {
                        list.Add(Id);
                    }
                }
            }
            return list;
        }

        public static (string, uint, string, uint, uint, Vector3[], Vector3[]) GetMaterialById(int id)
        {
            foreach ((int Id, string Name, int MinEt, int MaxEt, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, uint type) in Materials)
            {
                if (id == Id)
                {
                    return (Name, Job, JobName, Lv, Tp, Path, Points);
                }
            }
            return (null, 0, null, 0, 0, null, null);
        }

        public static int GetCollecMaterialIdByEt(int et, string lv)
        {
            (int lv0, int lv1) = Util.LevelSplit(lv);
            foreach ((int Id, string Name, int MinEt, int MaxEt, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, int[] CanGatherIndex) in CollecMaterials)
            {
                if (et >= MinEt && et <= MaxEt && Lv >= lv0 && Lv <= lv1)
                {
                    return Id;
                }

            }
            return 0;
        }

        public static (int, string, int, int, uint, string, uint, uint, Vector3[], Vector3[], int[]) GetCollecMaterialById(int id)
        {
            foreach ((int Id, string Name, int MinEt, int MaxEt, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, int[] CanGatherIndex) in CollecMaterials)
            {
                if (id == Id)
                {
                    return (Id, Name, MinEt, MaxEt, Job, JobName, Lv, Tp, Path, Points, CanGatherIndex);
                }
            }
            return (0, null, 0, 0, 0, null, 0, 0, null, null, null);
        }
    }
}
