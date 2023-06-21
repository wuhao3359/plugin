﻿using AlphaProject.RawInformation;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Linq;
using static AlphaProject.Craft.CurrentCraft;
using AlphaProject.Craft;
using Dalamud.Logging;

namespace AlphaProject.Actions
{
    internal unsafe class ActionWatching
    {
        public delegate byte UseActionDelegate(ActionManager* actionManager, uint actionType, uint actionID, long targetObjectID, uint param, uint useType, int pvp, bool* isGroundTarget);
        public static Hook<UseActionDelegate> UseActionHook;
        public static uint LastUsedAction = 0;

        private delegate void* ClickSynthesisButton(void* a1, void* a2);
        private static Hook<ClickSynthesisButton> clickSysnthesisButtonHook;
        private static byte UseActionDetour(ActionManager* actionManager, uint actionType, uint actionID, long targetObjectID, uint param, uint useType, int pvp, bool* isGroundTarget)
        {
            try
            {
                if (CanUse(actionID))
                {
                    PreviousAction = actionID;

                    if (LuminaSheets.ActionSheet.TryGetValue(actionID, out var act1))
                    {
                        string skillName = act1.Name;
                        var allOfSameName = LuminaSheets.ActionSheet.Where(x => x.Value.Name == skillName).Select(x => x.Key);

                        if (allOfSameName.Any(x => x == Skills.Manipulation))
                            ManipulationUsed = true;

                        if (allOfSameName.Any(x => x == Skills.WasteNot || x == Skills.WasteNot2))
                            WasteNotUsed = true;

                        if (allOfSameName.Any(x => x == Skills.FinalAppraisal))
                        {
                            JustUsedFinalAppraisal = true;
                            CurrentRecommendation = 0;
                            AutoCraft.Tasks.Clear();
                        }
                        else
                            JustUsedFinalAppraisal = false;

                        if (allOfSameName.Any(x => x == Skills.GreatStrides))
                            JustUsedGreatStrides = true;
                        else
                            JustUsedGreatStrides = false;

                        if (allOfSameName.Any(x => x == Skills.Innovation))
                            InnovationUsed = true;

                        if (allOfSameName.Any(x => x == Skills.Veneration))
                            VenerationUsed = true;

                        JustUsedObserve = false;
                        BasicTouchUsed = false;
                        StandardTouchUsed = false;
                        AdvancedTouchUsed = false;

                    }
                    if (LuminaSheets.CraftActions.TryGetValue(actionID, out var act2))
                    {
                        string skillName = act2.Name;
                        var allOfSameName = LuminaSheets.CraftActions.Where(x => x.Value.Name == skillName).Select(x => x.Key);

                        if (allOfSameName.Any(x => x == Skills.Observe))
                            JustUsedObserve = true;
                        else
                            JustUsedObserve = false;

                        if (allOfSameName.Any(x => x == Skills.BasicTouch))
                            BasicTouchUsed = true;
                        else
                            BasicTouchUsed = false;

                        if (allOfSameName.Any(x => x == Skills.StandardTouch))
                            StandardTouchUsed = true;
                        else
                            StandardTouchUsed = false;

                        if (allOfSameName.Any(x => x == Skills.AdvancedTouch))
                            AdvancedTouchUsed = true;
                        else
                            AdvancedTouchUsed = false;

                        JustUsedFinalAppraisal = false;

                        if (allOfSameName.Any(x => x == Skills.HeartAndSoul))
                        {
                            CurrentRecommendation = 0;
                            AutoCraft.Tasks.Clear();
                        }

                        if (allOfSameName.Any(x => x == Skills.CarefulObservation))
                        {
                            CurrentRecommendation = 0;
                            AutoCraft.Tasks.Clear();
                        }
                    }
                    CurrentRecommendation = 0;
                    AutoCraft.Tasks.Clear();
                }
                return UseActionHook!.Original(actionManager, actionType, actionID, targetObjectID, param, useType, pvp, isGroundTarget);
            }
            catch (Exception ex)
            {
                Dalamud.Logging.PluginLog.Error(ex, "UseActionDetour");
                return UseActionHook!.Original(actionManager, actionType, actionID, targetObjectID, param, useType, pvp, isGroundTarget);
            }
        }

        static ActionWatching()
        {
            UseActionHook ??= Hook<UseActionDelegate>.FromAddress((IntPtr)ActionManager.Addresses.UseAction.Value, UseActionDetour);
            clickSysnthesisButtonHook ??= Hook<ClickSynthesisButton>.FromAddress(DalamudApi.SigScanner.ScanText("E9 ?? ?? ?? ?? 4C 8B 44 24 ?? 49 8B D2 48 8B CB 48 83 C4 30 5B E9 ?? ?? ?? ?? 4C 8B 44 24 ?? 49 8B D2 48 8B CB 48 83 C4 30 5B E9 ?? ?? ?? ?? 33 D2"), ClickSynthesisButtonDetour);
            clickSysnthesisButtonHook?.Enable();
        }

        private static void* ClickSynthesisButtonDetour(void* a1, void* a2) {
            return clickSysnthesisButtonHook.Original(a1, a2);
        }

        public static void TryEnable()
        {
            if (!UseActionHook.IsEnabled)
                UseActionHook?.Enable();
        }

        public static void TryDisable()
        {
            if (UseActionHook.IsEnabled)
                UseActionHook?.Disable();
        }
        public static void Enable()
        {
            UseActionHook?.Enable();
        }

        public static void Disable()
        {
            UseActionHook?.Disable();
        }

        public static void Dispose()
        {
            UseActionHook?.Dispose();
        }

    }
}
