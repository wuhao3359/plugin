
using AlphaProject.Enums;

namespace AlphaProject.Data
{
    public class RecipeItems
    {
        public static readonly (string Name, uint Category, uint Sub, uint ItemId)[] UploadItems =
        {
            ("收藏用无花果冻糕", 0, 1, 36626),   // ItemId 36626
            ("收藏用巨人新薯煎饼", 0, 2, 35665),
            ("灵岩之剑", 0, 13, 36473),
            ("佛恩·米克", 0, 7, 36494),
            ("红弓鳍鱼", 0, 0, 36408),
            ("噬卵者", 0, 0, 36497),
        };

        public static readonly (int Id, string ItemName, int Category, int Sub)[] ExchangeItems =
        {
            (100007, "魔匠7", 100007, 9),
            (100092, "炼金药", 100092, 2),
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

        public static (int Category, int Sub) ExchangeApply(string itemName)
        {
            foreach (var (Id, ItemName,  Category, Sub) in ExchangeItems)
            {
                if (ItemName == itemName)
                {
                    return (Category, Sub);
                }
            }
            return (0, 0);
        }

        // 8-刻木匠 9-锻铁匠 10-铸甲匠 11-雕金匠 12-制革匠 13-裁衣匠
        // 14-炼金术士 15-烹调师
        // 16-采矿工 17-园艺工
        public static (uint Id, string Name, byte GetWay, uint Job)[] Items = {
            (27841, "山地小麦粉", (byte)GetWay.CRAFT, 15),
            (27832, "山地小麦", (byte)GetWay.GATHER, 17),
            (27842, "黄金蜂蜜", (byte)GetWay.CRAFT, 15),
            (27834, "大蜜蜂的巢", (byte)GetWay.GATHER, 17),
            (36095, "古代肉豆蔻", (byte)GetWay.GATHER, 15),
            (36079, "紫苏油", (byte)GetWay.CRAFT, 15),
            (36089, "巨人新薯", (byte)GetWay.GATHER, 17),
            (36080, "棕榈糖", (byte)GetWay.CRAFT, 15),
            (36080, "棕榈糖", (byte)GetWay.CRAFT, 15),
            (36086, "棕榈糖浆", (byte)GetWay.GATHER, 17),
            (36096, "无花果", (byte)GetWay.GATHER, 17),
        };

        public static (uint Id, string JobName)[] Jobs = {
            (14, "炼金术士"),
            (15, "烹调师"),
            (16, "采矿工"),
            (17, "园艺工"),
        };

        public static string GetJobName(uint Id)
        {
            foreach ((uint id, string jobName) in Jobs)
            {
                if (id == Id)
                {
                    return jobName;
                }
            }
            return null;
        }

        public static (uint Id, string Name, byte GetWay, uint job) GetItem(uint Id)
        {
            foreach ((uint id, string name, byte getWay, uint job) in Items)
            {
                if (id == Id)
                {
                    return (id, name, getWay, job);
                }
            }
            return (0, null, 0, 0);
        }

        public static (uint Id, string Name, byte GetWay, uint Job) GetItemByName(string Name)
        {
            foreach ((uint id, string name, byte getWay, uint job) in Items)
            {
                if (name == Name)
                {
                    return (id, name, getWay, job);
                }
            }
            return (0, null, 0, 0);
        }
    }
}
