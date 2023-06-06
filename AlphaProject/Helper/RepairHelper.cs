using ClickLib.Clicks;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using System;

namespace AlphaProject.Helper
{
    public unsafe static class RepairHelper
    {
        internal static void OpenRepairAddon()
        {
            if (DalamudApi.GameGui.GetAddonByName("Repair", 1) == IntPtr.Zero)
            {
                if (Throttler.Throttle(1200))
                {
                    ActionManager.Instance()->UseAction(ActionType.General, 6);
                }
            }
        }

        internal static void CloseRepairAddon()
        {
            if (DalamudApi.GameGui.GetAddonByName("Repair", 1) != IntPtr.Zero)
            {
                if (Throttler.Throttle(1200))
                {
                    ActionManager.Instance()->UseAction(ActionType.General, 6);
                }
            }
        }

        internal static void Repair()
        {
            if (GenericHelper.TryGetAddonByName<AddonRepair>("Repair", out var addon) && addon->AtkUnitBase.IsVisible && addon->RepairAllButton->IsEnabled && Throttler.Throttle(500))
            {
                new ClickRepair((IntPtr)addon).RepairAll();
            }
        }

        internal static void ConfirmYesNo()
        {
            if (GenericHelper.TryGetAddonByName<AddonRepair>("Repair", out var r) &&
                r->AtkUnitBase.IsVisible && GenericHelper.TryGetAddonByName<AddonSelectYesno>("SelectYesno", out var addon) &&
                addon->AtkUnitBase.IsVisible &&
                addon->YesButton->IsEnabled &&
                addon->AtkUnitBase.UldManager.NodeList[15]->IsVisible &&
                Throttler.Throttle(500))
            {
                new ClickSelectYesNo((IntPtr)addon).Yes();
            }
        }

        internal static int GetMinEquippedPercent()
        {
            var ret = int.MaxValue;
            var equipment = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems);
            for (var i = 0; i < equipment->Size; i++)
            {
                var item = equipment->GetInventorySlot(i);
                if (item->Condition < ret) ret = item->Condition;
            }
            return (ret / 300);
        }

        internal static bool ProcessRepair(bool use = true)
        {
            int repairPercent = 20;
            if (GetMinEquippedPercent() >= repairPercent)
            {
                if (AlphaProject.Debug) PluginLog.Verbose("Condition good");
                if (GenericHelper.TryGetAddonByName<AddonRepair>("Repair", out var r) && r->AtkUnitBase.IsVisible)
                {
                    if (AlphaProject.Debug) PluginLog.Verbose("Repair visible");
                    if (Throttler.Throttle(500))
                    {
                        if (AlphaProject.Debug) PluginLog.Verbose("Closing repair window");
                        ActionManager.Instance()->UseAction(ActionType.General, 6);
                    }
                    return false;
                }
                if (AlphaProject.Debug) PluginLog.Verbose("return true");
                return true;
            }
            else
            {
                if (AlphaProject.Debug) PluginLog.Verbose($"Condition bad, condition is {GetMinEquippedPercent()}, config is {20}");
                if (use)
                {
                    if (AlphaProject.Debug) PluginLog.Verbose($"Doing repair");
                    if (GenericHelper.TryGetAddonByName<AddonRepair>("Repair", out var r) && r->AtkUnitBase.IsVisible)
                    {
                        //PluginLog.Verbose($"Repair visible");
                        ConfirmYesNo();
                        Repair();
                    }
                    else
                    {
                        if (AlphaProject.Debug) PluginLog.Verbose($"Repair not visible");
                        if (Throttler.Throttle(500))
                        {
                            if (AlphaProject.Debug) PluginLog.Verbose($"Opening repair");
                            ActionManager.Instance()->UseAction(ActionType.General, 6);
                        }
                    }
                }
                if (AlphaProject.Debug) PluginLog.Verbose($"Returning false");
                return false;
            }
        }
    }
}
