
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace WoAutoCollectionPlugin.Managers;


internal class BagManager
{
    public BagManager()
    {
        
    }

    public unsafe void test() {
        var bag0 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory1);
        var bag1 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory2);
        var bag2 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory3);
        var bag3 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory4);

        var crystals = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Crystals);
        var currency = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Currency);

        if (bag0 != null && bag1 != null && bag2 != null && bag3 != null && crystals != null && currency != null) {

            PluginLog.Log($"size: {bag0->Size} {bag1->Size} {bag2->Size} {bag3->Size} {crystals->Size} {currency->Size}");
            int bagCount = 0;
            for (int i = 0; i < 35; i++) {
                if (bag0->Items[i].ItemID == 0) {
                    bagCount++;
                }
                if (bag1->Items[i].ItemID == 0)
                {
                    bagCount++;
                }
                if (bag2->Items[i].ItemID == 0)
                {
                    bagCount++;
                }
                if (bag3->Items[i].ItemID == 0)
                {
                    bagCount++;
                }

                //var itemId = bag0->Items[i].ItemID;
                //var Quantity = bag0->Items[i].Quantity;
                //var InventoryType = bag0->Items[i].Container;
                //PluginLog.Log($"{i} {itemId} {Quantity} {InventoryType}");
            }
            PluginLog.Log($"{bagCount}");
        }

        uint ItemId = 30281;
        int Inventory1 = InventoryManager.Instance()->GetItemCountInContainer(ItemId, InventoryType.Inventory1);
        int Inventory2 = InventoryManager.Instance()->GetItemCountInContainer(ItemId, InventoryType.Inventory2);
        int Inventory3 = InventoryManager.Instance()->GetItemCountInContainer(ItemId, InventoryType.Inventory3);
        int Inventory4 = InventoryManager.Instance()->GetItemCountInContainer(ItemId, InventoryType.Inventory4);

        int count = InventoryManager.Instance()->GetInventoryItemCount(ItemId);
        PluginLog.Log($"{Inventory1} {Inventory2} {Inventory3} {Inventory4} {count}");

    }
}
