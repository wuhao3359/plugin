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
            //var stage = this.getAtkStageSingleton();
            //var unitManagers = &stage->RaptureAtkUnitManager->AtkUnitManager.DepthLayerOneList;

            //var unitManager = &unitManagers[14];
            //var unitBaseArray = &unitManager->AtkUnitEntries;
            //PluginLog.Log($"{listNames[14]}-{unitManager->Count}");
            //PluginLog.Log($"node log");
            //for (var j = 0; j < unitManager->Count; j++)
            //{
            //    var unitBase = unitBaseArray[j];

            //    //AtkResNode* node = unitBase->GetNodeById(28);
            //    //AtkResNode* Target = unitBase->CursorTarget;
            //    //PluginLog.Log($"Target {(int)Target->Type}-{(int)Target->NodeID}-{(int)Target->X}-{(int)Target->Y}");
            //    //AtkResNode* ParentNode = node->ParentNode;
            //    //PluginLog.Log($"{(int)node->Type}-{(int)node->NodeID}-{(int)node->X}-{(int)node->Y}");
            //    //PluginLog.Log($"{(int)ParentNode->Type}-{(int)ParentNode->NodeID}-{(int)ParentNode->X}-{(int)ParentNode->Y}");
            //    var addonName = Marshal.PtrToStringAnsi(new IntPtr(unitBase->Name));
            //    PluginLog.Log($"{addonName}");
            //    AtkResNode* node = unitBase->RootNode;
            //    //AtkResNode* node3 = node->ChildNode;

            //    //PluginLog.Log($"{(int)node->Type}-{(int)node->NodeID}-{(int)node->X}-{(int)node->Y}");
            //    //PluginLog.Log($"{(int)node3->Type}-{(int)node3->NodeID}-{(int)node3->X}-{(int)node3->Y}");

            //    //AtkComponentNode* win = unitBase->WindowNode;
            //    //AtkResNode winNode = win->AtkResNode;
            //    //PluginLog.Log($"{(int)winNode.Type}-{(int)winNode.NodeID}-{(int)winNode.X}-{(int)winNode.Y}");

            //    //PluginLog.Log($"--------------------------------------");
            //    //AtkComponentBase* component = win->Component;
            //    //AtkUldManager uldManager = component->UldManager;
            //    //AtkUldManager uldManager = unitBase->UldManager;
            //    //PluginLog.Log($"{uldManager.NodeListCount}");
            //    //PluginLog.Log($"--------------------------------------");
            //    //AtkResNode** NodeList = uldManager.NodeList;
            //    //for (int k = 0; k < uldManager.NodeListCount; k++)
            //    //{
            //    //    AtkResNode* resNode = NodeList[k];
            //    //    PluginLog.Log($"{(int)resNode->Type}-{(int)resNode->NodeID}-{(int)resNode->X}-{(int)resNode->Y}");
            //    //}

            //    AtkResNode* Target = unitBase->CursorTarget;
            //    PluginLog.Log($"Target1 {(int)Target->Type}-{(int)Target->NodeID}-{(int)Target->X}-{(int)Target->Y}");
            //    PluginLog.Log($"-----------------PrintNode---------------------");
            //    PluginLog.Log($"Node List##{(ulong)unitBase:X} {unitBase->UldManager.NodeListCount}");
            //    for (var k = 0; k < unitBase->UldManager.NodeListCount; k++)
            //    {
            //        AtkResNode* atkNode = unitBase->UldManager.NodeList[k];
            //        if ((int)atkNode->Type < 1000)
            //        {
            //            //PluginLog.Log($"Type < 1000 {atkNode->Type} Node (ptr = {(long)atkNode:X})###{(long)atkNode}");
            //        }
            //        else
            //        {
            //            var compNode = (AtkComponentNode*)atkNode;
            //            var componentInfo = compNode->Component->UldManager;
            //            var childCount = componentInfo.NodeListCount;
            //            var objectInfo = (AtkUldComponentInfo*)componentInfo.Objects;

            //            PluginLog.Log($"Type > 1000 {objectInfo->ComponentType} Component Node (ptr = {(long)atkNode:X}, component ptr = {(long)compNode->Component:X}) child count = {childCount}  ###{(long)atkNode}");
            //            if (objectInfo->ComponentType == ComponentType.Button) {
            //                Target = unitBase->CursorTarget;
            //                PluginLog.Log($"Target2 {(int)Target->Type}-{(int)Target->NodeID}-{(int)Target->X}-{(int)Target->Y}");
            //            }
            //        }
            //    }
            //}

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
            return addon->UldManager.LoadedState == 3;
        }
    }
}
