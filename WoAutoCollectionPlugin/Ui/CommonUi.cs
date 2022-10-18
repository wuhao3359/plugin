using System;
using System.Collections.Generic;
using ClickLib;
using Dalamud.Logging;
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
                var AtkTextNode1 = (Addon->GatheredItemComponentCheckBox1)->AtkComponentButton.ButtonTextNode;
                var AtkTextNode2 = (Addon->GatheredItemComponentCheckBox2)->AtkComponentButton.ButtonTextNode;
                var AtkTextNode3 = (Addon->GatheredItemComponentCheckBox3)->AtkComponentButton.ButtonTextNode;
                var AtkTextNode4 = (Addon->GatheredItemComponentCheckBox4)->AtkComponentButton.ButtonTextNode;
                var AtkTextNode5 = (Addon->GatheredItemComponentCheckBox5)->AtkComponentButton.ButtonTextNode;
                var AtkTextNode6 = (Addon->GatheredItemComponentCheckBox6)->AtkComponentButton.ButtonTextNode;
                var AtkTextNode7 = (Addon->GatheredItemComponentCheckBox7)->AtkComponentButton.ButtonTextNode;
                var AtkTextNode8 = (Addon->GatheredItemComponentCheckBox8)->AtkComponentButton.ButtonTextNode;

                string n1 = GetNodeText(AtkTextNode1);
                string n2 = GetNodeText(AtkTextNode2);
                string n3 = GetNodeText(AtkTextNode3);
                string n4 = GetNodeText(AtkTextNode4);
                string n5 = GetNodeText(AtkTextNode5);
                string n6 = GetNodeText(AtkTextNode6);
                string n7 = GetNodeText(AtkTextNode7);
                string n8 = GetNodeText(AtkTextNode8);
                PluginLog.Log($"{n1} {n2} {n3} {n4} {n5} {n6} {n7} {n8}");
                foreach (string name in ItemNames)
                {
                    na = name;
                    if (name == n1) 
                    {
                        index = 1;
                        break;
                    } 
                    else if (name == n2)
                    {
                        index = 2;
                        break;
                    }
                    else if (name == n3)
                    {
                        index = 3;
                        break;
                    }
                    else if (name == n4)
                    {
                        index = 4;
                        break;
                    }
                    else if (name == n5)
                    {
                        index = 5;
                        break;
                    }
                    else if (name == n6)
                    {
                        index = 6;
                        break;
                    }
                    else if (name == n7)
                    {
                        index = 7;
                        break;
                    }
                    else if (name == n8)
                    {
                        index = 8;
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

    }
}
