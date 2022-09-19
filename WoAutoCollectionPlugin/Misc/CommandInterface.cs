﻿using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using WoAutoCollectionPlugin.Exceptions;

namespace WoAutoCollectionPlugin.Misc
{
    public class CommandInterface : ICommandInterface
    {
        /// <summary>
        /// Gets the static instance.
        /// </summary>
        internal static CommandInterface Instance { get; } = new();

        /// <inheritdoc/>
        public bool IsCrafting()
            => DalamudApi.Condition[ConditionFlag.Crafting] && !DalamudApi.Condition[ConditionFlag.PreparingToCraft];

        /// <inheritdoc/>
        public bool IsNotCrafting()
            => !this.IsCrafting();

        /// <inheritdoc/>
        public unsafe bool IsCollectable()
        {
            var addon = this.GetSynthesisAddon();
            return addon->AtkUnitBase.UldManager.NodeList[34]->IsVisible;
        }

        /// <inheritdoc/>
        public unsafe string GetCondition(bool lower = true)
        {
            var addon = this.GetSynthesisAddon();
            var text = addon->Condition->NodeText.ToString();

            if (lower)
                text = text.ToLowerInvariant();

            return text;
        }

        /// <inheritdoc/>
        public bool HasCondition(string condition, bool lower = true)
        {
            var actual = this.GetCondition(lower);
            return condition == actual;
        }

        public unsafe int GetItemName()
        {
            var addon = this.GetSynthesisAddon();
            return this.GetNodeTextAsInt(addon->ItemName, "Could not parse current item name in the Synthesis addon");
        }

        /// <inheritdoc/>
        public unsafe int GetProgress()
        {
            var addon = this.GetSynthesisAddon();
            return this.GetNodeTextAsInt(addon->CurrentProgress, "Could not parse current progress number in the Synthesis addon");
        }

        /// <inheritdoc/>
        public unsafe int GetMaxProgress()
        {
            var addon = this.GetSynthesisAddon();
            return this.GetNodeTextAsInt(addon->MaxProgress, "Could not parse max progress number in the Synthesis addon");
        }

        /// <inheritdoc/>
        public bool HasMaxProgress()
        {
            var current = this.GetProgress();
            var max = this.GetMaxProgress();
            return current == max;
        }

        /// <inheritdoc/>
        public unsafe int GetQuality()
        {
            var addon = this.GetSynthesisAddon();
            return this.GetNodeTextAsInt(addon->CurrentQuality, "Could not parse current quality number in the Synthesis addon");
        }

        /// <inheritdoc/>
        public unsafe int GetMaxQuality()
        {
            var addon = this.GetSynthesisAddon();
            return this.GetNodeTextAsInt(addon->MaxQuality, "Could not parse max quality number in the Synthesis addon");
        }

        /// <inheritdoc/>
        public bool HasMaxQuality()
        {
            var step = this.GetStep();

            if (step <= 1)
                return false;

            if (this.IsCollectable())
            {
                var current = this.GetQuality();
                var max = this.GetMaxQuality();
                return current == max;
            }
            else
            {
                var percentHq = this.GetPercentHQ();
                return percentHq == 100;
            }
        }

        /// <inheritdoc/>
        public unsafe int GetDurability()
        {
            var addon = this.GetSynthesisAddon();
            return this.GetNodeTextAsInt(addon->CurrentDurability, "Could not parse current durability number in the Synthesis addon");
        }

        /// <inheritdoc/>
        public unsafe int GetMaxDurability()
        {
            var addon = this.GetSynthesisAddon();
            return this.GetNodeTextAsInt(addon->StartingDurability, "Could not parse max durability number in the Synthesis addon");
        }

        /// <inheritdoc/>
        public int GetCp()
        {
            var cp = DalamudApi.ClientState.LocalPlayer?.CurrentCp ?? 0;
            return (int)cp;
        }

        /// <inheritdoc/>
        public int GetMaxCp()
        {
            var cp = DalamudApi.ClientState.LocalPlayer?.MaxCp ?? 0;
            return (int)cp;
        }

        /// <inheritdoc/>
        public unsafe int GetStep()
        {
            var addon = this.GetSynthesisAddon();
            var step = this.GetNodeTextAsInt(addon->StepNumber, "Could not parse current step number in the Synthesis addon");
            return step;
        }

        /// <inheritdoc/>
        public unsafe int GetPercentHQ()
        {
            var addon = this.GetSynthesisAddon();
            var step = this.GetNodeTextAsInt(addon->HQPercentage, "Could not parse percent hq number in the Synthesis addon");
            return step;
        }

        /// <inheritdoc/>
        public unsafe bool NeedsRepair()
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

                // Materia  MateriaGrade
                PluginLog.Log($"{item->Condition}-{item->Container}-{item->CrafterContentID}-{item->Flags}-{item->GlamourID}-{item->ItemID}");
                PluginLog.Log($"{item->Quantity}-{item->Slot}-{item->Spiritbond}-{item->Stain}-{item->GlamourID}-{item->ItemID}");

                if (item->Condition == 0)
                    return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public unsafe bool CanExtractMateria(float within = 100)
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
                PluginLog.Error("InventoryContainer is not loaded");
                return false;
            }

            var nextHighest = 0f;
            var canExtract = false;
            var allExtract = true;
            for (var i = 0; i < equipped->Size; i++)
            {
                var item = equipped->GetInventorySlot(i);
                if (item == null)
                    continue;

                var spiritbond = item->Spiritbond / 100;
                if (spiritbond == 100f)
                {
                    canExtract = true;
                }
                else
                {
                    allExtract = false;
                    nextHighest = Math.Max(spiritbond, nextHighest);
                }
            }

            if (allExtract)
            {
                PluginLog.Debug("All items are spiritbound, pausing");
                return true;
            }

            if (canExtract)
            {
                // Don't wait, extract immediately
                if (within == 100)
                {
                    PluginLog.Debug("An item is spiritbound, pausing");
                    return true;
                }

                // Keep going if the next highest spiritbonded item is within the allowed range
                // i.e. 100 and 99, do another craft to finish the 99.
                if (nextHighest >= within)
                {
                    PluginLog.Debug($"The next highest spiritbond is above ({nextHighest} >= {within}), keep going");
                    return false;
                }
                else
                {
                    PluginLog.Debug($"The next highest spiritbond is below ({nextHighest} < {within}), pausing");
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public unsafe bool HasStats(uint craftsmanship, uint control, uint cp)
        {
            var uiState = UIState.Instance();
            if (uiState == null)
            {
                PluginLog.Error("UIState is null");
                return false;
            }

            var hasStats =
                uiState->PlayerState.Attributes[70] >= craftsmanship &&
                uiState->PlayerState.Attributes[71] >= control &&
                uiState->PlayerState.Attributes[11] >= cp;

            return hasStats;
        }

        /// <inheritdoc/>
        public unsafe bool HasStatus(string statusName)
        {
            statusName = statusName.ToLowerInvariant();
            var sheet = DalamudApi.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Status>()!;
            var statusIDs = sheet
                .Where(row => row.Name.RawString.ToLowerInvariant() == statusName)
                .Select(row => row.RowId)
                .ToArray()!;

            return this.HasStatusId(statusIDs);
        }

        /// <inheritdoc/>
        public unsafe bool HasStatusId(params uint[] statusIDs)
        {
            var statusID = DalamudApi.ClientState.LocalPlayer!.StatusList
                .Select(se => se.StatusId)
                .ToList().Intersect(statusIDs)
                .FirstOrDefault();

            return statusID != default;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public unsafe string GetNodeText(string addonName, int nodeNumber)
        {
            var ptr = DalamudApi.GameGui.GetAddonByName(addonName, 1);
            if (ptr == IntPtr.Zero)
                throw new MacroCommandError($"Could not find {addonName} addon");

            var addon = (AtkUnitBase*)ptr;
            var count = addon->UldManager.NodeListCount;
            if (nodeNumber < 0 || nodeNumber >= count)
                throw new MacroCommandError($"Addon node number must be between 0 and {count} for the {addonName} addon");

            AtkResNode* node = addon->UldManager.NodeList[nodeNumber];
            if (node == null)
                throw new MacroCommandError($"{addonName} addon node[{nodeNumber}] is null");

            if (node->Type != NodeType.Text)
                throw new MacroCommandError($"{addonName} addon node[{nodeNumber}] is not a text node");

            var textNode = (AtkTextNode*)node;
            return textNode->NodeText.ToString();
        }

        /// <inheritdoc/>
        public unsafe string GetSelectStringText(int index)
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("SelectString", 1);
            if (ptr == IntPtr.Zero)
                throw new MacroCommandError("Could not find SelectString addon");

            var addon = (AddonSelectString*)ptr;
            var popup = &addon->PopupMenu.PopupMenu;

            var count = popup->EntryCount;
            PluginLog.Debug($"index={index} // Count={count} // {index < 0 || index > count}");
            if (index < 0 || index > count)
                throw new MacroCommandError("Index out of range");

            var textPtr = popup->EntryNames[index];
            if (textPtr == null)
                throw new MacroCommandError("Text pointer was null");

            return Marshal.PtrToStringUTF8((IntPtr)textPtr) ?? string.Empty;
        }

        // <inheritdoc/>
        public unsafe string GetSelectIconStringText(int index)
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("SelectIconString", 1);
            if (ptr == IntPtr.Zero)
                throw new MacroCommandError("Could not find SelectIconString addon");

            var addon = (AddonSelectIconString*)ptr;
            var popup = &addon->PopupMenu.PopupMenu;

            var count = popup->EntryCount;
            if (index < 0 || index > count)
                throw new MacroCommandError("Index out of range");

            var textPtr = popup->EntryNames[index];
            if (textPtr == null)
                throw new MacroCommandError("Text pointer was null");

            return Marshal.PtrToStringUTF8((IntPtr)textPtr) ?? string.Empty;
        }

        private unsafe int GetNodeTextAsInt(AtkTextNode* node, string error)
        {
            try
            {
                if (node == null)
                    throw new NullReferenceException("TextNode is null");

                var text = node->NodeText.ToString();
                var value = int.Parse(text);
                return value;
            }
            catch (Exception ex)
            {
                throw new MacroCommandError(error, ex);
            }
        }

        private unsafe AddonSynthesis* GetSynthesisAddon()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("Synthesis", 1);
            if (ptr == IntPtr.Zero)
                throw new MacroCommandError("Could not find Synthesis addon");

            return (AddonSynthesis*)ptr;
        }
    }
}
