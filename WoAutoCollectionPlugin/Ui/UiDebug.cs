using ClickLib.Exceptions;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Runtime.InteropServices;

namespace WoAutoCollectionPlugin.Ui
{
    internal unsafe class UiDebug
    {
        private readonly GetAtkStageSingleton getAtkStageSingleton;

        private const int UnitListCount = 18;
        private readonly string[] listNames = new string[UnitListCount]
        {
            "Depth Layer 1",
            "Depth Layer 2",
            "Depth Layer 3",
            "Depth Layer 4",
            "Depth Layer 5",
            "Depth Layer 6",
            "Depth Layer 7",
            "Depth Layer 8",
            "Depth Layer 9",
            "Depth Layer 10",
            "Depth Layer 11",
            "Depth Layer 12",
            "Depth Layer 13",
            "Loaded Units",
            "Focused Units",
            "Units 16",
            "Units 17",
            "Units 18",
        };

        public UiDebug()
        {
            var sigScanner = DalamudApi.SigScanner;
            var getSingletonAddr = sigScanner.ScanText("E8 ?? ?? ?? ?? 41 B8 01 00 00 00 48 8D 15 ?? ?? ?? ?? 48 8B 48 20 E8 ?? ?? ?? ?? 48 8B CF");
            this.getAtkStageSingleton = Marshal.GetDelegateForFunctionPointer<GetAtkStageSingleton>(getSingletonAddr);
        }

        private delegate AtkStage* GetAtkStageSingleton();

        public void Draw()
        {
            CommonUi.SelectYesButton();
        }

        public unsafe bool IsAddonVisible(string addonName)
        {
            var ptr = DalamudApi.GameGui.GetAddonByName(addonName, 1);
            if (ptr == IntPtr.Zero)
                return false;

            var addon = (AtkUnitBase*)ptr;
            return addon->IsVisible;
        }

        /// <inheritdoc/>
        public unsafe bool IsAddonReady(string addonName)
        {
            var ptr = DalamudApi.GameGui.GetAddonByName(addonName, 1);
            if (ptr == IntPtr.Zero)
                return false;

            var addon = (AtkUnitBase*)ptr;
            //return addon->UldManager.LoadedState == 3;
            return false;
        }
    }
}
