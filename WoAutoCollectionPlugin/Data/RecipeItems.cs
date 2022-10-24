
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

        public static (int Id, string Name, uint Job, string JobName, uint Lv, (int Id, string Name, int Quantity, bool Craft)[])[] CraftItems = {
            (0, "", 0, "", 0, Craft0),
            (31652, "收藏用愈疮木砂轮机", 8, "刻木匠", 80, Craft31652),
            (27693, "愈疮木木材", 8, "刻木匠", 80, Craft27693),
            (27714, "矮人银锭", 10, "铸甲匠", 80, Craft27714),
            (27804, "凝灰岩磨刀石", 11, "雕金匠", 80, Craft27804),
        };

        public static (int Id, string Name, uint Job, string JobName, uint Lv, (int Id, string Name, int Quantity, bool Craft)[]) GetCraftItems(int Id)
        {
            for (int i = 1; i < CraftItems.Length; i ++) {
                if (CraftItems[i].Id == Id) {
                    return CraftItems[i];
                }
            }
            return CraftItems[0];
        }

        public static (int Id, string Name, uint Job, string JobName, uint Lv, (int Id, string Name, int Quantity, bool Craft)[]) GetMidCraftItems(int Id)
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
