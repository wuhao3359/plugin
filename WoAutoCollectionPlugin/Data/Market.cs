using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoAutoCollectionPlugin.Data
{
    internal class Market
    {
        public static (uint Id, string ItemName, int RetainerIndex, int LowestPrice, int MaxSlot)[] Items =
        {
            (36223, "晓月灵砂", 1, 300, 20),
        };

        public static List<(uint, InventoryType)> InventoryTypeList = new(){
            (0, InventoryType.Inventory1),
            (1, InventoryType.Inventory2),
            (2, InventoryType.Inventory3),
            (3, InventoryType.Inventory4),
            (2001, InventoryType.Crystals),
        };

        public static List<(uint, string, int, int, int)> GetItemsByRetainer(int index)
        {
            List<(uint, string, int, int, int)> list = new();
            foreach ((uint Id, string ItemName, int RetainerIndex, int LowestPrice, int MaxSlot) in Items)
            {
                if (RetainerIndex == index)
                {
                    list.Add((Id, ItemName, RetainerIndex, LowestPrice, MaxSlot));
                }
            }
            return list;
        }
    }
}
