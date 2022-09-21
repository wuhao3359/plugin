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
        public static bool AddonSelectStringIsOpen()
        {
            var addon = DalamudApi.GameGui.GetAddonByName("SelectString", 1);
            return addon != IntPtr.Zero;
        }

        public static unsafe bool SelectString1Button()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("SelectString", 1);
            if (ptr != IntPtr.Zero)
            {
<<<<<<< HEAD
                return Click.TrySendClick("select_string15");
=======
                return Click.TrySendClick("select_string1");
>>>>>>> e1834f53afcdae64ecf9a1255ef3ce24a376555c
            }
            return false;
        }

        public static bool AddonSelectYesnoIsOpen()
        {
            var addon = DalamudApi.GameGui.GetAddonByName("SelectYesno", 1);
            return addon != IntPtr.Zero;
        }

        public static bool AddonContentsFinderConfirmIsOpen()
        {
            var addon = DalamudApi.GameGui.GetAddonByName("ContentsFinderConfirm", 1);
            return addon != IntPtr.Zero;
        }

        public static bool AddonGatheringIsOpen()
        {
            var addon = DalamudApi.GameGui.GetAddonByName("Gathering", 1);
            return addon != IntPtr.Zero;
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
                    ComponentCheckBox = Addon->GatheredItemComponentCheckBox1;
                }
                else if (index == 2)
                {
                    ComponentCheckBox = Addon->GatheredItemComponentCheckBox2;
                }
                else if (index == 3)
                {
                    ComponentCheckBox = Addon->GatheredItemComponentCheckBox3;
                }
                else if (index == 4)
                {
                    ComponentCheckBox = Addon->GatheredItemComponentCheckBox4;
                }
                else if (index == 5)
                {
                    ComponentCheckBox = Addon->GatheredItemComponentCheckBox5;
                }
                else if (index == 6)
                {
                    ComponentCheckBox = Addon->GatheredItemComponentCheckBox6;
                }
                else if (index == 7)
                {
                    ComponentCheckBox = Addon->GatheredItemComponentCheckBox7;
                }
                else if (index == 8)
                {
                    ComponentCheckBox = Addon->GatheredItemComponentCheckBox8;
                }
                else {
                    return false;
                }
                var AtkComponentButton = ComponentCheckBox->AtkComponentButton;
                var AtkComponentBase = AtkComponentButton.AtkComponentBase;
                var UldManager = AtkComponentBase.UldManager;

                var isVisible = (AtkUnitBase->Flags & 0x20) == 0x20;
                if (isVisible)
                {
                    AtkResNode* nb = null;
                    for (int i = 0; i < UldManager.NodeListCount; i++)
                    {
                        if (UldManager.NodeList[i]->Type == NodeType.Collision)
                        {
                            nb = UldManager.NodeList[i];
                            break;
                        }

                    }
                    if (nb == null) { 
                        return false;
                    }
                    AtkUnitBase->SetFocusNode(nb, true);
                    return true;
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

        public static unsafe bool ContentsFinderConfirmButton()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("ContentsFinderConfirm", 1);
            if (ptr != IntPtr.Zero)
            {
                return Click.TrySendClick("duty_commence");
            }
            return false;
        }

    }
}
