
using System.Collections.Generic;

namespace WoAutoCollectionPlugin.Data
{
    public class RecipeItems
    {
        public static readonly (string Name, uint Category, uint Sub, uint ItemId)[] UploadItems =
        {
            ("收藏用无花果冻糕", 0, 1, 36626),   // ItemId 36626
        };

        public static readonly (int Id, uint Category, uint Sub)[] ExchangeItems =
        {
            (1, 1, 1),   // TEST
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
        public static (int Id, string Name, int Quantity, bool Craft)[] Craft0 = {
        };
        public static (int Id, string Name, int Quantity, bool Craft)[] Craft31652 = {
            (27693, "愈疮木木材", 2, true),
            (27714, "矮人银锭", 1, true),
            (27804, "凝灰岩磨刀石", 1, true),
        };
        public static (int Id, string Name, int Quantity, bool Craft)[] Craft27693 = {
            (27687, "愈疮木原木", 2, false),
        };
        public static (int Id, string Name, int Quantity, bool Craft)[] Craft27714 = {
            (27703, "暗银矿", 4, false),
            (12534, "灵银矿 ", 1, false),
        };
        public static (int Id, string Name, int Quantity, bool Craft)[] Craft27804 = {
            (27803, "凝灰岩 ", 3, false),
        };

        public static (int Id, string Name, int Quantity, bool Craft)[] Craft31945 = {
            (31983, "第四期重建用的最高级小麦（检）", 10, true),
            (31984, "第四期重建用的最高级棉花（检）", 10, true),
            (31981, "第四期重建用的最高级白雪松原木（检）", 10, true),
            (27757, "矮人棉布", 1, true),
        };
        public static (int Id, string Name, int Quantity, bool Craft)[] Craft27757 = {
            (27758, "矮人棉线", 3, false),
        };
        public static (int Id, string Name, int Quantity, bool Craft)[] Craft27758 = {
            (27759, "矮人棉", 4, false),
        };

        public static (int Id, string Name, int Quantity, bool Craft)[] Craft31950 = {
            (31982, "第四期重建用的最高级古代树脂（检）", 10, true),
            (31988, "第四期重建用的最高级天然水（检）", 10, true),
            (31984, "第四期重建用的最高级棉花（检）", 10, true),
            (27693, "愈疮木木材", 1, true),
        };

        public static (int Id, string Name, int Quantity, bool Craft)[] Craft27843 = {
            (27826, "皇家葡萄", 5, false),
        };

        public static (int Id, int MaxBackPack, string Name, uint Job, string JobName, uint Lv, bool QuickCraft, (int Id, string Name, int Quantity, bool Craft)[])[] CraftItems = {
            (0, 0, "", 0, "", 0, false, Craft0),
            (31652, 0, "收藏用愈疮木砂轮机", 8, "刻木匠", 80, false, Craft31652),
            (27693, 4, "愈疮木木材", 8, "刻木匠", 80, true, Craft27693),
            (27714, 2, "矮人银锭", 10, "铸甲匠", 80, true, Craft27714),
            (27804, 2, "凝灰岩磨刀石", 11, "雕金匠", 80, true, Craft27804),

            (31945, 0, "第四期重建用的睡床", 8, "刻木匠", 80, false, Craft31945),
            (27757, 2, "矮人棉布", 13, "裁衣匠", 80, true, Craft27757),
            (27758, 4, "矮人棉线", 13, "裁衣匠", 80, true, Craft27758),

            (31950, 0, "第四期重建用的遮蓬", 13, "裁衣匠", 80, false, Craft31950),

            (27843, 3, "黑夜醋", 15, "烹调师", 80, true, Craft27843),
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
            List<int> list = new List<int>();
            for (int i = 1; i < CraftItems.Length; i++)
            {
                list.Add(CraftItems[i].Id);
            }
            return list;
        }

        public static (int Id, int MaxBackPack, string Name, uint Job, string JobName, uint Lv, bool QuickCraft, (int Id, string Name, int Quantity, bool Craft)[]) GetMidCraftItems(int Id)
        {
            for (int i = 1; i < CraftItems.Length; i++)
            {
                if (CraftItems[i].Id == Id)
                {
                    return CraftItems[i];
                }
            }
            return CraftItems[0];
        }

    }
}
