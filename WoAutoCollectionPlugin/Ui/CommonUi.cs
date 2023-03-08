using System;
using System.Collections.Generic;
using ClickLib;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using Dalamud.Plugin.Ipc.Exceptions;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using WoAutoCollectionPlugin.Managers;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Ui
{
    public static class CommonUi
    {
        public unsafe static bool AddonSelectStringIsOpen()
        {
            var (addon, success) = IsAddonVisible("SelectString");
            return success;
        }

        public unsafe static bool AddonSelectIconStringIsOpen()
        {
            var (addon, success) = IsAddonVisible("SelectIconString");
            return success;
        }

        public unsafe static bool AddonSelectYesnoIsOpen()
        {
            var (addon, success) = IsAddonVisible("SelectYesno");
            return success;
        }

        public unsafe static bool AddonContentsFinderConfirmIsOpen()
        {
            var (addon, success) = IsAddonVisible("ContentsFinderConfirm");
            return success;
        }

        public unsafe static bool AddonGatheringIsOpen()
        {
            var (addon, success) = IsAddonVisible("Gathering");
            return success;
        }

        public unsafe static bool AddonGatheringMasterpieceIsOpen()
        {
            var (addon, success) = IsAddonVisible("GatheringMasterpiece");
            return success;
        }

        public unsafe static bool AddonCollectablesShopIsOpen()
        {
            var (addon, success) = IsAddonVisible("CollectablesShop");
            return success;
        }

        public unsafe static bool AddonInclusionShopIsOpen()
        {
            var (addon, success) = IsAddonVisible("InclusionShop");
            return success;
        }

        public unsafe static bool AddonRetainerSellListIsOpen()
        {
            var (addon, success) = IsAddonVisible("AddonRetainerSellList");
            return success;
        }

        public unsafe static bool GetAddonRetainerSell()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("AddonRetainerSell", 1);
            return false;
        }

        public unsafe static bool SelectString1Button()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("SelectString", 1);
            if (ptr != IntPtr.Zero)
            {
                return Click.TrySendClick("select_string1");
            }
            return false;
        }

        public unsafe static bool SelectIconString2Button()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("SelectIconString", 1);
            if (ptr != IntPtr.Zero)
            {
                return Click.TrySendClick("select_icon_string2");
            }
            return false;
        }

        public static unsafe bool GatheringButton(int index)
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("Gathering", 1);
            if (ptr != IntPtr.Zero)
            {
                if (index == 1)
                {
                    return Click.TrySendClick("gathering_checkbox1");
                }
                else if (index == 2)
                {
                    return Click.TrySendClick("gathering_checkbox2");
                }
                else if (index == 3)
                {
                    return Click.TrySendClick("gathering_checkbox3");
                }
                else if (index == 4)
                {
                    return Click.TrySendClick("gathering_checkbox4");
                }
                else if (index == 5)
                {
                    return Click.TrySendClick("gathering_checkbox5");
                }
                else if (index == 6)
                {
                    return Click.TrySendClick("gathering_checkbox6");
                }
                else if (index == 7)
                {
                    return Click.TrySendClick("gathering_checkbox7");
                }
                else if (index == 8)
                {
                    return Click.TrySendClick("gathering_checkbox8");
                }
                else {
                    return false;
                }
            }
            return false;
        }

        public static unsafe (int, string) GetGatheringIndex(List<string> ItemNames) {
            int index = 0;
            string na = "";
            var ptr = DalamudApi.GameGui.GetAddonByName("Gathering", 1);
            if (ptr != IntPtr.Zero)
            {
                var AtkUnitBase = (AtkUnitBase*)ptr;
                var Addon = (AddonGathering*)ptr;
                uint itemId1 = Addon->GatheredItemId1;
                uint itemId2 = Addon->GatheredItemId2;
                uint itemId3 = Addon->GatheredItemId3;
                uint itemId4 = Addon->GatheredItemId4;
                uint itemId5 = Addon->GatheredItemId5;
                uint itemId6 = Addon->GatheredItemId6;
                uint itemId7 = Addon->GatheredItemId7;
                uint itemId8 = Addon->GatheredItemId8;

                string n1 = "";
                string n2 = "";
                string n3 = "";
                string n4 = "";
                string n5 = "";
                string n6 = "";
                string n7 = "";
                string n8 = "";
                if (itemId1 != 0) {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId1, out var item1);
                    n1 = item1 != null ? item1.Name.ToString() : "";
                }
                if (itemId2 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId2, out var item2);
                    n2 = item2 != null ? item2.Name.ToString() : "";
                }
                if (itemId3 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId3, out var item3);
                    n3 = item3 != null ? item3.Name.ToString() : "";
                }
                if (itemId4 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId4, out var item4);
                    n4 = item4 != null ? item4.Name.ToString() : "";
                }
                if (itemId5 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId5, out var item5);
                    n5 = item5 != null ? item5.Name.ToString() : "";
                }
                if (itemId6 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId6, out var item6);
                    n6 = item6 != null ? item6.Name.ToString() : "";
                }
                if (itemId7 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId7, out var item7);
                    n7 = item7 != null ? item7.Name.ToString() : "";
                }
                if (itemId8 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId8, out var item8);
                    n8 = item8 != null ? item8.Name.ToString() : "";
                }

                
                foreach (string name in ItemNames)
                {
                    if (name == n1 || n1.Contains("地图")) 
                    {
                        index = 1;
                        na = n1;
                        break;
                    }
                    else if (name == n2 || n2.Contains("地图"))
                    {
                        index = 2;
                        na = n2;
                        break;
                    }
                    else if (name == n3 || n3.Contains("地图"))
                    {
                        index = 3;
                        na = n3;
                        break;
                    }
                    else if (name == n4 || n4.Contains("地图"))
                    {
                        index = 4;
                        na = n4;
                        break;
                    }
                    else if (name == n5 || n5.Contains("地图"))
                    {
                        index = 5;
                        na = n5;
                        break;
                    }
                    else if (name == n6 || n6.Contains("地图"))
                    {
                        index = 6;
                        na = n6;
                        break;
                    }
                    else if (name == n7 || n7.Contains("地图"))
                    {
                        index = 7;
                        na = n7;
                        break;
                    }
                    else if (name == n8 || n8.Contains("地图"))
                    {
                        index = 8;
                        na = n8;
                        break;
                    }
                }
            }
            return (index, na);
        }

        public static unsafe (int, string) GetNormalGatheringIndex(List<string> ItemNames, bool CoolDown)
        {
            int index = 0;
            string na = "";
            var ptr = DalamudApi.GameGui.GetAddonByName("Gathering", 1);
            if (ptr != IntPtr.Zero)
            {
                var AtkUnitBase = (AtkUnitBase*)ptr;
                var Addon = (AddonGathering*)ptr;
                uint itemId1 = Addon->GatheredItemId1;
                uint itemId2 = Addon->GatheredItemId2;
                uint itemId3 = Addon->GatheredItemId3;
                uint itemId4 = Addon->GatheredItemId4;
                uint itemId5 = Addon->GatheredItemId5;
                uint itemId6 = Addon->GatheredItemId6;
                uint itemId7 = Addon->GatheredItemId7;
                uint itemId8 = Addon->GatheredItemId8;

                string n1 = "";
                string n2 = "";
                string n3 = "";
                string n4 = "";
                string n5 = "";
                string n6 = "";
                string n7 = "";
                string n8 = "";
                if (itemId1 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId1, out var item1);
                    n1 = item1 != null ? item1.Name.ToString() : "";
                }
                if (itemId2 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId2, out var item2);
                    n2 = item2 != null ? item2.Name.ToString() : "";
                }
                if (itemId3 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId3, out var item3);
                    n3 = item3 != null ? item3.Name.ToString() : "";
                }
                if (itemId4 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId4, out var item4);
                    n4 = item4 != null ? item4.Name.ToString() : "";
                }
                if (itemId5 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId5, out var item5);
                    n5 = item5 != null ? item5.Name.ToString() : "";
                }
                if (itemId6 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId6, out var item6);
                    n6 = item6 != null ? item6.Name.ToString() : "";
                }
                if (itemId7 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId7, out var item7);
                    n7 = item7 != null ? item7.Name.ToString() : "";
                }
                if (itemId8 != 0)
                {
                    WoAutoCollectionPlugin.GameData.Gatherables.TryGetValue(itemId8, out var item8);
                    n8 = item8 != null ? item8.Name.ToString() : "";
                }

                string priority = "地图";
                if (CoolDown) {
                    priority = "之水晶";
                }
                foreach (string name in ItemNames)
                {
                    if (n1.Contains(priority)) {
                        index = 1;
                        na = n1;
                        break;
                    }
                    if (n2.Contains(priority))
                    {
                        index = 2;
                        na = n2;
                        break;
                    }
                    if (n3.Contains(priority))
                    {
                        index = 3;
                        na = n3;
                        break;
                    }
                    if (n4.Contains(priority))
                    {
                        index = 4;
                        na = n4;
                        break;
                    }
                    if (n5.Contains(priority))
                    {
                        index = 5;
                        na = n5;
                        break;
                    }
                    if (n6.Contains(priority))
                    {
                        index = 6;
                        na = n6;
                        break;
                    }
                    if (n7.Contains(priority))
                    {
                        index = 7;
                        na = n7;
                        break;
                    }
                    if (n8.Contains(priority))
                    {
                        index = 8;
                        na = n8;
                        break;
                    }

                    if (name == n1)
                    {
                        index = 1;
                        na = n1;
                        break;
                    }
                    else if (name == n2)
                    {
                        index = 2;
                        na = n2;
                        break;
                    }
                    else if (name == n3)
                    {
                        index = 3;
                        na = n3;
                        break;
                    }
                    else if (name == n4)
                    {
                        index = 4;
                        na = n4;
                        break;
                    }
                    else if (name == n5)
                    {
                        index = 5;
                        na = n5;
                        break;
                    }
                    else if (name == n6)
                    {
                        index = 6;
                        na = n6;
                        break;
                    }
                    else if (name == n7)
                    {
                        index = 7;
                        na = n7;
                        break;
                    }
                    else if (name == n8)
                    {
                        index = 8;
                        na = n8;
                        break;
                    }
                }
            }
            return (index, na);
        }

        public static unsafe bool SelectYesButton()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("SelectYesno", 1);
            if (ptr != IntPtr.Zero)
            {
                return Click.TrySendClick("select_yes");
            }
            return false;
        }

        public unsafe static bool AddonMaterializeDialogIsOpen()
        {
            var (addon, success) = IsAddonVisible("MaterializeDialog");
            return success;
        }

        public unsafe static bool AddonPurifyItemSelectorIsOpen()
        {
            var (addon, success) = IsAddonVisible("PurifyItemSelector");
            return success;
        }

        public static unsafe bool SelectMaterializeDialogYesButton()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("MaterializeDialog", 1);
            if (ptr != IntPtr.Zero)
            {
                return Click.TrySendClick("materialize");
            }
            return false;
        }

        public static unsafe bool ContentsFinderConfirmButton()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("ContentsFinderConfirm", 1);
            if (ptr != IntPtr.Zero)
            {
                return Click.TrySendClick("duty_commence");
            }
            return false;
        }

        public unsafe static (IntPtr Addon, bool IsVisible) IsAddonVisible(string addonName)
        {
            var addonPtr = DalamudApi.GameGui.GetAddonByName(addonName, 1);
            if (addonPtr == IntPtr.Zero)
                return (addonPtr, false);

            var addon = (AtkUnitBase*)addonPtr;
            if (!addon->IsVisible || addon->UldManager.LoadedState != AtkLoadState.Loaded)
                return (addonPtr, false);

            return (addonPtr, true);
        }

        public unsafe static string GetNodeText(AtkTextNode* node)
        {
            try
            {
                if (node == null)
                    throw new NullReferenceException("TextNode is null");

                var text = node->NodeText.ToString();
                return text;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"{ex}");
                return "" ;
            }
        }

        public static bool CurrentJob(uint job) {
            var jobId = DalamudApi.ClientState.LocalPlayer?.ClassJob.Id;
            if (job == jobId) {
                return true;
            }
            return false;
        }

        public static bool AddonRepairIsOpen()
        {
            var (addon, success) = CommonUi.IsAddonVisible("Repair");
            return success;
        }

        public static unsafe bool AllRepairButton()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("Repair", 1);
            if (ptr != IntPtr.Zero)
            {
                return Click.TrySendClick("repair_all");
            }
            return false;
        }

        public static unsafe bool ItemSearchIsOpen()
        {
            var (addon, success) = CommonUi.IsAddonVisible("ItemSearch");
            return success;
        }

        public static unsafe bool RetainerSellConfirmButton()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("RetainerSell", 1);
            if (ptr != IntPtr.Zero)
            {
                return Click.TrySendClick("confirm");
            }
            return false;
        }

        public static unsafe bool test()
        {
            PluginLog.Log("----------into------------");
            var im = InventoryManager.Instance();
            var marketContainer = im->GetInventoryContainer(InventoryType.RetainerMarket);

            PluginLog.Log($"------------------------------------, {marketContainer->Size}");
            uint slot2 = 0;
            for (var i = 0; i < marketContainer->Size; i++)
            {
                var item = marketContainer->GetInventorySlot(i);
                if (item == null)
                    continue;

                PluginLog.Log($"ItemId:{item->ItemID}, Slot: {item->Slot}");
                if (item->ItemID == 0)
                {
                    slot2 = (uint)item->Slot;
                }
            }

            var inventory4Container = im->GetInventoryContainer(InventoryType.Inventory4);
            PluginLog.Log($"------------------------------------, {inventory4Container->Size}");
            uint slot1 = 0;
            for (var j = 0; j < inventory4Container->Size; j++)
            {
                var item = inventory4Container->GetInventorySlot(j);
                if (item == null)
                    continue;

                PluginLog.Log($"ItemId:{item->ItemID}, Slot: {item->Slot}");
                if (item->ItemID != 0) {
                    slot1 = (uint)item->Slot;
                }
            }
            PluginLog.Log($"{slot1}--->{slot2}");

           // int res = im->MoveItemSlot(InventoryType.Inventory4, slot1, InventoryType.RetainerMarket, slot2, 1);

            PluginLog.Log("------------------------------------");
            return false;
        }

        public static unsafe bool test1()
        {
            PluginLog.Log("----------into------------");
            var im = InventoryManager.Instance();
            var marketContainer = im->GetInventoryContainer(InventoryType.RetainerMarket);

            PluginLog.Log($"------------------------------------, {marketContainer->Size}");
            for (var i = 0; i < marketContainer->Size; i++)
            {
                var item = marketContainer->GetInventorySlot(i);
                if (item == null)
                    continue;

                PluginLog.Log($"ItemId:{item->ItemID}, Slot: {item->Slot}");
            }



            var inventory1Container = im->GetInventoryContainer(InventoryType.Inventory1);
            PluginLog.Log($"------------------------------------, {inventory1Container->Size}");
            for (var j = 0; j < inventory1Container->Size; j++)
            {
                var item = inventory1Container->GetInventorySlot(j);
                if (item == null)
                    continue;

                PluginLog.Log($"ItemId:{item->ItemID}, Slot: {item->Slot}");
            }
            PluginLog.Log("------------------------------------");
            return false;
        }

        public static unsafe bool test2() {
            var ic = AgentInventoryContext.Instance();
            if (ic == null) {
                PluginLog.Error("AgentInventoryContext was null");
                return false;
            }
            uint addId = ic->AgentInterface.GetAddonID();
            PluginLog.Log($"AgentInventoryContext addId: {addId}");
            ic->OpenForItemSlot(InventoryType.RetainerMarket, 1, addId);

            return false;
        }

        public static unsafe bool CanRepair()
        {
            var im = InventoryManager.Instance();
            if (im == null)
            {
                PluginLog.Error("InventoryManager was null");
                return false;
            }

            var equipped = im->GetInventoryContainer(InventoryType.EquippedItems);
            if (equipped == null)
            {
                PluginLog.Error("InventoryContainer was null");
                return false;
            }

            if (equipped->Loaded == 0)
            {
                PluginLog.Error($"InventoryContainer is not loaded");
                return false;
            }

            for (var i = 0; i < equipped->Size; i++)
            {
                var item = equipped->GetInventorySlot(i);
                if (item == null)
                    continue;

                if (item->Condition <= 29000)
                {
                    PluginLog.Log($"{item->Condition}");
                    return true;
                }
            }
            return false;
        }

        public static unsafe bool NeedsRepair()
        {
            var im = InventoryManager.Instance();
            if (im == null)
            {
                PluginLog.Error("InventoryManager was null");
                return false;
            }

            var equipped = im->GetInventoryContainer(InventoryType.EquippedItems);
            if (equipped == null)
            {
                PluginLog.Error("InventoryContainer was null");
                return false;
            }

            if (equipped->Loaded == 0)
            {
                PluginLog.Error($"InventoryContainer is not loaded");
                return false;
            }

            for (var i = 0; i < equipped->Size; i++)
            {
                var item = equipped->GetInventorySlot(i);
                if (item == null)
                    continue;

                if (item->Condition <= 10000)
                {
                    return true;
                }
            }
            return false;
        }

        public static unsafe int CanExtractMateria()
        {
            int count = 0;
            var im = InventoryManager.Instance();
            if (im == null)
            {
                PluginLog.Error("InventoryManager was null");
                return count;
            }

            var equipped = im->GetInventoryContainer(InventoryType.EquippedItems);
            if (equipped == null)
            {
                PluginLog.Error("InventoryContainer was null");
                return count;
            }

            if (equipped->Loaded == 0)
            {
                PluginLog.Error("InventoryContainer is not loaded");
                return count;
            }

            for (var i = 0; i < equipped->Size; i++)
            {
                var item = equipped->GetInventorySlot(i);
                if (item == null)
                    continue;

                var spiritbond = item->Spiritbond / 100;
                if (spiritbond == 100f)
                {
                    count++;
                }
            }
            PluginLog.Log($"总共有 {count}, 可以精制魔晶石 ");
            return count;
        }

        public static unsafe int CanExtractMateriaCollectable()
        {
            int count = 0;
            foreach ((int itemId, string itemName) in LimitMaterials.CollecMaterialItems) {
                count += BagManager.GetInventoryItemCountById((uint)itemId);
            }
            PluginLog.Log($"总共有 {count}, 可以精选物品");
            return count;
        }

        public static unsafe bool HasStatus(string statusName)
        {
            statusName = statusName.ToLowerInvariant();
            var statusList = DalamudApi.ClientState.LocalPlayer.StatusList.GetEnumerator();
            while (statusList.MoveNext())
            {
                Dalamud.Game.ClientState.Statuses.Status status = statusList.Current;
                uint statusId = status.StatusId;
                if (WoAutoCollectionPlugin.GameData.Status.TryGetValue(statusId, out var state)) {
                    if (state.Name == statusName)
                    {
                        return true;
                    }
                } 
            }
            return false;
        }

    }
}
