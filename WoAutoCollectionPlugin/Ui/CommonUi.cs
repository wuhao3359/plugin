using System;
using ClickLib;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;

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
                var AtkUnitBase = (AtkUnitBase*)ptr;
                var Addon = (AddonGathering*)ptr;
                var ComponentCheckBox = Addon->GatheredItemComponentCheckBox1;
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

        public static unsafe bool SelectYesButton()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("SelectYesno", 1);
            if (ptr != IntPtr.Zero)
            {
                return Click.TrySendClick("select_yes");
            }
            return false;
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

    }
}
