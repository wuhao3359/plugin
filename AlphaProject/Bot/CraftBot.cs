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

namespace AlphaProject.Bot
{
    public class CraftBot
    {
        private static bool closed = false;

        private List<uint> jobIds = new();

        public CraftBot() {}

        public void Init()
        {
            closed = false;
            jobIds = new();
            AlphaProject.GameData.CommonBot.Init();
            AlphaProject.status = (byte)TaskState.CRAFT;
        }

        public void StopScript()
        {
            closed = true;
            AlphaProject.GameData.CommonBot.StopScript();
        }

        public void CraftScript() {
            CraftScript(AlphaProject.Configuration.RecipeName);
        }

        public void CraftScript(string recipeName)
        {
            Init();
            try
            {
                uint recipeId = RecipeNoteUi.SearchRecipeId(recipeName);
                var recipe = CraftHelper.FilteredList[recipeId];
                PluginLog.Log($"---> {AlphaProject.Configuration.RecipeName}, {recipeId}");

                if (recipe == null)
                {
                    PluginLog.Error($"not found recipe...");
                    return;
                }

                int craftX = 0;
                while (!closed) {
                    Thread.Sleep(new Random().Next(900, 1200));
                    // 修理 TODO 重构
                    if (CommonUi.NeedsRepair())
                    {
                        PluginLog.Log($"开始修理装备");
                        if (RecipeNoteUi.RecipeNoteIsOpen())
                        {
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                            Thread.Sleep(1500);
                        }
                        AlphaProject.GameData.param.TryGetValue("repair", out var rep);
                        PluginLog.Log($"修理装备配置: {rep}");
                        if (rep == "1")
                        {
                            if (AlphaProject.GameData.param.TryGetValue("type", out var t) && t == "2")
                            {
                                AlphaProject.GameData.CommonBot.MovePositions(Positions.RepairNPCA, false);
                            }
                            AlphaProject.GameData.CommonBot.NpcRepair("阿里斯特尔");
                        }
                        else if (rep == "99")
                        {
                            AlphaProject.GameData.CommonBot.Repair();
                        }
                        else if (rep == "100")
                        {
                            AlphaProject.GameData.CommonBot.NpcRepair("修理工");
                        }
                    }
                    // 魔晶石精制
                    int em = CommonUi.CanExtractMateria();
                    if (em > 2)
                    {
                        AlphaProject.GameData.CommonBot.ExtractMateria(em);
                    }
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
                            PluginLog.Warning("原材料不足...");
                            AlphaProject.AP.TM.AddTask(lackItems);
                            AlphaProject.AP.TM.RunTask();
                        }
                        else {
                            PluginLog.Error($"当前任务因缺少原材料结束...num: {lackItems.Count}");
                            break;
                        }
                    }

                    // Synth
                    if (!CraftHelper.RecipeNoteWindowOpen() && !CraftHelper.SynthesisWindowOpen())
                    {
                        if (!DalamudApi.Condition[ConditionFlag.Crafting])
                        {
                            RecipeNoteUi.OpenRecipeNote(recipe.RowId);
                            Thread.Sleep(new Random().Next(1200, 2000));
                        }
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
                        if (RecipeNoteUi.RecipeNoteIsOpen())
                        {
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                            Thread.Sleep(new Random().Next(900, 1200));
                        }
                        Thread.Sleep(new Random().Next(900, 1200));
                        // 上交收藏品和交换道具
                        //if (AlphaProject.Configuration.CraftType == 2) {
                        //    if (!AlphaProject.GameData.CommonBot.CraftUploadAndExchange())
                        //    {
                        //        PluginLog.Log($"params error... plz check");
                        //        return;
                        //    }
                        //    else
                        //    {
                        //        PluginLog.Log($"CraftUploadAndExchange End.");
                        //    }
                        //}
                        // 上交重建品和交换道具 TODO
                    }
                    Thread.Sleep(new Random().Next(900, 1200));
                }
                PluginLog.Log($"Finish: {craftX} Item: {AlphaProject.Configuration.RecipeName}, {recipe.RowId}");
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
                // TODO choose HQ
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
            ClickRecipeNote.Using(recipeWindow).QuickSynthesis();

            var quickSynthPTR = DalamudApi.GameGui.GetAddonByName("SynthesisSimpleDialog", 1);
            if (quickSynthPTR == IntPtr.Zero)
                return;

            var quickSynthWindow = (AtkUnitBase*)quickSynthPTR;
            var qsynthButton = (AtkComponentButton*)quickSynthWindow->UldManager.NodeList[3];
            if (qsynthButton != null && !qsynthButton->IsEnabled)
            {
                qsynthButton->AtkComponentBase.OwnerNode->AtkResNode.Flags ^= 1 << 5;
            }

            var checkboxNode = (AtkComponentNode*)quickSynthWindow->UldManager.NodeList[5];
            if (checkboxNode == null)
                return;
            var checkboxComponent = (AtkComponentCheckBox*)checkboxNode->Component;
            if (!checkboxComponent->IsChecked)
            {
                checkboxComponent->AtkComponentButton.Flags ^= 0x40000;
                AtkResNode* checkmarkNode = checkboxComponent->AtkComponentButton.ButtonBGNode->PrevSiblingNode;
                checkmarkNode->Color.A = (byte)(true ? 0xFF : 0x7F);
                checkmarkNode->Flags ^= 0x10;
            }

            var numericInput = (AtkComponentNode*)quickSynthWindow->UldManager.NodeList[4];
            if (numericInput == null)
                return;
            var numericComponent = (AtkComponentNumericInput*)numericInput->Component;
            numericComponent->SetValue(numericComponent->Data.Max);

            AtkResNodeFunctions.ClickButton(quickSynthWindow, qsynthButton, 1);

            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            int r = new Random().Next(50, 70);

            while (end.Subtract(start).Minutes < 10 && !AutoCraft.currentCraftFinished)
            {
                end = DateTime.Now;
                Thread.Sleep(new Random().Next(8000, 15000));
            }
        }
    }
}