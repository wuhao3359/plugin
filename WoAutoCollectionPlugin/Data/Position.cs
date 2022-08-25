using System.Numerics;

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
        public static Vector3 YunGuanRepairNPC = new(866f, 2720f, 1873f);
        // 去区域A的路径 TerritoryType: 930 
        public static Vector3[] ToAreaA = {
            new Vector3 (866f, 2720f, 1873f),
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
        // Purple 紫票 佛恩·米克 灰蚯蚓 13.5-25.8 https://fish.ffmomola.com/#/wiki?spotId=279&fishId=36494
        // 去钓鱼区域的路径 TerritoryType: ? 
        public static Vector3[] ToPurpleFishArea = {
            new Vector3 (0f, 0f, 0f)
        };
        // 固定钓鱼区域 0-钓鱼点1 1-钓鱼点2 2-移动中转点
        public static Vector3[] PurpleFishArea = {
            new Vector3 (0f, 0f, 0f),
            new Vector3 (0f, 0f, 0f),
            new Vector3 (0f, 0f, 0f),
        };

        // White 白票 灵岩之剑 万能拟饵 11.9-20.6 https://fish.ffmomola.com/#/wiki?spotId=274&fishId=36473
        // 去钓鱼区域的路径 TerritoryType: ? 
        public static Vector3[] ToWhithFishArea = {
            new Vector3 (0f, 0f, 0f)
        };
        // 固定钓鱼区域 0-钓鱼点1 1-钓鱼点2 2-移动中转点
        public static Vector3[] WhiteFishArea = {
            new Vector3 (0f, 0f, 0f),
            new Vector3 (0f, 0f, 0f),
            new Vector3 (0f, 0f, 0f),
        };


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
    }
}
