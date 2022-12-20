using System.Collections.Generic;
using System.Numerics;

namespace WoAutoCollectionPlugin.Utility
{
    public static class NormalItems
    {

        public static (int Id, string Name)[] Items =
        {
            (8, "火之水晶"),
            (9, "冰之水晶"),
            (10, "风之水晶"),
            (11, "土之水晶"),
            (12, "雷之水晶"),
            (13, "水之水晶"),
        };


        public static int GetNormalItemId(string name)
        {
            foreach ((int Id, string Name) in Items)
            {
                if (Name == name)
                {
                    return Id;
                }
            }
            return 0;
        }
    }
}
