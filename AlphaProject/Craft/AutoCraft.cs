using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using System;
using System.Linq;
using Dalamud.Logging;
using AlphaProject.RawInformation;
using static AlphaProject.Craft.CurrentCraft;
using System.Threading.Tasks;
using System.Collections.Generic;
using AlphaProject.Bot;
using AlphaProject.Actions;

namespace AlphaProject.Craft
{
    public unsafe class AutoCraft : IDisposable
    {
        public static readonly object _lockObj = new();
        public static List<Task> Tasks = new();

        public static bool currentCraftFinished = false;
        public static bool warningMessage = false;

        public AutoCraft() {
            DalamudApi.Framework.Update += EnableBot;
            StepChanged += ResetRecommendation;
            DalamudApi.Condition.ConditionChange += Condition_ConditionChange;
            DalamudApi.ChatGui.ChatMessage += ScanForHQItems;
            ActionWatching.TryEnable();
        }

        public void Dispose()
        {
            DalamudApi.Framework.Update -= EnableBot;
            StepChanged -= ResetRecommendation;
            DalamudApi.Condition.ConditionChange -= Condition_ConditionChange;
            DalamudApi.ChatGui.ChatMessage -= ScanForHQItems;
            ActionWatching.TryDisable();
        }

        public static bool CheckIfCraftFinished()
        {
            if (QuickSynthMax > 0 && QuickSynthCurrent == QuickSynthMax) return true;
            if (MaxProgress == 0) return false;
            if (CurrentProgress == MaxProgress) return true;
            if (CurrentProgress < MaxProgress && CurrentDurability == 0) return true;
            currentCraftFinished = false;
            return false;
        }

        private void EnableBot(Framework framework) {
            if (!DalamudApi.ClientState.IsLoggedIn)
            {
                AutoCraftBot.AutoCraftBotEnable = false;
                return;
            }

            GetCraft();

            if (CanUse(Skills.BasicSynth) && CurrentRecommendation == 0 && Tasks.Count == 0 && CurrentStep >= 1)
            {
                if (Recipe is null && !warningMessage)
                {
                    PluginLog.Error("Warning: Your recipe cannot be parsed in Artisan. Please report this to the Discord with the recipe name and client language.");
                    warningMessage = true;
                }
                else
                {
                    warningMessage = false;
                }

                if (warningMessage)
                    return;

                Tasks.Add(DalamudApi.Framework.RunOnTick(() => FetchRecommendation(CurrentStep), TimeSpan.FromMilliseconds(500)));
            } 

            if (CheckIfCraftFinished() && !currentCraftFinished)
            {
                currentCraftFinished = true;
            }
        }

        public void FetchRecommendation(int e)
        {
            lock (_lockObj)
            {
                try
                {
                    CurrentRecommendation = Recipe.IsExpert ? GetExpertRecommendation() : GetRecommendation();
                    RecommendationName = CurrentRecommendation.NameOfAction();

                    PluginLog.Log($"{CurrentRecommendation} - {RecommendationName}");
                    if (CurrentRecommendation != 0)
                    {
                        if (LuminaSheets.ActionSheet.TryGetValue(CurrentRecommendation, out var normalAct))
                        {
                            if (normalAct.ClassJob.Value.RowId != CharacterInfo.JobID())
                            {
                                var newAct = LuminaSheets.ActionSheet.Values.Where(x => x.Name.RawString == normalAct.Name.RawString && x.ClassJob.Row == CharacterInfo.JobID()).FirstOrDefault();
                                CurrentRecommendation = newAct.RowId;
                            }
                        }

                        if (LuminaSheets.CraftActions.TryGetValue(CurrentRecommendation, out var craftAction))
                        {
                            if (craftAction.ClassJob.Row != CharacterInfo.JobID())
                            {
                                var newAct = LuminaSheets.CraftActions.Values.Where(x => x.Name.RawString == craftAction.Name.RawString && x.ClassJob.Row == CharacterInfo.JobID()).FirstOrDefault();
                                CurrentRecommendation = newAct.RowId;
                            }
                        }
                        DalamudApi.Framework.RunOnTick(() => Hotbars.ExecuteRecommended(CurrentRecommendation), TimeSpan.FromMilliseconds(new Random().Next(800, 1100)));
                    }
                }
                catch (Exception ex)
                {
                    PluginLog.Error(ex, "FetchRecommendation");
                }
            }
        }

        private void ResetRecommendation(object? sender, int e)
        {
            CurrentRecommendation = 0;
            if (e == 0)
            {
                ManipulationUsed = false;
                JustUsedObserve = false;
                VenerationUsed = false;
                InnovationUsed = false;
                WasteNotUsed = false;
                JustUsedFinalAppraisal = false;
                BasicTouchUsed = false;
                StandardTouchUsed = false;
                AdvancedTouchUsed = false;
                ExpertCraftOpenerFinish = false;
            }
            if (e > 0)
                Tasks.Clear();
        }

        private void Condition_ConditionChange(ConditionFlag flag, bool value)
        {
            if (AlphaProject.Debug) PluginLog.Verbose($"Condition_ConditionChange: {flag}");
            if (DalamudApi.Condition[ConditionFlag.PreparingToCraft])
            {
                State = CraftingState.PreparingToCraft;
                return;
            }
            if (DalamudApi.Condition[ConditionFlag.Crafting] && !DalamudApi.Condition[ConditionFlag.PreparingToCraft])
            {
                State = CraftingState.Crafting;
                return;
            }
            if (!DalamudApi.Condition[ConditionFlag.Crafting] && !DalamudApi.Condition[ConditionFlag.PreparingToCraft])
            {
                State = CraftingState.NotCrafting;
                return;
            }
        }

        private void ScanForHQItems(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (AlphaProject.Debug) PluginLog.Verbose("ScanForHQItems Test");
            if (type == (XivChatType)2242 && DalamudApi.Condition[ConditionFlag.Crafting])
            {
                if (message.Payloads.Any(x => x.Type == PayloadType.Item))
                {
                    var item = (ItemPayload)message.Payloads.First(x => x.Type == PayloadType.Item);
                    if (item.Item.CanBeHq)
                        LastItemWasHQ = item.IsHQ;

                    LastCraftedItem = item.Item;
                }
            }
        }
    }
}
