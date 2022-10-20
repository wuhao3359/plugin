
namespace WoAutoCollectionPlugin.Data
{
    public class Items
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
    }
}
