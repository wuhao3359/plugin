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
            var(addon, success) = IsAddonVisible("Synthesis");
            return success;
        }

        public static bool RecipeNoteIsOpen()
        {
            var (addon, success) = IsAddonVisible("RecipeNote");
            return success;
        }

        public static bool SelectYesnoIsOpen()
        {
            var (addon, success) = IsAddonVisible("SelectYesno");
            return success;
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
            return true;
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

        private unsafe static (IntPtr Addon, bool IsVisible) IsAddonVisible(string addonName)
        {
            var addonPtr = DalamudApi.GameGui.GetAddonByName(addonName, 1);
            if (addonPtr == IntPtr.Zero)
                return (addonPtr, false);

            var addon = (AtkUnitBase*)addonPtr;
            if (!addon->IsVisible || addon->UldManager.LoadedState != AtkLoadState.Loaded)
                return (addonPtr, false);

            return (addonPtr, true);
        }

    }
}
