using System;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace WoAutoCollectionPlugin.Ui
{
    public static class CommonUi
    {
        public static bool AddonSelectStringIsOpen()
        {
            var addon = DalamudApi.GameGui.GetAddonByName("SelectString", 1);
            return addon != IntPtr.Zero;
        }

        public static bool AddonSelectYesnoIsOpen()
        {
            var addon = DalamudApi.GameGui.GetAddonByName("SelectYesno", 1);
            return addon != IntPtr.Zero;
        }

        public static bool AddonContentsFinderConfirmOpen()
        {
            var addon = DalamudApi.GameGui.GetAddonByName("ContentsFinderConfirm", 1);
            return addon != IntPtr.Zero;
        }

        public static unsafe bool SelectYesButton()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("SelectYesno", 1);
            if (ptr != IntPtr.Zero)
            {
                var AtkUnitBase = (AtkUnitBase*)ptr;
                var Addon = (AddonSelectYesno*)ptr;
                var AtkComponentButton = Addon->YesButton;
                var button = AtkComponentButton->ButtonBGNode;
                var nb = button->NextSiblingNode;
                var isVisible = (AtkUnitBase->Flags & 0x20) == 0x20;
                if (isVisible)
                {
                    AtkUnitBase->SetFocusNode(nb, true);
                    return true;
                }
            }
            return false;
        }

        public static unsafe bool ContentsFinderConfirmButton()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("ContentsFinderConfirm", 1);
            if (ptr != IntPtr.Zero)
            {
                var AtkUnitBase = (AtkUnitBase*)ptr;
                var Addon = (AddonContentsFinderConfirm*)ptr;
                var AtkComponentButton = Addon->CommenceButton;
                var button = AtkComponentButton->ButtonBGNode;
                var nb = button->NextSiblingNode;
                var isVisible = (AtkUnitBase->Flags & 0x20) == 0x20;
                if (isVisible)
                {
                    AtkUnitBase->SetFocusNode(nb, true);
                    return true;
                }
            }
            return false;
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
                var isVisible = (AtkUnitBase->Flags & 0x20) == 0x20;
                if (isVisible) {
                    AtkUnitBase->SetFocusNode(nb, true);
                    return true;
                }
            }
            return false;
        }

    
    }
}
