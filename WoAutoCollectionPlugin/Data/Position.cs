using Dalamud.Logging;
using System.Collections.Generic;
using System.Numerics;

namespace WoAutoCollectionPlugin.Utility
{
    public static class Position
    {
        // 16-采矿工 17-园艺工 18-捕鱼人

        // 天穹街
        public static ushort TianQiongJieTerritoryType = 886;
        //
        public static ushort YunGuanTerritoryType = 939;

        // ------------------------------- 云岛 采集 ------------------------------------ 

        // 采矿默认移动点
        public static Vector3[] AreaPositionA = {
            new Vector3 (829f, 2730f, 1788f),
            new Vector3 (877f, 2714f, 1781f),
            new Vector3 (885f, 2708f, 1777f), // 2 tp
            new Vector3 (1001f, 2219f, 1675f), // 3
            new Vector3 (1032f, 2214f, 1705f), // 4
            new Vector3 (1099f, 2207f, 1787f),
            new Vector3 (1143f, 2200f, 1796f), // 6
            new Vector3 (1242f, 2199f, 1843f), // 7
            new Vector3 (1305f, 2201f, 1729f), // 8
            new Vector3 (1352f, 2203f, 1649f), // 9
            new Vector3 (1175f, 2214f, 1577f), // 10
            new Vector3 (1131f, 2218f, 1659f),
            new Vector3 (1260f, 2206f, 1641f), // 12
            new Vector3 (1326f, 2206f, 1571f), // 13
            new Vector3 (1418f, 2181f, 1523f), // 14
            new Vector3 (1418f, 2154f, 1435f), // 15
            new Vector3 (1577f, 2158f, 1439f), // 16
            new Vector3 (1646f, 2147f, 1421f), // 
            new Vector3 (1719f, 2153f, 1450f), // 18
            new Vector3 (1818f, 2147f, 1393f), // 19
            new Vector3 (1874f, 2151f, 1189f), // 
            new Vector3 (1817f, 2140f, 1110f), // 21
            new Vector3 (1920f, 2157f, 1004f), // 
            new Vector3 (1995f, 2115f, 960f), // 23
            new Vector3 (2043f, 2131f, 1036f), // 
            new Vector3 (2045f, 2058f, 1102f), // 25
            new Vector3 (2104f, 2115f, 1079f), // 26
            new Vector3 (2154f, 2108f, 1139f), // 
            new Vector3 (2281f, 2064f, 1146f), // 28
            new Vector3 (2361f, 2056f, 1145f), // 29
            new Vector3 (2278f, 2075f, 1005f), // 30
            new Vector3 (2252f, 2084f, 1020f), // 
            new Vector3 (2416f, 2063f, 1111f), // 32
            new Vector3 (2413f, 2065f, 1149f), // 
            new Vector3 (2422f, 2044f, 1183f), // 34 tp
            new Vector3 (2585f, 2739f, 690f), // 35
            new Vector3 (2635f, 2743f, 657f), // 36
            new Vector3 (2627f, 2747f, 605f), // 
            new Vector3 (2751f, 2736f, 616f), // 38
            new Vector3 (2835f, 2751f, 628f), // 
            new Vector3 (2892f, 2734f, 634f), // 40
            new Vector3 (2756f, 2752f, 795f), // 
            new Vector3 (2717f, 2745f, 856f), // 42
            new Vector3 (2744f, 2761f, 806f), // 
            new Vector3 (2827f, 2740f, 715f), // 44
            new Vector3 (2969f, 2740f, 653f), // 
            new Vector3 (3007f, 2724f, 671f), // 46 tp
            new Vector3 (3064f, 2547f, 869f), // 47
            new Vector3 (3122f, 2545f, 800f), // 48
            new Vector3 (3156f, 2550f, 881f), // 49
            new Vector3 (3196f, 2554f, 940f), // 
            new Vector3 (3252f, 2551f, 931f), // 51
            new Vector3 (3264f, 2551f, 1034f), // 52
            new Vector3 (3341f, 2539f, 991f), // 
            new Vector3 (3370f, 2521f, 1037f), // 54
            new Vector3 (3280f, 2565f, 1082f), // 
            new Vector3 (3214f, 2540f, 1103f), // 56
            new Vector3 (3198f, 2532f, 1124f), // 57
            new Vector3 (3202f, 2528f, 1156f), // 58 tp
            new Vector3 (3420f, 2653f, 1303f), // 59
            new Vector3 (3424f, 2652f, 1315f), // 60
            new Vector3 (3430f, 2652f, 1337f), //
            new Vector3 (3423f, 2652f, 1363f), // 62 tp
            new Vector3 (3558f, 2591f, 1537f), // 63
            new Vector3 (3595f, 2596f, 1562f), // 64
            new Vector3 (3569f, 2604f, 1640f), // 
            new Vector3 (3525f, 2598f, 1640f), // 66
            new Vector3 (3639f, 2583f, 1675f), // 67
            new Vector3 (3615f, 2598f, 1787f), // 68
            new Vector3 (3580f, 2600f, 1848f), // 69
            new Vector3 (3587f, 2609f, 1921f), // 70
            new Vector3 (3502f, 2602f, 1951f), // 71
            new Vector3 (3466f, 2602f, 1954f), //
            new Vector3 (3334f, 2605f, 1712f), // 73
            new Vector3 (3420f, 2602f, 1824f), //
            new Vector3 (3465f, 2602f, 1841f), // 75
            new Vector3 (3451f, 2603f, 1963f), // 76
            new Vector3 (3446f, 2602f, 2046f), // 77
            new Vector3 (3466f, 2590f, 2070f), // 78 tp
            new Vector3 (3100f, 2113f, 2557f), // 79
            new Vector3 (3062f, 2113f, 2556f), // 80
            new Vector3 (3024f, 2112f, 2670f), // 81
            new Vector3 (3140f, 2139f, 2698f), // 82
            new Vector3 (3232f, 2140f, 2720f), // 83
            new Vector3 (3380f, 2082f, 2780f), // 84
            new Vector3 (3340f, 2090f, 2842f), //
            new Vector3 (3288f, 2085f, 2879f), // 86
            new Vector3 (3346f, 2096f, 2974f), // 
            new Vector3 (3259f, 2085f, 2984f), // 88
            new Vector3 (3181f, 2100f, 3055f), // 
            new Vector3 (3149f, 2081f, 3077f), // 90
            new Vector3 (3140f, 2140f, 3022f), // 
            new Vector3 (3116f, 2140f, 2836f), // 92
            new Vector3 (3015f, 2143f, 2809f), // 93
            new Vector3 (2925f, 2142f, 2866f), // 94
            new Vector3 (2911f, 2145f, 2991f), //
            new Vector3 (2863f, 2144f, 2994f), // 96
            new Vector3 (2917f, 2146f, 3063f), // 97
            new Vector3 (2908f, 2145f, 3028f), // 
            new Vector3 (2839f, 2133f, 3087f), // 99 tp
            new Vector3 (2589f, 2331f, 3144f), // 100
            new Vector3 (2667f, 2330f, 3228f), // 101
            new Vector3 (2709f, 2339f, 3271f), // 
            new Vector3 (2511f, 2323f, 3387f), // 103
            new Vector3 (2450f, 2348f, 3327f), // 
            new Vector3 (2441f, 2329f, 3242f), // 105
            new Vector3 (2429f, 2330f, 3280f), // 
            new Vector3 (2296f, 2332f, 3285f), // 107
            new Vector3 (2311f, 2331f, 3361f), // 108
            new Vector3 (2240f, 2363f, 3384f), // 
            new Vector3 (2217f, 2356f, 3404f), // 110
            new Vector3 (2184f, 2378f, 3380f), // 111
            new Vector3 (2162f, 2391f, 3428f), // 
            new Vector3 (2020f, 2378f, 3302f), // 113
            new Vector3 (1956f, 2384f, 3267f), // 
            new Vector3 (1978f, 2380f, 3209f), // 115
            new Vector3 (1955f, 2384f, 3239f), // 
            new Vector3 (1870f, 2371f, 3191f), // 117 tp
            new Vector3 (1479f, 2790f, 3225f), // 118
            new Vector3 (1441f, 2781f, 3080f), // 119
            new Vector3 (1352f, 2794f, 3014f), // 120
            new Vector3 (1288f, 2831f, 3005f), // 
            new Vector3 (1163f, 2819f, 2906f), // 122
            new Vector3 (1123f, 2834f, 2956f), // 
            new Vector3 (1119f, 2793f, 2789f), // 124
            new Vector3 (1093f, 2789f, 2813f), // 
            new Vector3 (1043f, 2780f, 2780f), // 126 tp
            new Vector3 (688f, 2692f, 2013f), // 127
            new Vector3 (798f, 2757f, 1856f),
            new Vector3 (835f, 2729f, 1852f),
        };

        // 采矿TP点
        public static int[] TpA = {
            2, 34, 46, 58, 62, 78, 99, 117, 126,
        };

        // 采矿采集点
        public static int[] GatherPositionA = {
            3, 4, 6, 7, 8, 9, 10, 12, 13, 14, 15, 16, 18, 19, 21, 23, 25, 26, 28, 29, 30, 32, 35, 36, 38, 40, 42, 44, 47, 48, 49, 51, 52, 54, 56, 57,
            59, 60, 63, 64, 66, 67, 68, 69, 70, 71, 73, 75, 76, 77, 79, 80, 81, 82, 83, 84, 86, 88, 90, 92, 93, 94, 96, 97, 100, 101, 103, 105,
            107, 108, 110, 111, 113, 115, 118, 119, 120, 122, 124, 127
        };

        // 园艺默认移动点
        public static Vector3[] AreaPositionB = {
            new Vector3 (801f, 2743f, 1867f), // 
            new Vector3 (688f, 2693f, 2014f), //
            new Vector3 (688f, 2693f, 2014f), // 
            new Vector3 (720f, 2692f, 2050f), // 3
            new Vector3 (698f, 2691f, 2080f), // 4 tp
            new Vector3 (1177f, 2855f, 2911f), // 5
            new Vector3 (956f, 2837f, 3037f), // 6
            new Vector3 (1265f, 2827f, 2990f), // 7
            new Vector3 (1386f, 2791f, 3118f), // 8
            new Vector3 (1504f, 2796f, 3293f), // 9
            new Vector3 (1563f, 2786f, 3272f), // 10 tp
            new Vector3 (1911f, 2387f, 3193f), // 11
            new Vector3 (1933f, 2386f, 3141f), // 
            new Vector3 (2033f, 2386f, 3191f), // 13
            new Vector3 (2015f, 2394f, 3204f), // 
            new Vector3 (2183f, 2377f, 3341f), // 15
            new Vector3 (2320f, 2332f, 3295f), // 16
            new Vector3 (2331f, 2332f, 3212f), // 17
            new Vector3 (2385f, 2341f, 3288f), // 
            new Vector3 (2490f, 2328f, 3315f), // 19
            new Vector3 (2475f, 2340f, 3293f), // 
            new Vector3 (2546f, 2330f, 3247f), // 21
            new Vector3 (2577f, 2330f, 3177f), // 22
            new Vector3 (2638f, 2342f, 3325f), // 
            new Vector3 (2676f, 2321f, 3280f), // 24
            new Vector3 (2699f, 2327f, 3168f), // 
            new Vector3 (2659f, 2326f, 3128f), // 
            new Vector3 (2691f, 2320f, 3095f), // 27 tp
            new Vector3 (2900f, 2141f, 3050f), // 28
            new Vector3 (2836f, 2143f, 3034f), // 29
            new Vector3 (2787f, 2148f, 2938f), // 30
            new Vector3 (2962f, 2149f, 2835f), // 31
            new Vector3 (2942f, 2156f, 2835f), // 
            new Vector3 (2973f, 2144f, 3037f), // 33
            new Vector3 (3086f, 2145f, 3042f), // 
            new Vector3 (3136f, 2082f, 3083f), // 35
            new Vector3 (3239f, 2100f, 3002f), // 36
            new Vector3 (3260f, 2089f, 2939f), // 
            new Vector3 (3280f, 2091f, 2845f), // 38
            new Vector3 (3238f, 2148f, 2715f), // 39
            new Vector3 (3015f, 2121f, 2731f), // 40 --
            new Vector3 (3072f, 2123f, 2688f), // 41
            new Vector3 (3047f, 2131f, 2613f), //
            new Vector3 (3039f, 2100f, 2578f), // 43
            new Vector3 (3089f, 2105f, 2555f), // 44
            new Vector3 (3086f, 2097f, 2533f), // 45 tp
            new Vector3 (3490f, 2598f, 2026f), // 46
            new Vector3 (3401f, 2602f, 1991f), // 47
            new Vector3 (3445f, 2609f, 1899f), // 48
            new Vector3 (3592f, 2605f, 1866f), // 49
            new Vector3 (3406f, 2606f, 1758f), // 50
            new Vector3 (3365f, 2643f, 1668f), //  
            new Vector3 (3414f, 2611f, 1643f), // 52
            new Vector3 (3314f, 2624f, 1721f), //  
            new Vector3 (3546f, 2608f, 1722f), // 54
            new Vector3 (3621f, 2603f, 1593f), // 55
            new Vector3 (3562f, 2589f, 1515f), // 56
            new Vector3 (3574f, 2586f, 1487f), // 57 tp
            new Vector3 (3431f, 2654f, 1346f), // 58
            new Vector3 (3429f, 2654f, 1323f), // 59
            new Vector3 (3413f, 2653f, 1298f), // 60
            new Vector3 (3377f, 2653f, 1319f), // 61
            new Vector3 (3374f, 2653f, 1335f), // 
            new Vector3 (3366f, 2653f, 1332f), // 63 tp
            new Vector3 (3212f, 2536f, 1115f), // 64
            new Vector3 (3289f, 2538f, 1134f), // 65
            new Vector3 (3268f, 2536f, 1102f), // 
            new Vector3 (3312f, 2535f, 1050f), // 67
            new Vector3 (3289f, 2549f, 1046f), // 
            new Vector3 (3254f, 2537f, 1043f), // 69
            new Vector3 (3250f, 2563f, 954f), // 70
            new Vector3 (3140f, 2551f, 927f), // 71
            new Vector3 (3069f, 2564f, 931f), // 72
            new Vector3 (3157f, 2550f, 835f), // 73
            new Vector3 (3108f, 2536f, 820f), // 
            new Vector3 (3093f, 2532f, 817f), // 75 tp
            new Vector3 (2889f, 2741f, 649f), // 76
            new Vector3 (2833f, 2729f, 794f), // 77
            new Vector3 (2756f, 2748f, 868f), // 
            new Vector3 (2695f, 2740f, 820f), // 9
            new Vector3 (2634f, 2738f, 760f), //
            new Vector3 (2577f, 2732f, 770f), // 81
            new Vector3 (2538f, 2740f, 725f), // 
            new Vector3 (2575f, 2737f, 670f), // 83
            new Vector3 (2810f, 2743f, 570f), // 84
            new Vector3 (2717f, 2738f, 510f), // 85
            new Vector3 (2593f, 2739f, 644f), // 86
            new Vector3 (2541f, 2735f, 667f), //
            new Vector3 (2508f, 2735f, 668f), // 88 tp
            new Vector3 (2387f, 2052f, 1122f), // 89
            new Vector3 (2364f, 2056f, 1086f), // 90
            new Vector3 (2274f, 2057f, 1131f), // 91
            new Vector3 (2267f, 2085f, 1027f), // 92
            new Vector3 (2146f, 2116f, 875f), // 93
            new Vector3 (2028f, 2111f, 934f), // 94
            new Vector3 (1905f, 2122f, 932f), // 95
            new Vector3 (1835f, 2151f, 1026f), // 96
            new Vector3 (1750f, 2151f, 1128f), // 97
            new Vector3 (1650f, 2147f, 1194f), // 98
            new Vector3 (1463f, 2144f, 1322f), // 99
            new Vector3 (1413f, 2147f, 1455f), // 100
            new Vector3 (1400f, 2187f, 1552f), // 101
            new Vector3 (1316f, 2216f, 1636f), // 102
            new Vector3 (1307f, 2211f, 1718f), // 103
            new Vector3 (1251f, 2211f, 1763f), // 104
            new Vector3 (1264f, 2224f, 1736f), // 
            new Vector3 (1174f, 2200f, 1665f), // 106
            new Vector3 (1080f, 2213f, 1648f), // 107
            new Vector3 (1028f, 2215f, 1748f), // 108
            new Vector3 (1007f, 2225f, 1722f), // 109
            new Vector3 (984f, 2217f, 1665f), // 
            new Vector3 (972f, 2216f, 1681f), // 111 tp
            new Vector3 (852f, 2747f, 1846f), // 
            new Vector3 (886f, 2723f, 1866f), // 112
            new Vector3 (817f, 2742f, 1860f), // 
            new Vector3 (758f, 2709f, 1852f), // 114
            new Vector3 (817f, 2742f, 1860f), // 
            new Vector3 (852f, 2746f, 1846f), // 
        };

        // 园艺TP点
        public static int[] TpB = {
            4, 10, 27, 45, 57, 63, 75, 88, 111
        };

        // 园艺采集点
        public static int[] GatherPositionB = {
            2, 3, 5, 6, 7, 8, 9, 11, 13, 15, 16, 17, 19, 21, 22, 24, 28, 29, 30, 31, 33, 35, 36, 38, 39, 40, 41, 43, 44, 46, 47, 48,
            49, 50, 52, 54, 55, 56, 58, 59, 60, 61, 64,65, 67, 69, 70, 71, 72, 73,  76, 78, 79, 81, 83, 84, 85, 86, 89, 90, 91, 92,
            93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 106, 107, 108, 109, 112, 114
        };

        // ------------------------------- 云岛 捕鱼 ------------------------------------ 

        // 去天穹街进入空岛NPC路径 TerritoryType: 886
        public static Vector3[] YunGuanNPC = {
            new Vector3 (1093f, 1093f, 1428f),
            new Vector3 (1091f, 1093f, 1406f),
        };
        public static Vector3 YunGuanRepairNPC = new(869f, 2720f, 1876f);

        // 去开始点
        public static Vector3[] ToStart = {
            new Vector3 (951, 2764, 1902),
        };

        public static Vector3[] ToGroundA = {
            new Vector3 (951, 2764, 1902),
            new Vector3 (1112, 2381, 2049),
            new Vector3 (1082, 1906, 2349),
            new Vector3 (1251, 1725, 2327),
        };
        public static Vector3[] ToGroundB = {
            new Vector3 (1491, 1917, 2280),
            new Vector3 (2525, 1828, 1480),
            new Vector3 (2576, 1774, 1385),
        };
        public static Vector3[] ToGroundC = {
            new Vector3 (3174, 2580, 976),
            new Vector3 (3077, 2563, 932),
        };
        public static Vector3[] GroundA = {
            new Vector3 (1229, 1720, 2302),
            new Vector3 (1289, 1731, 2311),
        };
        public static Vector3[] GroundB = {
            new Vector3 (2594, 1774, 1385),
            new Vector3 (2559, 1774, 1417),
        };
        public static Vector3[] GroundC = {
            new Vector3 (3076, 2561, 956),
            new Vector3 (3036, 2563, 909),
        };

        // A 狂风云海 灵飘尘  <14 双提 14-24 三提 >24单提
        // B 旋风云海 灵罡风  <12 双提 >12 单提
        // C 摇风云海 灵飞电  <16 双提 >16单提

        // 练级路线
        public static Vector3[] Leveling = {
            new Vector3 (869f, 2720f, 1876f),
            new Vector3 (913f, 2742f, 1852f),
            new Vector3 (3157f, 2583f, 915f),
            new Vector3 (3214f, 2545f, 854f),
        };

        public static Vector3[] LevelingPoints = {
            new Vector3 (869f, 2720f, 1876f),
            new Vector3 (913f, 2742f, 1852f),
            new Vector3 (3157f, 2583f, 915f),
        };

        // ------------------------------- 工票 捕鱼 ------------------------------------
        // 工票随大版本更新而变化
        // Purple 紫票 佛恩·米克 (灰蚯蚓 16.5-25.8) https://fish.ffmomola.com/#/wiki?spotId=279&fishId=36494
        // 去钓鱼区域的路径 TerritoryType: ? 
        public static Vector3[] ToPurpleFishArea = {
            new Vector3 (0f, 0f, 0f)
        };
        // 固定钓鱼区域 0-钓鱼点1 1-钓鱼点2 2-移动中转点
        public static Vector3[] PurpleFishArea = {
            new Vector3 (916f, 2303f, 3107f),
            new Vector3 (955f, 2303f, 3071f),
            new Vector3 (934f, 2303f, 3087f),
        };
        public static int PurpleFishTime = 16 ;

        // White 白票 灵岩之剑 (万能拟饵 11.9-20.6) https://fish.ffmomola.com/#/wiki?spotId=274&fishId=36473
        // 去钓鱼区域的路径 TerritoryType: ? 
        public static Vector3[] ToWhiteFishArea = {
            new Vector3 (0f, 0f, 0f)
        };
        // 固定钓鱼区域 0-钓鱼点1 1-钓鱼点2 2-移动中转点
        public static Vector3[] WhiteFishArea = {
            new Vector3 (2287f, 2193f, 3119f),
            new Vector3 (2253f, 2192f, 3126f),
            new Vector3 (2264f, 2194f, 3107f),
        };
        public static int WhiteFishTime = 11;

        // 普通鱼 萨维奈卡拉墨鱼 (万能拟饵 9.2-13.7) (青花鱼块 4-10.9) https://fish.ffmomola.com/#/wiki?spotId=269&fishId=36686
        // 去钓鱼区域的路径 TerritoryType: ? 
        public static Vector3[] ToNormalFishArea = {
            new Vector3 (0f, 0f, 0f)
        };
        // 固定钓鱼区域 0-钓鱼点1 1-钓鱼点2 2-移动中转点
        public static Vector3[] NormalFishArea = {
            new Vector3 (0f, 0f, 0f),
            new Vector3 (0f, 0f, 0f),
            new Vector3 (0f, 0f, 0f),
        };
        public static int NormalFishTime = 4;


        // ------------------------------- 园艺 ------------------------------------

        // 传送水晶到区域A路径
        //public static Vector3[] TestArea = {
        //    new Vector3 (0f, 0f, 0f),
        //    new Vector3 (0f, 0f, 0f),
        //    new Vector3 (0f, 0f, 0f),
        //};
        // 路径
        public static Vector3[] TestArea = {
            new Vector3 (1698f, 2170f, 2848f), // 进入A
            new Vector3 (1768f, 2157f, 2859f),
            new Vector3 (1816f, 2156f, 2872f),
            new Vector3 (1824f, 2155f, 2876f),
            new Vector3 (1814f, 2154f, 2904f),
            new Vector3 (1729f, 2158f, 2926f), // 离开A
            new Vector3 (1658f, 2164f, 2944f), // 进入B
            new Vector3 (1607f, 2155f, 2953f),
            new Vector3 (1552f, 2160f, 2978f),
            new Vector3 (1515f, 2164f, 2944f),
            new Vector3 (1451f, 2172f, 2906f),
            new Vector3 (1454f, 2197f, 2861f), // 离开B
            new Vector3 (1459f, 2207f, 2675f), // 进入C
            new Vector3 (1459f, 2193f, 2609f),
            new Vector3 (1448f, 2193f, 2610f),
            new Vector3 (1417f, 2198f, 2599f), // 中转
            new Vector3 (1395f, 2194f, 2592f),
            new Vector3 (1444f, 2196f, 2556f),
            new Vector3 (1599f, 2210f, 2794f), // 离开C
        };

        public static int[] TestIndex = {
            1, 2, 3, 4, 7, 8, 9, 10, 13, 14, 16, 17
        };

        public static int[] TestABC = {
            1, 7, 13,
        };

        public static int[] TestIndexNum3 = {
            4, 4, 4
        };

        public static int Gatheing1Button = 3;
        public static int Gatheing2Button = 8;

        public static Vector3[] Area3 = {
            new Vector3 (1651f, 2219f, 2104f), // 进入A
            new Vector3 (1609f, 2216f, 2092f),
            new Vector3 (1599f, 2216f, 2085f),
            new Vector3 (1615f, 2216f, 2060f),
            new Vector3 (1669f, 2234f, 2082f), // 离开A
            new Vector3 (1796f, 2235f, 2159f), // 进入B
            new Vector3 (1854f, 2225f, 2149f),
            new Vector3 (1845f, 2223f, 2192f),
            new Vector3 (1864f, 2228f, 2193f),
            new Vector3 (1855f, 2227f, 2209f),
            new Vector3 (1740f, 2238f, 2279f), // 离开B
            new Vector3 (1643f, 2205f, 2349f), // 进入C
            new Vector3 (1613f, 2182f, 2397f),
            new Vector3 (1600f, 2182f, 2387f),
            new Vector3 (1575f, 2182f, 2352f),
            new Vector3 (1614f, 2186f, 2355f),
            new Vector3 (1651f, 2224f, 2280f), // 离开C
        };

        public static int[] Index3 = {
            1, 2, 3, 6, 7, 8, 9, 12, 13, 14, 15
        };

        public static int[] IndexNum3 = {
            3, 4, 4
        };

        public static int[] ABC3 = {
            1, 6, 12,
        };
        public static int Gatheing3Button = 4;
        // 火水晶
        public static int Gatheing100Button = 5;

        // 葛根
        public static Vector3[] Area4 = {
            new Vector3 (1330f, 2270f, 2492f), // 进入A
            new Vector3 (1366f, 2249f, 2517f),
            new Vector3 (1367f, 2243f, 2456f),
            new Vector3 (1309f, 2245f, 2400f),
            new Vector3 (1279f, 2268f, 2434f), // 离开A
            new Vector3 (1225f, 2262f, 2446f), // 进入B
            new Vector3 (1163f, 2253f, 2379f),
            new Vector3 (1129f, 2260f, 2425f),
            new Vector3 (1129f, 2263f, 2451f), // 中转
            new Vector3 (1151f, 2258f, 2438f),
            new Vector3 (1102f, 2266f, 2498f),
            new Vector3 (1115f, 2274f, 2533f), // 离开B
            new Vector3 (1144f, 2267f, 2589f), // 进入C
            new Vector3 (1153f, 2264f, 2610f),
            new Vector3 (1122f, 2260f, 2627f), // 中转
            new Vector3 (1122f, 2260f, 2627f),
            new Vector3 (1130f, 2261f, 2654f),
            new Vector3 (1136f, 2261f, 2666f),
            new Vector3 (1182f, 2285f, 2627f), // 离开C
        };

        public static int[] Index4 = {
            1, 2, 3, 6, 7, 9, 10, 13, 14, 16, 17
        };

        public static int[] IndexNum4 = {
            3, 4, 4
        };

        public static int[] ABC4 = {
            1, 6, 13,
        };
        public static int Gatheing4Button = 3;

        // ---------------------------------------------------------------------------------------------------
        // ---------------------------------------------------------------------------------------------------

        public static Vector3[] path4814 = {
            new Vector3 (1792, 2219, 2860),
            new Vector3 (2001, 2156, 2933),
        };
        public static Vector3[] Points4814 = {
            new Vector3 (2064, 2154, 2980), // 进入A
            new Vector3 (2065, 2152, 2999),
            new Vector3 (2074, 2152, 3015),
            new Vector3 (2111, 2151, 3005),
            new Vector3 (2127, 2155, 2982), // 离开A
            new Vector3 (2151, 2156, 2949), // 进入B
            new Vector3 (2175, 2156, 2949),
            new Vector3 (2163, 2151, 2891),
            new Vector3 (2159, 2150, 2850),
            new Vector3 (2178, 2150, 2841), // 离开B
            new Vector3 (2178, 2150, 2841), // 进入C
            new Vector3 (2207, 2152, 2826),
            new Vector3 (2188, 2154, 2820),
            new Vector3 (2196, 2154, 2767),
            new Vector3 (2171, 2152, 2775), // 离开C
            new Vector3 (2156, 2152, 2765), // 进入D
            new Vector3 (2153, 2152, 2751),
            new Vector3 (2140, 2152, 2747),
            new Vector3 (2134, 2152, 2752),
            new Vector3 (2136, 2158, 2808), // 离开D
        };
        public static int[] CanCollectPoints4814 = {
            1, 2, 3, 6, 7, 8, 11, 12, 13, 16, 17, 18
        };
        public static int[] UnknownPointsNum4814 = {
            3, 3, 3, 3
        };
        public static int[] Area4814 = {
            1, 6, 11, 16
        };

        public static Vector3[] path4790 = {
            new Vector3 (1853, 2242, 1287),
            new Vector3 (1861, 2248, 1217),
            new Vector3 (1991, 2173, 975),
        };
        public static Vector3[] Points4790 = {
            new Vector3 (1996, 2163, 888), // 进入A
            new Vector3 (1976, 2166, 909),
            new Vector3 (2023, 2159, 961),
            new Vector3 (2077, 2158, 942),
            new Vector3 (2075, 2169, 924), // 离开A
            new Vector3 (2067, 2056, 890), // 进入B
            new Vector3 (2085, 2158, 885),
            new Vector3 (2092, 2159, 879),
            new Vector3 (2107, 2158, 837),
            new Vector3 (2108, 2155, 810), // 离开B
            new Vector3 (2108, 2155, 810), // 进入C
            new Vector3 (2103, 2157, 793),
            new Vector3 (2112, 2158, 788),
            new Vector3 (2051, 2171, 773),
            new Vector3 (2036, 2163, 799), // 离开C
            new Vector3 (2022, 2164, 818), // 进入D
            new Vector3 (2003, 2167, 834),
            new Vector3 (2017, 2162, 852),
            new Vector3 (1972, 2172, 855),
            new Vector3 (1982, 2169, 871), // 离开D
        };
        public static int[] CanCollectPoints4790 = {
            1, 2, 3, 6, 7, 8, 11, 12, 13, 16, 17, 18
        };
        public static int[] UnknownPointsNum4790 = {
            3, 3, 3, 3
        };
        public static int[] Area4790 = {
            1, 6, 11, 16
        };

        public static Vector3[] path19872 = {
            new Vector3 (3103, 2295, 1985),
        };
        public static Vector3[] Points19872 = {
            new Vector3 (3180, 2230, 1991), // 进入A
            new Vector3 (3213, 2231, 1978),
            new Vector3 (3184, 2236, 2003), //
            new Vector3 (3188, 2236, 2008),
            new Vector3 (3186, 2237, 2016),
            new Vector3 (3187, 2241, 2031),
            new Vector3 (3204, 2294, 2082), // 离开A
            new Vector3 (3303, 2313, 2233), // 进入B
            new Vector3 (3337, 2321, 2253),
            new Vector3 (3361, 2326, 2309),
            new Vector3 (3361, 2334, 2320),
            new Vector3 (3296, 2336, 2346), //
            new Vector3 (3301, 2332, 2378),
            new Vector3 (3148, 2332, 2202), // 离开B
            new Vector3 (3028, 2288, 2164), // 进入C
            new Vector3 (3032, 2295, 2184),
            new Vector3 (2985, 2282, 2187),
            new Vector3 (2963, 2280, 2186),
            new Vector3 (2961, 2282, 2167), //
            new Vector3 (2911, 2265, 2158),
            new Vector3 (3125, 2305, 2071), // 离开C
        };
        public static int[] CanCollectPoints19872 = {
            1, 3, 4, 5, 8, 9, 10, 12, 15, 16, 17, 19
        };
        public static int[] UnknownPointsNum19872 = {
            4, 4, 4
        };
        public static int[] Area19872 = {
            1, 8, 15
        };

        public static Vector3[] path27826 = {
            new Vector3 (1735, 2280, 2821),
            new Vector3 (1684, 2220, 2823),
        };
        public static Vector3[] Points27826 = {
            new Vector3 (1631, 2158, 2893), // 进入A
            new Vector3 (1636, 2155, 2913),
            new Vector3 (1617, 2154, 2899),
            new Vector3 (1578, 2160, 2919),
            new Vector3 (1586, 2158, 2887),
            new Vector3 (1354, 2192, 2750), // 离开A
            new Vector3 (1300, 2168, 2711), // 进入B
            new Vector3 (1251, 2162, 2678),
            new Vector3 (1278, 2162, 2676),
            new Vector3 (1288, 2171, 2665), //
            new Vector3 (1298, 2169, 2654),
            new Vector3 (1317, 2170, 2647),
            new Vector3 (1442, 2191, 2618), // 离开B
            new Vector3 (1548, 2181, 2584), // 进入C
            new Vector3 (1559, 2180, 2566),
            new Vector3 (1607, 2190, 2563),
            new Vector3 (1619, 2188, 2599),
            new Vector3 (1590, 2186, 2592),
            new Vector3 (1602, 2198, 2712), // 离开C
        };
        public static int[] CanCollectPoints27826 = {
            1, 2, 3, 4, 7, 8, 10, 11, 14, 15, 16, 17
        };
        public static int[] UnknownPointsNum27826 = {
            4, 4, 4
        };
        public static int[] Area27826 = {
            1, 7, 14
        };

        public static Vector3[] path27824 = {
            new Vector3 (1304, 2269, 2477),
            new Vector3 (1309, 2229, 2379),
        };
        public static Vector3[] Points27824 = {
            new Vector3 (1414, 2159, 2336), // 进入A
            new Vector3 (1442, 2153, 2332),
            new Vector3 (1419, 2153, 2299),
            new Vector3 (1406, 2153, 2276),
            new Vector3 (1411, 2153, 2268),
            new Vector3 (1351, 2171, 2208), // 离开A
            new Vector3 (1212, 2165, 2069), // 进入B
            new Vector3 (1184, 2159, 2042),
            new Vector3 (1155, 2162, 2039),
            new Vector3 (1146, 2211, 2192), // 离开B
            new Vector3 (1126, 2225, 2386), // 进入C
            new Vector3 (1121, 2226, 2405),
            new Vector3 (1148, 2228, 2437),
            new Vector3 (1163, 2216, 2393),
            new Vector3 (1188, 2210, 2428),
            new Vector3 (1262, 2201, 2392), // 离开C
        };
        public static int[] CanCollectPoints27824 = {
            1, 2, 3, 4, 7, 8, 11, 12, 13, 14
        };
        public static int[] UnknownPointsNum27824 = {
            4, 2, 4
        };
        public static int[] Area27824 = {
            1, 7, 11
        };

        public static Vector3[] path27832 = {
            new Vector3 (1747, 2198, 3583),
        };
        public static Vector3[] Points27832 = {
            new Vector3 (1770, 2176, 3644), // 进入A
            new Vector3 (1788, 2174, 3655),
            new Vector3 (1776, 2172, 3672),
            new Vector3 (1792, 2168, 3710),
            new Vector3 (1781, 2168, 3727),
            new Vector3 (1755, 2183, 3667), // 离开A
            new Vector3 (1581, 2190, 3751), // 进入B
            new Vector3 (1565, 2191, 3766),
            new Vector3 (1545, 2193, 3750),
            new Vector3 (1529, 2193, 3737),
            new Vector3 (1497, 2194, 3728),
            new Vector3 (1533, 2202, 3646), // 离开B
            new Vector3 (1588, 2188, 3460), // 进入C
            new Vector3 (1595, 2186, 3440),
            new Vector3 (1614, 2186, 3401),
            new Vector3 (1617, 2183, 3429),
            new Vector3 (1633, 2183, 3429),
            new Vector3 (1686, 2197, 3511), // 离开C
        };
        public static int[] CanCollectPoints27832 = {
            1, 2, 3, 4, 7, 8, 9, 10, 13, 14, 15, 16
        };
        public static int[] UnknownPointsNum27832 = {
            4, 4, 4
        };
        public static int[] Area27832 = {
            1, 7, 13
        };

        public static Vector3[] path27687 = {
            new Vector3 (1167, 2375, 1069),
        };
        public static Vector3[] Points27687 = {
            new Vector3 (1241, 2358, 922), // 进入A
            new Vector3 (1269, 2354, 903),
            new Vector3 (1255, 2356, 871),
            new Vector3 (1304, 2355, 880),
            new Vector3 (1344, 2349, 873),
            new Vector3 (1301, 2367, 906), // 离开A
            new Vector3 (1398, 2359, 1045), // 进入B
            new Vector3 (1420, 2358, 1054),
            new Vector3 (1440, 2351, 1110),
            new Vector3 (1367, 2357, 1078),
            new Vector3 (1343, 2349, 1100),
            new Vector3 (1222, 2352, 1120), // 离开B
            new Vector3 (1050, 2305, 1172), // 进入C
            new Vector3 (1027, 2298, 1179),
            new Vector3 (1066, 2310, 1116),
            new Vector3 (1018, 2317, 1083),
            new Vector3 (1083, 2319, 1048),
            new Vector3 (1175, 2366, 983), // 离开C
        };
        public static int[] CanCollectPoints27687 = {
            1, 2, 3, 4, 7, 8, 9, 10, 13, 14, 15, 16
        };
        public static int[] UnknownPointsNum27687 = {
            4, 4, 4
        };
        public static int[] Area27687 = {
            1, 7, 13
        };

        public static Vector3[] path27703 = {
            new Vector3 (3599, 2270, 1660),
        };
        public static Vector3[] Points27703 = {
            new Vector3 (3541, 2210, 1597), // 进入A
            new Vector3 (3519, 2215, 1628),
            new Vector3 (3553, 2217, 1628),  //
            new Vector3 (3571, 2205, 1643),
            new Vector3 (3598, 2215, 1654),
            new Vector3 (3608, 2212, 1628),
            new Vector3 (3699, 2261, 1476), // 离开A
            new Vector3 (3711, 2252, 1388), // 进入B
            new Vector3 (3753, 2251, 1399), 
            new Vector3 (3716, 2250, 1381), //
            new Vector3 (3739, 2260, 1369),
            new Vector3 (3732, 2258, 1360),
            new Vector3 (3714, 2256, 1362),
            new Vector3 (3595, 2287, 1301), // 离开B
            new Vector3 (3562, 2267, 1299), // 进入C
            new Vector3 (3552, 2269, 1283),
            new Vector3 (3525, 2276, 1268),
            new Vector3 (3495, 2273, 1267), // 
            new Vector3 (3500, 2273, 1265),
            new Vector3 (3493, 2283, 1240),
            new Vector3 (3573, 2265, 1520), // 离开C
        };
        public static int[] CanCollectPoints27703 = {
            1, 3, 4, 5, 8, 10, 11, 12, 15, 16, 18, 19
        };
        public static int[] UnknownPointsNum27703 = {
            4, 4, 4
        };
        public static int[] Area27703 = {
            1, 8, 15
        };

        public static Vector3[] path19915 = {
            new Vector3 (2007, 2483, 2488),
            new Vector3 (1712, 2184, 2827),
        };
        public static Vector3[] Points19915 = {
            new Vector3 (1753, 2158, 2853), // 进入A
            new Vector3 (1768, 2157, 2859),
            new Vector3 (1815, 2156, 2872),
            new Vector3 (1824, 2155, 2876),
            new Vector3 (1814, 2154, 2904),
            new Vector3 (1775, 2176, 2923), // 离开A
            new Vector3 (1653, 2160, 2945), // 进入B
            new Vector3 (1608, 2155, 2953),
            new Vector3 (1552, 2160, 2978), 
            new Vector3 (1516, 2160, 2945),
            new Vector3 (1452, 2172, 2907),
            new Vector3 (1467, 2201, 2803), // 离开B
            new Vector3 (1472, 2195, 2657), // 进入C
            new Vector3 (1460, 2193, 2610),
            new Vector3 (1449, 2193, 2611),
            new Vector3 (1431, 2198, 2604), // 
            new Vector3 (1396, 2192, 2592),
            new Vector3 (1442, 2196, 2558),
            new Vector3 (1658, 2213, 2763), // 离开C
        };
        public static int[] CanCollectPoints19915 = {
            1, 2, 3, 4, 7, 8, 9, 10, 13, 14, 16, 17
        };
        public static int[] UnknownPointsNum19915 = {
            4, 4, 4
        };
        public static int[] Area19915 = {
            1, 7, 13
        };



        public static (int Id, int MaxBackPack, string Name, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, int[] CanCollectPoints, int[] UnknownPointsNum, int[] Area)[] NormalMaterials = {
            (4814, 2, "血红奇异果", 17, "园艺工", 50, 6, path4814, Points4814, CanCollectPoints4814, UnknownPointsNum4814, Area4814),    // 摩杜纳
            (4790, 2, "芦荟", 17, "园艺工", 50, 19, path4790, Points4790, CanCollectPoints4790, UnknownPointsNum4790, Area4790),    // 南萨纳兰
            //(12534, 2, "灵银矿", 16, "采矿工", 70, 107, path19872, Points19872, CanCollectPoints19872, UnknownPointsNum19872, Area19872), //
            (19872, 2, "石间清水", 16, "采矿工", 70, 107, path19872, Points19872, CanCollectPoints19872, UnknownPointsNum19872, Area19872), // 延夏
            //(19853, 8, "葛根", 16, "采矿工", 80, 132, path27703, Points27703, CanCollectPoints27703, UnknownPointsNum27703, Area27703), //
            (19914, 2, "繁缕", 17, "园艺工", 70, 110, path19915, Points19915, CanCollectPoints19915, UnknownPointsNum19915, Area19915), // 延夏
            (19915, 2, "稻槎草", 17, "园艺工", 70, 110, path19915, Points19915, CanCollectPoints19915, UnknownPointsNum19915, Area19915), // 延夏

            // (27759, 8, "矮人棉", 17, "园艺工", 80, 144, path27824, Points27824, CanCollectPoints27824, UnknownPointsNum27824, Area27824), //
            // (27803, 3, "凝灰岩", 17, "园艺工", 80, 144, path27824, Points27824, CanCollectPoints27824, UnknownPointsNum27824, Area27824), //
            (27824, 8, "野园甜菜", 17, "园艺工", 80, 144, path27824, Points27824, CanCollectPoints27824, UnknownPointsNum27824, Area27824), // 伊尔美格
            (27826, 8, "皇家葡萄", 17, "园艺工", 80, 142, path27826, Points27826, CanCollectPoints27826, UnknownPointsNum27826, Area27826), // 拉凯提卡大森林
            (27832, 8, "山地小麦", 17, "园艺工", 80, 138, path27832, Points27832, CanCollectPoints27832, UnknownPointsNum27832, Area27832), // 珂露西亚岛
            //(27834, 8, "大蜜蜂的巢", 17, "园艺工", 80, 138, path27832, Points27832, CanCollectPoints27832, UnknownPointsNum27832, Area27832), //
            (27687, 8, "愈疮木原木", 17, "园艺工", 80, 136, path27687, Points27687, CanCollectPoints27687, UnknownPointsNum27687, Area27687), // 雷克兰德 
            //(27703, 8, "暗银矿", 16, "采矿工", 80, 132, path27703, Points27703, CanCollectPoints27703, UnknownPointsNum27703, Area27703), // 雷克兰德 tt

            //(36083, 8, "裸麦", 16, "采矿工", 90, 132, path27703, Points27703, CanCollectPoints27703, UnknownPointsNum27703, Area27703), //
            //(36084, 5, "北洋岩盐", 16, "采矿工", 90, 132, path27703, Points27703, CanCollectPoints27703, UnknownPointsNum27703, Area27703), //
            //(36085, 9, "萨维奈紫苏", 16, "采矿工", 90, 132, path27703, Points27703, CanCollectPoints27703, UnknownPointsNum27703, Area27703), // 
            //(36086, 10, "棕榈糖浆", 16, "采矿工", 90, 132, path27703, Points27703, CanCollectPoints27703, UnknownPointsNum27703, Area27703), // 
            //(36087, 5, "椰子", 16, "采矿工", 90, 132, path27703, Points27703, CanCollectPoints27703, UnknownPointsNum27703, Area27703), // 
            //(36089, 10, "巨人新薯", 16, "采矿工", 90, 132, path27703, Points27703, CanCollectPoints27703, UnknownPointsNum27703, Area27703), //
        };

        public static (int, int, string, uint, string, uint, uint, Vector3[], Vector3[], int[], int[], int[]) GetMaterialById(int id)
        {
            foreach ((int Id, int MaxBackPack, string Name, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, int[] CanCollectPoints, int[] UnknownPointsNum, int[] Area) in NormalMaterials)
            {
                if (id == Id)
                {
                    return (Id, MaxBackPack, Name, Job, JobName, Lv, Tp, Path, Points, CanCollectPoints, UnknownPointsNum, Area);
                }
            }
            return (0, 0, null, 0, null, 0, 0,  null, null, null, null, null);
        }

        public static List<int> GetMateriaId(uint lv)
        {
            List<int> list = new();
            foreach ((int Id, int MaxBackPack, string Name, uint Job, string JobName, uint Lv, uint Tp, Vector3[] Path, Vector3[] Points, int[] CanCollectPoints, int[] UnknownPointsNum, int[] Area) in NormalMaterials)
            {
                if (lv >= Lv)
                {
                    list.Add(Id);
                }
            }
            return list;
        }
    }


}
