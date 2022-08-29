using System;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace WoAutoCollectionPlugin.Ui
{
    public static class RepairUi
    {
        public static bool AddonRepairIsOpen()
        {
            var addon = DalamudApi.GameGui.GetAddonByName("Repair", 1);
            return addon != IntPtr.Zero;
        }

        public static unsafe bool AllRepairButton()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("Repair", 1);
            if (ptr != IntPtr.Zero)
            {
                var AtkUnitBase = (AtkUnitBase*)ptr;
                var Addon = (AddonRepair*)ptr;
                var AtkComponentButton = Addon->RepairAllButton;
                var button = AtkComponentButton->ButtonBGNode;
                var nb = button->NextSiblingNode;
                AtkUnitBase->SetFocusNode(nb, true);
                return true;
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

                if (item->Condition <= 8000) {
                    PluginLog.Log($"{item->Condition}");
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
            return count;
        }
    }
}
