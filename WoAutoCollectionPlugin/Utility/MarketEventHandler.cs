using Dalamud.Game.Text;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Numerics;
using System;
using FFXIVClientStructs.FFXIV.Client.UI;
using ClickLib.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using WoAutoCollectionPlugin.Ui;
using Dalamud.Game.Network.Structures;
using Dalamud.Game.Network;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System.Net;
using System.Linq;
using System.Runtime.InteropServices;
using Lumina.Data.Parsing;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using FFXIVClientStructs.Attributes;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace WoAutoCollectionPlugin.Utility
{
    public unsafe class MarketEventHandler : IDisposable
    {
        #region Sigs, Hooks & Delegates declaration

        private readonly string AddonItemSearchResult_ReceiveEvent_Signature =
            "4C 8B DC 53 56 48 81 EC ?? ?? ?? ?? 49 89 6B 08";

        private readonly string AddonRetainerSell_OnSetup_Signature =
            "48 89 5C 24 ?? 55 56 57 48 83 EC 50 4C 89 64 24";

        private readonly string AddonItemSearchResult_OnSetup_Signature =
            "40 53 41 56 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 89 AC 24";

        private readonly string AddonRetainerSellList_OnSetup_Signature =
            "40 53 55 56 57 41 56 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B F9 49 8B F0 49 8D 48 10";

        private readonly string AddonRetainerSellList_OnFinalize_Signature =
            "40 53 48 83 EC 20 80 B9 ?? ?? ?? ?? ?? 48 8B D9 74 0E 45 33 C9";

        private readonly string AddonInventoryManager_MoveItemSlot_Signature =
            "E8 ?? ?? ?? ?? 33 DB 89 1E";

        private readonly string AddonInventoryContext_OnSetup_Signature =
            "83 B9 ?? ?? ?? ?? ?? 7E 11";

        private HookWrapper<Addon_ReceiveEvent_Delegate> AddonItemSearchResult_ReceiveEvent_HW;
        private HookWrapper<Addon_OnSetup_Delegate> AddonRetainerSell_OnSetup_HW;
        private HookWrapper<Addon_OnSetup_Delegate> AddonItemSearchResult_OnSetup_HW;
        private HookWrapper<Addon_OnSetup_Delegate> AddonRetainerSellList_OnSetup_HW;
        private HookWrapper<Addon_OnFinalize_Delegate> AddonRetainerSellList_OnFinalize_HW;

        private HookWrapper<Addon_MoveItemSlot_Delegate> AddonInventoryManager_MoveItemSlot_HW;
        private HookWrapper<AddonInventoryContext_OnSetup_Delegate> AddonInventoryContext_OnSetup_HW;

        // __int64 __fastcall Client::UI::AddonXXX_ReceiveEvent(__int64 a1, __int16 a2, int a3, __int64 a4, __int64* a5)
        private delegate IntPtr Addon_ReceiveEvent_Delegate(IntPtr self, ushort eventType,
            uint eventParam, IntPtr eventStruct, IntPtr /* AtkResNode* */ nodeParam);

        // __int64 __fastcall Client::UI::AddonXXX_OnSetup(__int64 a1, unsigned int a2, __int64 a3)
        private delegate IntPtr Addon_OnSetup_Delegate(IntPtr addon, uint a2, IntPtr dataPtr);

        // __int64 __fastcall Client::UI::AddonXXX_Finalize(__int64 a1)
        private delegate void Addon_OnFinalize_Delegate(IntPtr addon);

        private delegate int Addon_MoveItemSlot_Delegate(InventoryType srcContainer, uint srcSlot, InventoryType dstContainer, uint dstSlot, byte unk);

        private delegate void* AddonInventoryContext_OnSetup_Delegate(AgentInventoryContext* agent, InventoryType inventory, ushort slot, int a4, ushort a5, byte a6);

        #endregion

        //internal Configuration conf => Configuration.GetOrLoad();

        private IntPtr AddonRetainerSellList = IntPtr.Zero;

        private static Dictionary<string, int> itemsPrice = new();

        private static string itemName = "";

        private static int itemPrice = 10000000;

        public MarketEventHandler()
        {
            AddonItemSearchResult_ReceiveEvent_HW =
                MarketCommons.Hook<Addon_ReceiveEvent_Delegate>(
                    AddonItemSearchResult_ReceiveEvent_Signature,
                    AddonItemSearchResult_ReceiveEvent_Delegate_Detour);

            AddonRetainerSell_OnSetup_HW = MarketCommons.Hook<Addon_OnSetup_Delegate>(
                AddonRetainerSell_OnSetup_Signature,
                AddonRetainerSell_OnSetup_Delegate_Detour);

            AddonItemSearchResult_OnSetup_HW = MarketCommons.Hook<Addon_OnSetup_Delegate>(
                AddonItemSearchResult_OnSetup_Signature,
                AddonItemSearchResult_OnSetup_Delegate_Detour);

            AddonRetainerSellList_OnSetup_HW = MarketCommons.Hook<Addon_OnSetup_Delegate>(
                AddonRetainerSellList_OnSetup_Signature,
                AddonRetainerSellList_OnSetup_Delegate_Detour);

            AddonRetainerSellList_OnFinalize_HW = MarketCommons.Hook<Addon_OnFinalize_Delegate>(
                AddonRetainerSellList_OnFinalize_Signature,
                AddonRetainerSellList_OnFinalize_Delegate_Detour);

            AddonInventoryManager_MoveItemSlot_HW = MarketCommons.Hook<Addon_MoveItemSlot_Delegate>(
                AddonInventoryManager_MoveItemSlot_Signature,
                AddonInventoryManager_MoveItemSlot_Delegate_Detour);

            AddonInventoryContext_OnSetup_HW = MarketCommons.Hook<AddonInventoryContext_OnSetup_Delegate>(
                AddonInventoryContext_OnSetup_Signature,
                AddonInventoryContext_OnSetup_Delegate_Detour);
        }

        public void OnNetworkEvent(IntPtr dataPtr, ushort opCode, uint sourceActorId, uint targetActorId, NetworkMessageDirection direction)
        {
            if (direction != NetworkMessageDirection.ZoneDown) return;
            if (!WoAutoCollectionPlugin.GameData.DataManager.IsDataReady) return;
            if (opCode == WoAutoCollectionPlugin.GameData.DataManager.ServerOpCodes["MarketBoardItemRequestStart"])
            {
                WoAutoCollectionPlugin.newRequest = true;

                // clear cache on new request so we can verify that we got all the data we need when we inspect the price
                WoAutoCollectionPlugin._cache.Clear();
            }
            if (!Retainer()) return;
            if (opCode != WoAutoCollectionPlugin.GameData.DataManager.ServerOpCodes["MarketBoardOfferings"] || !WoAutoCollectionPlugin.newRequest) return;
            var listing = MarketBoardCurrentOfferings.Read(dataPtr);

            // collect data for data integrity
            WoAutoCollectionPlugin._cache.Add(listing);
            if (!IsDataValid(listing)) return;

            var i = 0;
            if (DalamudApi.KeyState[Keys.ctrl_key])
            {
                if (WoAutoCollectionPlugin.items.Single(j => j.RowId == listing.ItemListings[0].CatalogId).CanBeHq)
                {
                    while (i < listing.ItemListings.Count && !listing.ItemListings[i].IsHq) i++;
                    if (i == listing.ItemListings.Count) return;
                }
            }

            itemPrice = (int)listing.ItemListings[i].PricePerUnit - 1;

            if (itemPrice <= 0) {
                itemPrice = 1;
            }
            PluginLog.Log($"price --->> {itemPrice}");
            if (itemPrice >= 1 && itemPrice < 10000000) {
                if (itemsPrice.TryGetValue(itemName, out var p)) {
                    itemsPrice.Remove(itemName);
                }
                itemsPrice.Add(itemName, itemPrice);
            }
            WoAutoCollectionPlugin.newRequest = false;
        }

        internal unsafe bool AddonRetainerSellList_Position(out Vector2 position)
        {
            position = Vector2.One;
            if (AddonRetainerSellList == IntPtr.Zero)
                return false;

            position = new Vector2(
                ((AtkUnitBase*)AddonRetainerSellList)->X,
                ((AtkUnitBase*)AddonRetainerSellList)->Y
            );
            PluginLog.Log($"AddonRetainerSellList : {position.X}, {position.Y}");
            return true;
        }

        private void AddonRetainerSellList_OnFinalize_Delegate_Detour(IntPtr addon)
        {
            if (addon == AddonRetainerSellList)
                PluginLog.Log($"AddonRetainerSellList.OnFinalize (known: {addon:X})");
            else
                PluginLog.Log(
                    $"AddonRetainerSellList.OnFinalize (unk. have {AddonRetainerSellList:X} got {addon:X})");
            AddonRetainerSellList = IntPtr.Zero;
            itemsPrice = new();
            itemName = "";
            itemPrice = 10000000;
            AddonRetainerSellList_OnFinalize_HW.Original(addon);
        }

        private IntPtr AddonRetainerSellList_OnSetup_Delegate_Detour(IntPtr addon, uint a2, IntPtr dataPtr)
        {
            PluginLog.Log($"AddonRetainerSellList.OnSetup (got: {addon:X})");
            var result = AddonRetainerSellList_OnSetup_HW.Original(addon, a2, dataPtr);
            AddonRetainerSellList = addon;

            itemsPrice = new();
            itemName = "";
            itemPrice = 10000000;
            return result;
        }

        public void Dispose()
        {
            AddonItemSearchResult_ReceiveEvent_HW.Dispose();
            AddonRetainerSell_OnSetup_HW.Dispose();
            AddonItemSearchResult_OnSetup_HW.Dispose();

            AddonInventoryManager_MoveItemSlot_HW.Dispose();
        }

        private unsafe IntPtr AddonRetainerSell_OnSetup_Delegate_Detour(IntPtr addon, uint a2, IntPtr dataPtr)
        {
            PluginLog.Log($"AddonRetainerSell.OnSetup, {a2} {dataPtr.ToInt64}");
            var result = AddonRetainerSell_OnSetup_HW.Original(addon, a2, dataPtr);

            //open compare prices list on opening sell price selection
            var addonRetainerSell = (AddonRetainerSell*)addon;
            var comparePrices = addonRetainerSell->ComparePrices->AtkComponentBase.OwnerNode;
            itemName = CommonUi.GetNodeText(addonRetainerSell->ItemName);
            PluginLog.Log($"itemName: {itemName}");
            if (itemsPrice.TryGetValue(itemName, out var p))
            {
                PluginLog.Log($"有缓存数据: {p}");
                SetPrice();
            }
            else
            {
                // Client::UI::AddonRetainerSell.ReceiveEvent this=0x214C05CB480 evt=EventType.CHANGE               a3=4   a4=0x2146C18C210 (src=0x214C05CB480; tgt=0x214606863B0) a5=0xBB316FE6C8
                MarketCommons.SendClick(addon, EventType.CHANGE, 4, comparePrices);
            }
            return result;
        }

        private unsafe IntPtr AddonItemSearchResult_OnSetup_Delegate_Detour(IntPtr addon, uint a2, IntPtr dataPtr)
        {
            PluginLog.Log($"AddonItemSearchResult.OnSetup, {a2}, {dataPtr.ToInt64}");
            var result = AddonItemSearchResult_OnSetup_HW.Original(addon, a2, dataPtr);

            if (!CommonUi.ItemSearchIsOpen()) {
                Task task = new(() =>
                {
                    Thread.Sleep(1000);
                    // close ItemSearchResult
                    // Component::GUI::AtkComponentWindow.ReceiveEvent this=0x1AC801863B0 evt=EventType.CHANGE               a3=2   a4=0x1AC66640090 (src=0x1AC801863B0; tgt=0x1AC98B47EA0) a5=0x4AAAEFE388
                    var addonItemSearchResult = MarketCommons.GetUnitBase("ItemSearchResult");
                    if (addonItemSearchResult != null)
                        MarketCommons.SendClick(new IntPtr(addonItemSearchResult->WindowNode->Component), EventType.CHANGE, 2,
                            addonItemSearchResult->WindowNode->Component->UldManager
                                .NodeList[7]->GetComponent()->OwnerNode);

                    SetPrice();
                });
                task.Start();
            }

            return result;
        }

        private IntPtr AddonItemSearchResult_ReceiveEvent_Delegate_Detour(IntPtr self, ushort eventType,
            uint eventParam, IntPtr eventStruct, IntPtr /* AtkResNode* */ nodeParam)
        {
            PluginLog.Log($"AddonItemSearchResult_ReceiveEvent_Delegate_Detour, {eventType}, {eventParam}");
            var result =
                AddonItemSearchResult_ReceiveEvent_HW.Original(self, eventType, eventParam, eventStruct, nodeParam);

            return result;
        }

        private int AddonInventoryManager_MoveItemSlot_Delegate_Detour(InventoryType srcContainer, uint srcSlot, 
            InventoryType dstContainer, uint dstSlot, byte unk)
        {
            PluginLog.Log($"AddonInventoryManager_MoveItemSlot_Delegate_Detour, {srcContainer}, {srcSlot}, {dstContainer}, {dstSlot}, {unk}");
            var result =
                AddonInventoryManager_MoveItemSlot_HW.Original(srcContainer, srcSlot, dstContainer, dstSlot, unk);

            PluginLog.Log($"AddonInventoryManager_MoveItemSlot_HW result, {result}");
            return result;
        }

        private void* AddonInventoryContext_OnSetup_Delegate_Detour(AgentInventoryContext* agent, InventoryType inventoryType, ushort slot, int a4, ushort a5, byte a6)
        {
            PluginLog.Log($"AddonInventoryContext_OnSetup_Delegate_Detour, {inventoryType}, {slot}, {a4}, {a5}, {a6}");
            var result =
                AddonInventoryContext_OnSetup_HW.Original(agent, inventoryType, slot, a4, a5, a6);

            try
            {
                if (false) {
                    var inventory = InventoryManager.Instance()->GetInventoryContainer(inventoryType);
                    if (inventory != null)
                    {
                        var itemSlot = inventory->GetInventorySlot(slot);
                        if (itemSlot != null)
                        {
                            var itemId = itemSlot->ItemID;
                            var item = WoAutoCollectionPlugin.GameData.DataManager.GetExcelSheet<Item>()?.GetRow(itemId);
                            if (item != null)
                            {
                                var addonId = agent->AgentInterface.GetAddonID();
                                if (addonId == 0) return result;
                                var addon = GetAddonByID(addonId);
                                if (addon == null) return result;

                                for (var i = 0; i < agent->ContextItemCount; i++)
                                {
                                    var contextItemParam = agent->EventParamsSpan[agent->ContexItemStartIndex + i];
                                    if (contextItemParam.Type != ValueType.String) continue;
                                    var contextItemName = contextItemParam.ToString();

                                    var text = "Put Up for Sale";
                                    if (text.Contains(contextItemName))
                                    {
                                        //if (Bitmask.IsBitSet(agent->ContextItemDisabledMask, i))
                                        //{
                                        //    PluginLog.Debug($"QRA found {i}:{contextItemName} but it's disabled");
                                        //    continue;
                                        //}
                                        Util.GenerateCallback(addon, 0, i, 0U, 0, 0);
                                        agent->AgentInterface.Hide();
                                        UiHelper.Close(addon);
                                        PluginLog.Debug($"QRA Selected {i}:{contextItemName}");
                                        return result;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Log($"AddonInventoryContext_OnSetup_Delegate_Detour error, {ex.Message} !!!");
            }

            return result;
        }

        private unsafe void SetPrice()
        {
            var retainerSell = MarketCommons.GetUnitBase("RetainerSell");
            if (retainerSell == null) return;

            if (retainerSell->UldManager.NodeListCount != 23) {
                PluginLog.LogError("Unexpected fields in addon RetainerSell");
                return;
            }

            if (itemsPrice.TryGetValue(itemName, out var p)) {
                // 价格
                var priceComponentNumericInput = (AtkComponentNumericInput*)retainerSell->UldManager.NodeList[15]->GetComponent();
                // 数量
                var quantityComponentNumericInput = (AtkComponentNumericInput*)retainerSell->UldManager.NodeList[11]->GetComponent();
                PluginLog.Log($"componentNumericInput: {new IntPtr(priceComponentNumericInput).ToString("X")}");
                PluginLog.Log($"componentNumericInput: {new IntPtr(quantityComponentNumericInput).ToString("X")}");
                priceComponentNumericInput->SetValue(p);

                // click confirm on RetainerSell
                // Client::UI::AddonRetainerSell.ReceiveEvent this=0x214B4D360E0 evt=EventType.CHANGE  a3=21  a4=0x214B920D2E0 (src=0x214B4D360E0; tgt=0x21460686550) a5=0xBB316FE6C8
                var addonRetainerSell = (AddonRetainerSell*)retainerSell;
                if (DalamudApi.KeyState[Keys.shift_key])
                {
                    MarketCommons.SendClick(new IntPtr(addonRetainerSell), EventType.CHANGE, 21, addonRetainerSell->Confirm);
                }
            }
        }

        private bool IsDataValid(MarketBoardCurrentOfferings listing)
        {
            // handle early items / if the first request has less than 10
            if (listing.ListingIndexStart == 0 && listing.ListingIndexEnd == 0)
            {
                return true;
            }

            // handle paged requests. 10 per request
            var neededItems = listing.ListingIndexStart + listing.ItemListings.Count;
            var actualItems = WoAutoCollectionPlugin._cache.Sum(x => x.ItemListings.Count);
            return (neededItems == actualItems);
        }

        private bool Retainer()
        {
            return (WoAutoCollectionPlugin.getFilePtr != null) && Marshal.ReadInt64(WoAutoCollectionPlugin.getFilePtr(7), 0xB0) != 0;
        }

        public static AtkUnitBase* GetAddonByID(uint id)
        {
            return AtkStage.GetSingleton()->RaptureAtkUnitManager->GetAddonById((ushort)id);
        }
    }
}
