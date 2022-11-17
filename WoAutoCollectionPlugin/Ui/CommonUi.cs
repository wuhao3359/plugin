using System;
using System.Collections.Generic;
using ClickLib;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using WoAutoCollectionPlugin.Managers;

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

                if (item->Condition <= 8000)
                {
                    PluginLog.Log($"{item->Condition}");
                    return true;
                }
            }
            return false;
        }

        public static unsafe int CanExtractMateria()
        {
            int count = 0;

            bool b = WoAutoCollectionPlugin.GameData.param.TryGetValue("extractMateria", out var v);
            if (!b || v == null || v == "0")
            {
                PluginLog.Log($"精制配置: b: {b}, v: {v},");
                return count;
            }

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

    }
}
