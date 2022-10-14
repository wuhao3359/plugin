using System.Numerics;

namespace WoAutoCollectionPlugin.Utility
{
    public static class Position
    {
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
            new Vector3 (1177f, 2831f, 2911f), // 5
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
        // 去区域的路径 TerritoryType: 930 0-钓鱼点A1 1-钓鱼点A1 ... 
        public static Vector3[] ToArea1 = {
            new Vector3 (869f, 2720f, 1876f),
            new Vector3 (913f, 2742f, 1852f),
            new Vector3 (3157f, 2583f, 915f),
            new Vector3 (3214f, 2545f, 854f),
        };
        public static Vector3[] YFishArea1 = {
            new Vector3 (3254f, 2540f, 873f),
            new Vector3 (3248f, 2540f, 845f),
        };
        public static Vector3[] ToArea11 = {
            new Vector3 (869f, 2720f, 1876f),
            new Vector3 (913f, 2742f, 1852f),
            new Vector3 (3157f, 2583f, 915f),
            new Vector3 (3214f, 2545f, 854f),
        };
        public static Vector3[] YFishArea11 = {
            new Vector3 (3254f, 2540f, 873f),
            new Vector3 (3248f, 2540f, 845f),
        };
        public static Vector3[] ToAreaB = {
            new Vector3 (866f, 2720f, 1873f),
            new Vector3 (913f, 2742f, 1852f),
            new Vector3 (3187f, 2571f, 1099f),
            new Vector3 (3313f, 2551f, 1042f),
            new Vector3 (3342f, 2522f, 988f),
        };
        public static Vector3[] YFishAreaB = {
            new Vector3 (3310f, 2524f, 942f),
            new Vector3 (3339f, 2524f, 968f),
            new Vector3 (3326f, 2525f, 958f),
        };
        public static Vector3[] ToAreaC = {
            new Vector3 (869f, 2720f, 1876f),
            new Vector3 (913f, 2742f, 1852f),
            new Vector3 (3094f, 2581f, 867f),
            new Vector3 (3124f, 2533f, 800f),
            new Vector3 (3150f, 2531f, 792f),
        };
        // 固定钓鱼区域C 0-钓鱼点B1 1-钓鱼点B2 2-移动中转点 TerritoryType: 930 
        public static Vector3[] YFishAreaC = {
            new Vector3 (3196f, 2526f, 822f),
            new Vector3 (3165f, 2530f, 791f),
            new Vector3 (3180f, 2531f, 809f),
        };
        public static Vector3[] ToArea100 = {
            new Vector3 (866f, 2720f, 1873f),
            new Vector3 (861f, 2721f, 1891f),
            new Vector3 (1003f, 2831f, 2507f),
            new Vector3 (1074f, 2785f, 2788f),
        };
        public static Vector3[] YFishArea100 = {
            new Vector3 (1163f, 2785f, 2814f),
            new Vector3 (1110f, 2785f, 2787f),
            new Vector3 (1134f, 2785f, 2807f),
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
            new Vector3 (0f, 0f, 0f),
            new Vector3 (0f, 0f, 0f),
            new Vector3 (0f, 0f, 0f),
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
    }


}
