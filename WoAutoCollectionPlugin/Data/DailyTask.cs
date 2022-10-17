using System.Collections.Generic;
using System.Numerics;

namespace WoAutoCollectionPlugin.Utility
{
    public static class DailyTask
    {
        public static Vector3[] path1 = { 
            new Vector3(801f, 2743f, 1867f) 
        };

        // 限时点
        public static readonly (int Id, string Name, int MinEt, int MaxEt, string Job, int GatherIndex, uint tp, Vector3[] path)[] Materials =
        {
            (0, "暗物质晶簇", 9, 9, "园艺工", 10, 10, path1),
            (1, "暗物质晶簇", 9, 9, "园艺工", 10, 11, path1),
            (2, "暗物质晶簇", 1, 1, "园艺工", 10, 24, path1),
            (3, "暗物质晶簇", 5, 5, "园艺工", 10, 24, path1),
            (4, "暗物质晶簇", 9, 9, "园艺工", 10, 24, path1),
            (5, "暗物质晶簇", 1, 1, "园艺工", 10, 4, path1),
            (6, "暗物质晶簇", 5, 5, "园艺工", 10, 11, path1),
            (7, "暗物质晶簇", 13, 13, "园艺工", 10, 18, path1),
            (8, "暗物质晶簇", 3, 3, "园艺工", 10, 7, path1),
            (9, "暗物质晶簇", 9, 9, "园艺工", 10, 52, path1),
            (10, "暗物质晶簇", 7, 7, "园艺工", 10, 15, path1),
            (11, "暗物质晶簇", 2, 2, "园艺工", 10, 3, path1),
            (12, "暗物质晶簇", 8, 8, "园艺工", 10, 13, path1),
            (13, "暗物质晶簇", 6, 6, "园艺工", 10, 10, path1),
            (14, "暗物质晶簇", 2, 2, "园艺工", 10, 6, path1),
            (15, "暗物质晶簇", 6, 6, "园艺工", 10, 3, path1),
            (16, "暗物质晶簇", 4, 4, "园艺工", 10, 19, path1),
            (16, "暗物质晶簇", 4, 4, "采矿工", 10, 19, path1),
            (17, "暗物质晶簇", 2, 2, "园艺工", 10, 76, path1),
            (18, "暗物质晶簇", 14, 14, "园艺工", 10, 76, path1),
            (19, "暗物质晶簇", 1, 1, "采矿工", 10, 23, path1),
            (20, "暗物质晶簇", 9, 9, "采矿工", 10, 18, path1),
            (21, "暗物质晶簇", 17, 17, "采矿工", 10, 11, path1),
            (22, "暗物质晶簇", 21, 21, "采矿工", 10, 23, path1),
            (23, "暗物质晶簇", 5, 5, "采矿工", 10, 18, path1),
            (24, "暗物质晶簇", 5, 5, "采矿工", 10, 18, path1),
            (25, "暗物质晶簇", 13, 13, "采矿工", 10, 24, path1),
            (26, "暗物质晶簇", 17, 17, "采矿工", 10, 24, path1),
            (27, "暗物质晶簇", 21, 21, "采矿工", 10, 24, path1),
            (28, "暗物质晶簇", 3, 3, "采矿工", 10, 22, path1),
            (29, "暗物质晶簇", 4, 4, "采矿工", 10, 53, path1),
            (30, "暗物质晶簇", 18, 18, "采矿工", 10, 10, path1),
            (31, "暗物质晶簇", 19, 19, "采矿工", 10, 52, path1),
            (32, "暗物质晶簇", 6, 6, "采矿工", 10, 6, path1),
            (33, "暗物质晶簇", 5, 5, "采矿工", 10, 17, path1),
            (34, "迷迭香", 17, 17, "园艺工", 10, 4, path1),
            (35, "延龄花", 5, 5, "园艺工", 10, 4, path1),
            (36, "黄杏", 9, 9, "园艺工", 10, 52, path1),
        };

        public static List<int> GetMaterialIdsByEt(int et)
        {
            List<int> list = new List<int>();
            foreach ((int Id, string Name, int MinEt, int MaxEt, string Job, int GatherIndex, uint tp, Vector3[] path) in Materials)
            {
                if (et >= MinEt && et <= MaxEt)
                {
                    list.Add(Id);
                }
            }
            return list;
        }

        public static (string Name, string Job, int GatherIndex, uint tp, Vector3[] path) GetMaterialById(int id)
        {
            foreach ((int Id, string Name, int MinEt, int MaxEt, string Job, int GatherIndex, uint tp, Vector3[] path) in Materials)
            {
                if (id == Id)
                {
                    return (Name, Job, GatherIndex, tp, path);
                }
            }
            return (null, null, -1, 0, null);
        }
    }
}
