using System;
using WoAutoCollectionPlugin.RawInformation;
using ClickLib.Clicks;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace WoAutoCollectionPlugin.Helper
{
    public unsafe static class CraftHelper
    {
        public static List<uint> InsufficientMaterials = new();
        public static Dictionary<Recipe, bool> CraftableItems = new();
        public unsafe static InventoryManager* invManager = InventoryManager.Instance();

        public static Dictionary<uint, Recipe> FilteredList = LuminaSheets.RecipeSheet.Values
            .DistinctBy(x => x.RowId)
            .OrderBy(x => x.RecipeLevelTable.Value.ClassJobLevel)
            .ThenBy(x => x.ItemResult.Value.Name.RawString)
            .ToDictionary(x => x.RowId, x => x);

        public unsafe static bool RecipeWindowOpen()
        {
            return GenericHelper.TryGetAddonByName<AddonRecipeNote>("RecipeNote", out var addon) && addon->AtkUnitBase.IsVisible;
        }

        public unsafe static void CloseCraftingMenu()
        {
            CommandProcessorHelper.OpenCraftingMenu();
        }

        public unsafe static void OpenRecipeByID(uint recipeID, bool skipThrottle = false)
        {
            if (!GenericHelper.TryGetAddonByName<AddonRecipeNote>("RecipeNote", out var addon))
            {
                if (Throttler.Throttle(500) || skipThrottle)
                {
                    AgentRecipeNote.Instance()->OpenRecipeByRecipeIdInternal(recipeID);
                }
            }
        }

        // 原材料是否足够
        public static bool HasItemsForRecipe(uint currentProcessedItem)
        {
            if (currentProcessedItem == 0) return false;
            var recipe = FilteredList[currentProcessedItem];
            if (recipe.RowId == 0) return false;
            return CheckForIngredients(recipe, false);
        }

        // 设置物品数量
        private unsafe static void SetIngredients()
        {
            if (GenericHelper.TryGetAddonByName<AtkUnitBase>("RecipeNote", out var addon) && addon->IsVisible)
            {
                for (var i = 0; i <= 5; i++)
                {
                    try
                    {
                        var node = addon->UldManager.NodeList[23 - i]->GetAsAtkComponentNode();
                        if (node is null || !node->AtkResNode.IsVisible)
                        {
                            return;
                        }

                        var setNQ = node->Component->UldManager.NodeList[9]->GetAsAtkComponentNode()->Component->UldManager.NodeList[2]->GetAsAtkTextNode()->NodeText.ToString();
                        var setHQ = node->Component->UldManager.NodeList[6]->GetAsAtkComponentNode()->Component->UldManager.NodeList[2]->GetAsAtkTextNode()->NodeText.ToString();
                        var setNQint = Convert.ToInt32(setNQ);
                        var setHQint = Convert.ToInt32(setHQ);

                        var nqNodeText = node->Component->UldManager.NodeList[8]->GetAsAtkTextNode();
                        var hqNodeText = node->Component->UldManager.NodeList[5]->GetAsAtkTextNode();
                        var required = node->Component->UldManager.NodeList[15]->GetAsAtkTextNode();

                        int nqMaterials = Convert.ToInt32(nqNodeText->NodeText.ToString().GetNumbers());
                        int hqMaterials = Convert.ToInt32(hqNodeText->NodeText.ToString().GetNumbers());
                        int requiredMaterials = Convert.ToInt32(required->NodeText.ToString().GetNumbers());

                        if (setHQint + setNQint == requiredMaterials) continue;

                        for (int m = 0; m <= requiredMaterials && m <= nqMaterials; m++)
                        {
                            ClickRecipeNote.Using((IntPtr)addon).Material(i, false);
                        }

                        for (int m = 0; m <= requiredMaterials && m <= hqMaterials; m++)
                        {
                            ClickRecipeNote.Using((IntPtr)addon).Material(i, true);
                        }
                    }
                    catch
                    {
                        return;
                    }
                }
            }
        }

        public unsafe static bool CheckForIngredients(Recipe recipe, bool fetchFromCache = true)
        {
            if (fetchFromCache)
                if (CraftableItems.TryGetValue(recipe, out bool canCraft)) return canCraft;

            foreach (var value in recipe.UnkData5.Where(x => x.ItemIngredient != 0 && x.AmountIngredient > 0))
            {
                try
                {
                    int? invNumberNQ = invManager->GetInventoryItemCount((uint)value.ItemIngredient);
                    int? invNumberHQ = invManager->GetInventoryItemCount((uint)value.ItemIngredient, true);

                    if (value.AmountIngredient > invNumberNQ + invNumberHQ)
                    {
                        invNumberHQ = null;
                        invNumberNQ = null;

                        CraftableItems[recipe] = false;
                        return false;
                    }

                    invNumberHQ = null;
                    invNumberNQ = null;
                }
                catch
                {
                }
            }

            CraftableItems[recipe] = true;
            return true;
        }
    }
}
