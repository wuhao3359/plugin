
namespace WoAutoCollectionPlugin.Data
{
    public class Items
    {
        public static readonly (string Name, uint Category, uint Sub)[] UploadItems =
        {
            ("收藏用无花果冻糕", 0, 1),   // TEST
        };

        public static readonly (int Id, uint Category, uint Sub)[] ExchangeItems =
        {
            (1, 1, 1),   // TEST
        };

        public static (uint Category, uint Sub) UploadApply(string ItemName)
        {
            foreach ((string Name, uint Category, uint Sub) in UploadItems)
            {
                if (ItemName.Contains(Name)) { 
                    return (Category, Sub);
                }
            }
            return (0, 0);
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
    }
}
