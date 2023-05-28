using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlphaProject.Craft;
using AlphaProject.ECommons;
using AlphaProject.Helper;

namespace AlphaProject.Bot
{
    internal unsafe class AutoCraftBot
    {
        internal static bool AutoCraftBotEnable = false;
        internal static CircularBuffer<long> Errors = new(5);
        internal static int RecipeID = 0;
        public static List<Task> Tasks = new();

        internal static void Init()
        {
            SignatureHelper.Initialise(new AutoCraftBot());
            DalamudApi.Framework.Update += Framework_Update;
            DalamudApi.ToastGui.ErrorToast += Toasts_ErrorToast;
        }

        internal static void Dispose()
        {
            AutoCraftBotEnable = false;
            DalamudApi.Framework.Update -= Framework_Update;
            DalamudApi.ToastGui.ErrorToast -= Toasts_ErrorToast;
        }

        private static void Toasts_ErrorToast(ref Dalamud.Game.Text.SeStringHandling.SeString message, ref bool isHandled)
        {
            if (AutoCraftBotEnable)
            {
                Errors.PushBack(Environment.TickCount64);
                if (Errors.Count() >= 5 && Errors.All(x => x > Environment.TickCount64 - 30 * 1000))
                {
                    PluginLog.Error("Endurance has been disabled due to too many errors in succession.");
                    AutoCraftBotEnable = false;
                }
            }
        }

        private static void Framework_Update(Dalamud.Game.Framework framework)
        {
            if (AutoCraftBotEnable)
            {
                var isCrafting = DalamudApi.Condition[ConditionFlag.Crafting];
                var preparing = DalamudApi.Condition[ConditionFlag.PreparingToCraft];

                if (!Throttler.Throttle(0))
                {
                    return;
                }

                if (DalamudApi.Condition[ConditionFlag.Occupied39])
                {
                    Throttler.Rethrottle(1000);
                }

                if (SpiritbondHelper.IsSpiritbondReadyAny())
                {
                    if (AlphaProject.Debug) PluginLog.Verbose("Entered materia extraction");
                    if (GenericHelper.TryGetAddonByName<AtkUnitBase>("RecipeNote", out var addon) && addon->IsVisible && DalamudApi.Condition[ConditionFlag.Crafting])
                    {
                        if (AlphaProject.Debug) PluginLog.Verbose("Crafting");
                        if (Throttler.Throttle(1000))
                        {
                            if (AlphaProject.Debug) PluginLog.Verbose("Closing crafting log");
                            CraftHelper.CloseCraftingMenu();
                        }
                    }
                    if (!SpiritbondHelper.IsMateriaMenuOpen() && !isCrafting && !preparing)
                    {
                        SpiritbondHelper.OpenMateriaMenu();
                    }
                    if (SpiritbondHelper.IsMateriaMenuOpen() && !isCrafting && !preparing)
                    {
                        SpiritbondHelper.ExtractFirstMateria();
                    }
                }
                else
                {
                    SpiritbondHelper.CloseMateriaMenu();
                }

                if (!RepairHelper.ProcessRepair(false) && ((!SpiritbondHelper.IsSpiritbondReadyAny())))
                {
                    if (AlphaProject.Debug) PluginLog.Verbose("Entered repair check");
                    if (GenericHelper.TryGetAddonByName<AtkUnitBase>("RecipeNote", out var addon) && addon->IsVisible && DalamudApi.Condition[ConditionFlag.Crafting])
                    {
                        if (AlphaProject.Debug) PluginLog.Verbose("Crafting");
                        if (Throttler.Throttle(1000))
                        {
                            if (AlphaProject.Debug) PluginLog.Verbose("Closing crafting log");
                            CraftHelper.CloseCraftingMenu();
                        }
                    }
                    else
                    {
                        if (AlphaProject.Debug) PluginLog.Verbose("Not crafting");
                        if (!DalamudApi.Condition[ConditionFlag.Crafting]) RepairHelper.ProcessRepair(true);
                    }
                    return;
                }

                if (CraftHelper.RecipeWindowOpen())
                {
                    if (AlphaProject.Debug) PluginLog.Verbose("Addon visible");
                    if (Tasks.Count == 0)
                    {
                        Tasks.Add(DalamudApi.Framework.RunOnTick(CurrentCraft.RepeatActualCraft, TimeSpan.FromMilliseconds(300)));
                    }
                }
                else
                {
                    if (!DalamudApi.Condition[ConditionFlag.Crafting])
                    {
                        if (AlphaProject.Debug) PluginLog.Verbose("Addon invisible");
                        if (Tasks.Count == 0 && !DalamudApi.Condition[ConditionFlag.Crafting40])
                        {
                            if (AlphaProject.Debug) PluginLog.Verbose("Opening crafting log");
                            if (RecipeID == 0)
                            {
                                CommandProcessorHelper.ExecuteThrottled("/clog");
                            }
                            else
                            {
                                if (AlphaProject.Debug) PluginLog.Debug($"Opening recipe {RecipeID}");
                                AgentRecipeNote.Instance()->OpenRecipeByRecipeIdInternal((uint)RecipeID);
                            }
                        }
                    }
                }
            }
        }
    }
}