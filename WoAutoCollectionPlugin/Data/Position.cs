﻿using System.Numerics;

namespace WoAutoCollectionPlugin.Utility
{
    public static class Position
    {
        // 天穹街
        public static ushort TianQiongJieTerritoryType = 886;
        //
        public static ushort YunGuanTerritoryType = 939;

        // ------------------------------- 云岛 捕鱼 ------------------------------------ 

        // 去天穹街进入空岛NPC路径 TerritoryType: 886
        public static Vector3[] YunGuanNPC = {
            new Vector3 (1093f, 1093f, 1428f),
            new Vector3 (1091f, 1093f, 1406f),
        };
        public static Vector3 YunGuanRepairNPC = new(869f, 2720f, 1876f);
        // 去区域A的路径 TerritoryType: 930 
        public static Vector3[] ToAreaA = {
            new Vector3 (869f, 2720f, 1876f),
            new Vector3 (913f, 2742f, 1852f),
            new Vector3 (3157f, 2583f, 915f),
            new Vector3 (3214f, 2545f, 854f),
        };
        // 固定钓鱼区域A 0-钓鱼点A1 1-钓鱼点A2 2-移动中转点 TerritoryType: 930 
        public static Vector3[] YFishAreaA = {
            new Vector3 (3254f, 2540f, 873f),
            new Vector3 (3248f, 2540f, 845f),
            new Vector3 (3246f, 2542f, 866f),
        };
        // 去区域B的路径 TerritoryType: 930 
        public static Vector3[] ToAreaB = {
            new Vector3 (866f, 2720f, 1873f),
            new Vector3 (913f, 2742f, 1852f),
            new Vector3 (3187f, 2571f, 1099f),
            new Vector3 (3262f, 2533f, 1119f),
        };
        // 固定钓鱼区域B 0-钓鱼点B1 1-钓鱼点B2 2-移动中转点 TerritoryType: 930 
        public static Vector3[] YFishAreaB = {
            new Vector3 (3297f, 2527f, 1148f),
            new Vector3 (3267f, 2527f, 1145f),
            new Vector3 (3280f, 2529f, 1140f),
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
