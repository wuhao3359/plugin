using ClickLib;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Linq;
using WoAutoCollectionPlugin.Exceptions;
using Sheets = Lumina.Excel.GeneratedSheets;

namespace WoAutoCollectionPlugin.Ui
{
    public static class RecipeNoteUi
    {
        public static bool SynthesisIsOpen()
        {
            var(addon, success) = CommonUi.IsAddonVisible("Synthesis");
            return success;
        }

        public static bool RecipeNoteIsOpen()
        {
            var (addon, success) = CommonUi.IsAddonVisible("RecipeNote");
            return success;
        }

        public static bool SelectYesnoIsOpen()
        {
            var (addon, success) = CommonUi.IsAddonVisible("SelectYesno");
            return success;
        }

        public static unsafe bool QuickSynthesizeButton()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("RecipeNote", 1);
            if (ptr != IntPtr.Zero)
            {
                return Click.TrySendClick("quick_synthesis");
            }
            else
            {
                return false;
            }
        }

        public static unsafe bool SynthesizeButton() {
            var ptr = DalamudApi.GameGui.GetAddonByName("RecipeNote", 1);
            if (ptr != IntPtr.Zero)
            {
                return Click.TrySendClick("synthesize");
            }
            else {
                return false;
            }
        }

        public static unsafe bool Material1HqButton()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("RecipeNote", 1);
            if (ptr != IntPtr.Zero)
            {
                if (WoAutoCollectionPlugin.GameData.param.TryGetValue("hq", out var hq))
                {
                    string[] hqs = hq.Split(",");
                    for (int k = 0; k < hqs.Length; k++)
                    {
                        string[] config = hqs[k].Split("-");
                        string index = config[0];
                        string quantity = config[1];
                        if (index == "1")
                        {
                            for (int kk = 0; kk < int.Parse(quantity); kk++)
                            {
                                Click.TrySendClick("synthesis_material1_hq");
                            }
                        } 
                        else if (index == "2")
                        {
                            for (int kk = 0; kk < int.Parse(quantity); kk++)
                            {
                                Click.TrySendClick("synthesis_material2_hq");
                            }
                        }
                        else if (index == "3")
                        {
                            for (int kk = 0; kk < int.Parse(quantity); kk++)
                            {
                                Click.TrySendClick("synthesis_material3_hq");
                            }
                        }
                        else if (index == "4")
                        {
                            for (int kk = 0; kk < int.Parse(quantity); kk++)
                            {
                                Click.TrySendClick("synthesis_material4_hq");
                            }
                        }
                        else if (index == "5")
                        {
                            for (int kk = 0; kk < int.Parse(quantity); kk++)
                            {
                                Click.TrySendClick("synthesis_material5_hq");
                            }
                        }
                        else if (index == "6")
                        {
                            for (int kk = 0; kk < int.Parse(quantity); kk++)
                            {
                                Click.TrySendClick("synthesis_material6_hq");
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public static unsafe void OpenRecipeNote(uint recipeID)
        {
            var agent = AgentRecipeNote.Instance();

            if (agent == null)
                throw new MacroCommandError("AgentRecipeNote not found");

            agent->OpenRecipeByRecipeId(recipeID);
        }

        public static uint SearchRecipeId(string recipeName)
        {
            var sheet = DalamudApi.DataManager.GetExcelSheet<Sheets.Recipe>()!;
            var recipes = sheet.Where(r => r.ItemResult.Value?.Name.ToString().ToLowerInvariant() == recipeName).ToList();

            switch (recipes.Count)
            {
                case 0: throw new MacroCommandError("Recipe not found");
                case 1: return recipes.First().RowId;
                default:
                    var jobId = DalamudApi.ClientState.LocalPlayer?.ClassJob.Id;

                    var recipe = recipes.Where(r => GetClassJobID(r) == jobId).FirstOrDefault();
                    if (recipe == default)
                        return recipes.First().RowId;

                    return recipe.RowId;
            }
        }

        private static uint GetClassJobID(Sheets.Recipe recipe)
        {
            // Name           CraftType ClassJob
            // Carpenter      0         8
            // Blacksmith     1         9
            // Armorer        2         10
            // Goldsmith      3         11
            // Leatherworker  4         12
            // Weaver         5         13
            // Alchemist      6         14
            // Culinarian     7         15
            return recipe.CraftType.Value!.RowId + 8;
        }

        private unsafe static AddonSynthesis* GetSynthesisAddon()
        {
            var ptr = DalamudApi.GameGui.GetAddonByName("Synthesis", 1);
            if (ptr == IntPtr.Zero)
                throw new MacroCommandError("Could not find Synthesis addon");

            return (AddonSynthesis*)ptr;
        }

        public unsafe static String GetItemName()
        {
            try
            {
                var addon = GetSynthesisAddon();
                return GetNodeTextAsString(addon->ItemName, "Could not get ItemName in the Synthesis addon");
            }
            catch (Exception ex)
            {
                PluginLog.Error($"{ex}");
                return "";
            }
        }

        private unsafe static string GetNodeTextAsString(AtkTextNode* node, string error)
        {
            try
            {
                if (node == null)
                    throw new NullReferenceException("TextNode is null");

                var text = node->NodeText.ToString();
                return text;
            }
            catch (Exception ex)
            {
                throw new MacroCommandError(error, ex);
            }
        }

        private unsafe static int GetNodeTextAsInt(AtkTextNode* node, string error)
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


    }
}
