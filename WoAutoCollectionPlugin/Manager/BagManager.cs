
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace WoAutoCollectionPlugin.Managers;


internal class BagManager
{
    public BagManager()
    {
        
    }

    public static unsafe int InventoryRemaining() {
        var bag0 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory1);
        var bag1 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory2);
        var bag2 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory3);
        var bag3 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory4);

        int count = 0;
        if (bag0 != null && bag1 != null && bag2 != null && bag3 != null)
        {
            
            for (int i = 0; i < 35; i++)
            {
                if (bag0->Items[i].ItemID == 0)
                {
                    count++;
                }
                if (bag1->Items[i].ItemID == 0)
                {
                    count++;
                }
                if (bag2->Items[i].ItemID == 0)
                {
                    count++;
                }
                if (bag3->Items[i].ItemID == 0)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public static unsafe int GetInventoryItemCount(uint ItemId) {
        var bag0 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory1);
        var bag1 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory2);
        var bag2 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory3);
        var bag3 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory4);

        int count = 0;
        if (bag0 != null && bag1 != null && bag2 != null && bag3 != null)
        {

            for (int i = 0; i < 35; i++)
            {
                if (bag0->Items[i].ItemID == ItemId)
                {
                    count++;
                }
                if (bag1->Items[i].ItemID == ItemId)
                {
                    count++;
                }
                if (bag2->Items[i].ItemID == ItemId)
                {
                    count++;
                }
                if (bag3->Items[i].ItemID == ItemId)
                {
                    count++;
                }
            }
        }

        return count;
        //return InventoryManager.Instance()->GetInventoryItemCount(ItemId);
    }

    public static unsafe (int, int, int, int) GetItemCountInContainer(uint ItemId)
    {
        int Inventory1 = InventoryManager.Instance()->GetItemCountInContainer(ItemId, InventoryType.Inventory1);
        int Inventory2 = InventoryManager.Instance()->GetItemCountInContainer(ItemId, InventoryType.Inventory2);
        int Inventory3 = InventoryManager.Instance()->GetItemCountInContainer(ItemId, InventoryType.Inventory3);
        int Inventory4 = InventoryManager.Instance()->GetItemCountInContainer(ItemId, InventoryType.Inventory4);
        return (Inventory1, Inventory2, Inventory3, Inventory4);
    }

    public static unsafe int GetItemQuantityInContainer(uint ItemId) {
        var bag0 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory1);
        var bag1 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory2);
        var bag2 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory3);
        var bag3 = InventoryManager.Instance()->GetInventoryContainer(InventoryType.Inventory4);

        uint quantity = 0;
        if (bag0 != null && bag1 != null && bag2 != null && bag3 != null)
        {

            for (int i = 0; i < 35; i++)
            {
                if (bag0->Items[i].ItemID == 0)
                {
                    quantity += bag0->Items[i].Quantity;
                }
                if (bag1->Items[i].ItemID == 0)
                {
                    quantity += bag1->Items[i].Quantity;
                }
                if (bag2->Items[i].ItemID == 0)
                {
                    quantity += bag2->Items[i].Quantity;
                }
                if (bag3->Items[i].ItemID == 0)
                {
                    quantity += bag3->Items[i].Quantity;
                }
            }
        }
        return (int)quantity;
    }

    public static bool ItemQuantityEnough((int Id, string Name, int Quantity, bool Craft)[] LowCraft)
    {
        for (int i = 0; i < LowCraft.Length; i++)
        {
            int quantity = BagManager.GetItemQuantityInContainer((uint)LowCraft[i].Id);
            if (quantity < LowCraft[i].Quantity)
            {
                return false;
            }
        }
        return true;
    }

    public static bool QickItemQuantityEnough((int Id, string Name, int Quantity, bool Craft)[] LowCraft)
    {
        for (int i = 0; i < LowCraft.Length; i++)
        {
            int quantity = BagManager.GetItemQuantityInContainer((uint)LowCraft[i].Id);
            if (quantity < LowCraft[i].Quantity * 50)
            {
                return false;
            }
        }
        return true;
    }
}
