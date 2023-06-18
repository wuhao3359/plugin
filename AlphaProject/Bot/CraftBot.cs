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
    public class CraftBot
    {
        private static bool closed = false;

        private List<uint> jobIds = new();

        private uint job = 0;

        public CraftBot() {}

        public void Init(uint job)
        {
            closed = false;
            jobIds = new();
            this.job = job;
            AlphaProject.GameData.CommonBot.Init();
            AlphaProject.status = (byte)TaskState.CRAFT;
        }

        public void StopScript()
        {
            closed = true;
            AlphaProject.GameData.CommonBot.StopScript();
        }

        public void CraftScript() {
            CraftScript(AlphaProject.Configuration.RecipeName, 15);
        }

        public void CraftScript(string recipeName, uint job)
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
                while (!closed) {
                    Thread.Sleep(new Random().Next(900, 1200));
                    // 修理
                    if (CommonUi.NeedsRepair())
                    {
                        PluginLog.Error("Repaair...");
                        Thread.Sleep(3000);
                        if (CraftHelper.RecipeNoteWindowOpen())
                        {
                            CraftHelper.CloseCraftingMenu();
                        }

                        Teleporter.Teleport(Positions.ShopTp);
                        AlphaProject.GameData.KeyOperates.MovePositions(Positions.RepairNPC, false);
                        AlphaProject.GameData.CommonBot.NpcRepair("阿塔帕");
                        Thread.Sleep(500 + new Random().Next(100, 300));
                    }
                    // 魔晶石精制 TODO
                    //int em = CommonUi.CanExtractMateria();
                    //if (em > 2)
                    //{
                    //    if (CraftHelper.RecipeNoteWindowOpen())
                    //    {
                    //        CraftHelper.CloseCraftingMenu();
                    //    }
                    //    PluginLog.Error("ExtractMateria...");
                    //    Thread.Sleep(3000);
                    //    AlphaProject.GameData.CommonBot.ExtractMateria(em);
                    //}
                    // 食物 TODO
                    // 背包容量检查
                    if (BagManager.InventoryRemaining() <= 5)
                    {
                        PluginLog.Log("背包容量不足...");
                        closed = true;
                        break;
                    }
                    // 原材料检查
                    if (!CraftHelper.CheckForRecipeIngredients(recipe.RowId, out List<uint> lackItems, false) && lackItems.Count > 0) {
                        if (!AlphaProject.Configuration.AutoGather)
                        {
                            PluginLog.Error($"原材料不足 未配置自动采集...");
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
                            closed = false;
                        }
                        else {
                            PluginLog.Error($"当前任务因缺少原材料结束...num: {lackItems.Count}");
                            break;
                        }
                    }

                    if (this.job != 0 && !CommonUi.CurrentJob(this.job))
                    {
                        Thread.Sleep(1800 + new Random().Next(200, 500));
                        string jobName = RecipeItems.GetJobName(this.job);
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
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
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
            AlphaProject.status = (byte)TaskState.READY;
        }

        public unsafe void RunSynthByRecipe(Recipe recipe)
        {
            int error = 0;
            if (CraftHelper.RecipeNoteWindowOpen())
            {
                CraftHelper.SetIngredients();

                RecipeNoteUi.SynthesizeButton();
                if (error > 3) {
                    closed = true;
                    PluginLog.Log($"停止 error: {error}");
                }
                Thread.Sleep(new Random().Next(1800, 2500));
            }

            if (RecipeNoteUi.SynthesisIsOpen())
            {
                while (RecipeNoteUi.SynthesisIsOpen())
                {
                    Thread.Sleep(new Random().Next(1000, 1200));
                    if (closed)
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

        public unsafe void RunQuickSynthByRecipe(Recipe recipe) {
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

            if (CraftHelper.SynthesisSimpleWindowOpen())
            {
                CloseQuickSynthWindow();
            }
        }

        public unsafe void CloseQuickSynthWindow()
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

        public unsafe void test() {
            //var quickSynthPTR = DalamudApi.GameGui.GetAddonByName("SynthesisSimpleDialog", 1);
            //if (quickSynthPTR == IntPtr.Zero)
            //    return;

            //var quickSynthWindow = (AtkUnitBase*)quickSynthPTR;
            //var qsynthButton = (AtkComponentButton*)quickSynthWindow->UldManager.NodeList[3];
            //if (qsynthButton != null && !qsynthButton->IsEnabled)
            //{
            //    qsynthButton->AtkComponentBase.OwnerNode->AtkResNode.Flags ^= 1 << 5;
            //}

            //AtkResNodeFunctions.ClickButton(quickSynthWindow, qsynthButton, 1);

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