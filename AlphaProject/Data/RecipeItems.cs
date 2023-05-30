
using System.Collections.Generic;

namespace AlphaProject.Data
{
    public class RecipeItems
    {
        public static readonly (string Name, uint Category, uint Sub, uint ItemId)[] UploadItems =
        {
            ("收藏用无花果冻糕", 0, 1, 36626),   // ItemId 36626
            ("灵岩之剑", 0, 13, 36473),
            ("佛恩·米克", 0, 7, 36494),
            ("红弓鳍鱼", 0, 0, 36408),
            ("噬卵者", 0, 0, 36497),
        };

        public static readonly (int Id, uint Category, uint Sub)[] ExchangeItems =
        {
            (1, 1, 1),   // TEST
            (2, 1, 1),
        };

        public static (uint Category, uint Sub, uint ItemId) UploadApply(string ItemName)
        {
            foreach ((string Name, uint Category, uint Sub, uint ItemId) in UploadItems)
            {
                if (ItemName.Contains(Name)) { 
                    return (Category, Sub, ItemId);
                }
            }
            return (0, 0, 0);
        }

        public static (uint Category, uint Sub) ExchangeApply(int ItemId)
        {
            foreach (var (Id, Category, Sub) in ExchangeItems)
            {
                if (Id == ItemId)
                {
                    return (Category, Sub);
                }
            }
            return (0, 0);
        }

        // 8-刻木匠 9-锻铁匠 10-铸甲匠 11-雕金匠 12-制革匠 13-裁衣匠
        // 14-炼金术士 15-烹调师
        public static (uint Id, string Name, byte GetWay)[] Items = {

        };

        public static (uint Id, string Name, byte GetWay) GetItem(uint Id)
        {
            foreach ((uint id, string name, byte getWay) in Items)
            {
                if (id == Id)
                {
                    return (id, name, getWay);
                }
            }
            return (0, null, 0);
        }


        public static (int Id, string Name, int Quantity, bool Craft)[] Craft0 = {
        };
        public static (int Id, string Name, int Quantity, bool Craft)[] Craft31652 = {
            (27693, "愈疮木木材", 2, true),
            (27714, "矮人银锭", 1, true),
            (27804, "凝灰岩磨刀石", 1, true),
        };
        public static (int Id, string Name, int Quantity, bool Craft)[] Craft31945 = {
            (31983, "第四期重建用的最高级小麦（检）", 10, false),
            (31984, "第四期重建用的最高级棉花（检）", 10, false),
            (31981, "第四期重建用的最高级白雪松原木（检）", 10, false),
            (27757, "矮人棉布", 1, true),
        };
        public static (int Id, string Name, int Quantity, bool Craft)[] Craft27757 = {
            (27758, "矮人棉线", 3, false),
        };
        public static (int Id, string Name, int Quantity, bool Craft)[] Craft27758 = {
            (27759, "矮人棉", 4, false),
        };

        public static (int Id, string Name, int Quantity, bool Craft)[] Craft31950 = {
            (31982, "第四期重建用的最高级古代树脂（检）", 10, false),
            (31988, "第四期重建用的最高级天然水（检）", 10, false),
            (31984, "第四期重建用的最高级棉花（检）", 10, false),
            (27693, "愈疮木木材", 1, true),
        };


        public static (int Id, string Name, int Quantity)[] Craft27804 = {
            (27803, "凝灰岩", 7),
            (10, "风之水晶", 3),
        };
        public static (int Id, string Name, int Quantity)[] Craft27839 = {
            (27824, "野园甜菜", 5),
            (8, "火之水晶", 6),
        };
        public static (int Id, string Name, int Quantity)[] Craft27843 = {
            (27826, "皇家葡萄", 5),
            (8, "火之水晶", 7),
        };
        public static (int Id, string Name, int Quantity)[] Craft27841 = {
            (27832, "山地小麦", 5),
            (8, "火之水晶", 7),
        };
        public static (int Id, string Name, int Quantity)[] Craft27693 = {
            (27687, "愈疮木原木", 4),
            (10, "风之水晶", 7),
        };
        public static (int Id, string Name, int Quantity)[] Craft27714 = {
            (27703, "暗银矿", 4),
            (12534, "灵银矿 ", 1),
            (9, "冰之水晶", 7),
        };
        // 空闲时间快速生产任务填充
        public static (int Id, string Name, uint Job, string JobName, uint Lv, (int Id, string Name, int Quantity)[])[] QuickCrafts = {
            (27804, "凝灰岩磨刀石", 11, "雕金匠", 80, Craft27804),
            (27693, "愈疮木木材", 8, "刻木匠 ", 80, Craft27693),
            (27714, "矮人银锭", 10, "铸甲匠 ", 80, Craft27714),
            (27839, "野园甜菜糖 ", 15, "烹调师", 80, Craft27839),
            (27843, "黑夜醋", 15, "烹调师", 80, Craft27843),
            (27841, "山地小麦粉", 15, "烹调师 ", 80, Craft27841),
        };

        public static (int Id, int MaxBackPack, string Name, uint Job, string JobName, uint Lv, bool QuickCraft, (int Id, string Name, int Quantity, bool Craft)[])[] CraftItems = {
            (0, 0, "", 0, "", 0, false, Craft0),
            (31652, 0, "收藏用愈疮木砂轮机", 8, "刻木匠", 80, false, Craft31652),   // lv.80以下 
            (31945, 0, "第四期重建用的睡床", 8, "刻木匠", 80, false, Craft31945),
            (27757, 2, "矮人棉布", 13, "裁衣匠", 80, true, Craft27757),
            (27758, 4, "矮人棉线", 13, "裁衣匠", 80, true, Craft27758),

            (31950, 0, "第四期重建用的遮蓬", 13, "裁衣匠", 80, false, Craft31950),
        };

        public static (int Id, int MaxBackPack, string Name, uint Job, string JobName, uint Lv, bool QuickCraft, (int Id, string Name, int Quantity, bool Craft)[]) GetCraftItems(int Id)
        {
            for (int i = 1; i < CraftItems.Length; i ++) {
                if (CraftItems[i].Id == Id) {
                    return CraftItems[i];
                }
            }
            return CraftItems[0];
        }

        public static List<int> GetCraftItemIds()
        {
            List<int> list = new();
            for (int i = 1; i < CraftItems.Length; i++)
            {
                list.Add(CraftItems[i].Id);
            }
            return list;
        }

        public static (int Id, string Name, uint Job, string JobName, uint Lv, (int Id, string Name, int Quantity)[]) GetMidCraftItems(int Id, List<uint> JobIds)
        {
            for (int i = 1; i < QuickCrafts.Length; i++)
            {
                if (QuickCrafts[i].Id == Id && !JobIds.Contains(QuickCrafts[i].Job))
                {
                    return QuickCrafts[i];
                }
            }
            return QuickCrafts[0];
        }

        public static (int Id, string Name, uint Job, string JobName, uint Lv, (int Id, string Name, int Quantity)[]) GetMidCraftItemById(int Id)
        {
            for (int i = 1; i < QuickCrafts.Length; i++)
            {
                if (QuickCrafts[i].Id == Id)
                {
                    return QuickCrafts[i];
                }
            }
            return QuickCrafts[0];
        }

        public static List<int> GetAllQuickCraftItems()
        {
            List<int> list = new();
            for (int i = 1; i < QuickCrafts.Length; i++)
            {
                list.Add((int)QuickCrafts[i].Id);
            }
            return list;
        }

    }
}
