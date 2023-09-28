using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using AlphaProject.Managers;
using AlphaProject.Ui;
using AlphaProject.Utility;
using AlphaProject.Helper;
using Lumina.Excel.GeneratedSheets;
using AlphaProject.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using AlphaProject.RawInformation;
using ClickLib.Clicks;
using AlphaProject.Craft;
using AlphaProject.Data;
using AlphaProject.SeFunctions;

namespace AlphaProject.Bot
{
    public static class CraftBot
    {
        public static bool Closed = false;

        private static List<uint> JobIds = new();

        private static uint Job = 0;


        public static void Init(uint job)
        {
            Closed = false;
            JobIds = new();
            Job = job;
            CommonBot.Init();
            Tasks.Status = (byte)TaskState.CRAFT;

            if (DalamudApi.Condition[ConditionFlag.Mounted]) {
                Thread.Sleep(3000);
                Teleporter.Teleport(Positions.randomTp());
            }
        }

        public static void StopScript()
        {
            Closed = true;
            CommonBot.StopScript();
        }

        public static void CraftScript() {
            CraftScript(AlphaProject.Configuration.RecipeName, 15);
        }

        public static void CraftScript(string recipeName, uint job)
        {
            Init(job);
            try
            {
                uint recipeId = RecipeNoteUi.SearchRecipeId(recipeName);
                var recipe = CraftHelper.FilteredList[recipeId];
                PluginLog.Log($"---> {recipeName}, {recipeId}");

                if (recipe == null)
                {
                    PluginLog.Error($"not found recipe...");
                    return;
                }

                int craftX = 0;
                while (!Closed && Tasks.TaskRun) {
                    Thread.Sleep(new Random().Next(900, 1200));

                    if (DalamudApi.Condition[ConditionFlag.Occupied39])
                    {
                        Throttler.Rethrottle(1000);
                    }

                    // 修理
                    if (!CommonHelper.RepairTask()) continue;
                    // 精制
                    if (!CommonHelper.ExtractMateriaTask()) continue;
                    // 食物 TODO
                    // 背包容量检查
                    if (BagManager.InventoryRemaining() <= 5)
                    {
                        PluginLog.Log("背包容量不足...");
                        Closed = true;
                        break;
                    }
                    // 原材料检查
                    if (!CraftHelper.CheckForRecipeIngredients(recipe.RowId, out List<uint> lackItems, false) && lackItems.Count > 0) {
                        if (!AlphaProject.Configuration.AutoGather)
                        {
                            PluginLog.Error($"原材料不足 未配置自动采集...");
                            Closed = true;
                            break;
                        }
                        if (AlphaProject.AP.TM.TaskList.Count == 0)
                        {
                            PluginLog.Warning($"{lackItems.Count}, 原材料不足...");
                            AlphaProject.AP.TM.AddTask(lackItems);
                            if (CraftHelper.RecipeNoteWindowOpen()) {
                                CraftHelper.CloseCraftingMenu();
                            }
                            AlphaProject.AP.TM.RunTask();
                            Thread.Sleep(10000);
                            Closed = false;
                        }
                        else {
                            PluginLog.Error($"当前: {recipe.ItemResult.Value.Name}, 任务因缺少原材料结束...num: {lackItems.Count}");
                            //AlphaProject.AP.TM.TaskList.Clear();
                            break;
                        }
                    }

                    if (Job != 0 && !CommonUi.CurrentJob(Job))
                    {
                        Thread.Sleep(1800 + new Random().Next(200, 500));
                        string jobName = RecipeItems.GetJobName(Job);
                        CommandProcessorHelper.DoGearChange(jobName);
                        Thread.Sleep(200 + new Random().Next(200, 400));
                    }

                    // Synth
                    if (!CraftHelper.RecipeNoteWindowOpen() && !CraftHelper.SynthesisWindowOpen())
                    {
                        if (!DalamudApi.Condition[ConditionFlag.Crafting])
                        {
                            RecipeNoteUi.OpenRecipeNote(recipe.RowId);
                            Thread.Sleep(new Random().Next(1200, 2000));
                        }
                    } else
                    {
                        KeyOperates.KeyMethod(Keys.num0_key);
                    }
                    if (!recipe.CanQuickSynth)
                    {
                        PluginLog.Log($"begin synth: {recipe.ItemResult.Value.Name}");
                        RunSynthByRecipe(recipe);
                        craftX++;
                    }
                    else if (recipe.CanQuickSynth)
                    {
                        PluginLog.Log($"begin quick synth: {recipe.ItemResult.Value.Name}");
                        RunQuickSynthByRecipe(recipe);
                    }
                    // 提交收藏品
                    if (recipe.ItemResult.Value.Name.ToString().Contains("收藏") && BagManager.InventoryRemaining() <= 5)
                    {
                        Thread.Sleep(3000);
                        if (CraftHelper.RecipeNoteWindowOpen())
                        {
                            CraftHelper.CloseCraftingMenu();
                        }
                        Thread.Sleep(new Random().Next(900, 1200));
                        // 上交收藏品和交换道具
                        TicketHelper.CraftUploadAndExchange(AlphaProject.Configuration.RecipeName, AlphaProject.Configuration.ExchangeItem);
                        // 上交重建品和交换道具 TODO
                    }
                    Thread.Sleep(new Random().Next(900, 1200));
                }
                PluginLog.Log($"Finish: {craftX} Item: {AlphaProject.Configuration.RecipeName}, {recipe.RowId}");
                if (CraftHelper.RecipeNoteWindowOpen()) {
                    CraftHelper.CloseCraftingMenu();
                }
            }
            catch (Exception e)
            {
                PluginLog.Error($"error!!!\n{e}");
            }
            Tasks.Status = (byte)TaskState.READY;
        }

        public unsafe static void RunSynthByRecipe(Recipe recipe)
        {
            int error = 0;
            if (CraftHelper.RecipeNoteWindowOpen())
            {
                CraftHelper.SetIngredients();

                RecipeNoteUi.SynthesizeButton();
                if (error > 3) {
                    Closed = true;
                    PluginLog.Log($"停止 error: {error}");
                }
                Thread.Sleep(new Random().Next(1800, 2500));
            }

            if (RecipeNoteUi.SynthesisIsOpen())
            {
                while (RecipeNoteUi.SynthesisIsOpen())
                {
                    Thread.Sleep(new Random().Next(1000, 1200));
                    if (Closed)
                    {
                        PluginLog.Log($"craft stopping");
                        return;
                    }
                }
            }
            else {
                error++;
            }
            Thread.Sleep(new Random().Next(1000, 2000));
        }

        public unsafe static void RunQuickSynthByRecipe(Recipe recipe) {
            var recipeWindow = DalamudApi.GameGui.GetAddonByName("RecipeNote", 1);
            if (recipeWindow == IntPtr.Zero)
                return;

            var quickSynthPTR = DalamudApi.GameGui.GetAddonByName("SynthesisSimpleDialog", 1);
            if (quickSynthPTR == IntPtr.Zero) {
                ClickRecipeNote.Using(recipeWindow).QuickSynthesis();
                Thread.Sleep(new Random().Next(1000, 2000));
                quickSynthPTR = DalamudApi.GameGui.GetAddonByName("SynthesisSimpleDialog", 1);
            }

            if (quickSynthPTR == IntPtr.Zero)
                return;

            var quickSynthWindow = (AtkUnitBase*)quickSynthPTR;
            if (quickSynthWindow == null)
                return;

            var numericInput = (AtkComponentNode*)quickSynthWindow->UldManager.NodeList[4];
            if (numericInput == null)
                return;
            var numericComponent = (AtkComponentNumericInput*)numericInput->Component;

            var values = stackalloc AtkValue[2];
            values[0] = new()
            {
                Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                Int = numericComponent->Data.Max,
            };
            values[1] = new()
            {
                Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Bool,
                Byte = 1,
            };
            quickSynthWindow->FireCallback(3, values);

            Thread.Sleep(new Random().Next(1000, 2000));
            while (!AutoCraft.currentCraftFinished)
            {
                PluginLog.Log($"wait quick synth: {AutoCraft.currentCraftFinished}");
                Thread.Sleep(new Random().Next(8000, 15000));

                if (!CraftHelper.SynthesisSimpleWindowOpen()) {
                    return;
                }
            }
            PluginLog.Log($"end quick synth: {AutoCraft.currentCraftFinished}");
            Thread.Sleep(new Random().Next(3000, 5000));
            if (CraftHelper.SynthesisSimpleWindowOpen())
            {
                CloseQuickSynthWindow();
            }
        }

        public unsafe static void CloseQuickSynthWindow()
        {
            try
            {
                var quickSynthPTR = DalamudApi.GameGui.GetAddonByName("SynthesisSimple", 1);
                if (quickSynthPTR == IntPtr.Zero)
                    return;

                var quickSynthWindow = (AtkUnitBase*)quickSynthPTR;
                if (quickSynthWindow == null)
                    return;

                var qsynthButton = (AtkComponentButton*)quickSynthWindow->UldManager.NodeList[2];
                if (qsynthButton != null && !qsynthButton->IsEnabled)
                {
                    qsynthButton->AtkComponentBase.OwnerNode->AtkResNode.Flags ^= 1 << 5;
                }
                AtkResNodeFunctions.ClickButton(quickSynthWindow, qsynthButton, 0);
            }
            catch (Exception e)
            {
                PluginLog.Log($"close quick window error: {e.Message}");
            }
        }

        public unsafe static void test() {
            var quickSynthPTR = DalamudApi.GameGui.GetAddonByName("SynthesisSimpleDialog", 1);
            if (quickSynthPTR == IntPtr.Zero)
                return;

            var quickSynthWindow = (AtkUnitBase*)quickSynthPTR;
            if (quickSynthWindow == null)
                return;

            var numericInput = (AtkComponentNode*)quickSynthWindow->UldManager.NodeList[4];
            if (numericInput == null)
                return;
            var numericComponent = (AtkComponentNumericInput*)numericInput->Component;
            //numericComponent->SetValue(numericComponent->Data.Max);

            var values = stackalloc AtkValue[2];
            values[0] = new()
            {
                Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Int,
                Int = numericComponent->Data.Max,
            };
            values[1] = new()
            {
                Type = FFXIVClientStructs.FFXIV.Component.GUI.ValueType.Bool,
                Byte = 1,
            };
            quickSynthWindow->FireCallback(3, values);
        }
    }
}