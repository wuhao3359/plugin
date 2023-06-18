using System;
using AlphaProject.RawInformation;
using ClickLib.Clicks;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI;
using Dalamud.Logging;
using Newtonsoft.Json.Linq;
using AlphaProject.Data;
using AlphaProject.Managers;
using AlphaProject.Ui;
using AlphaProject.Utility;
using System.Threading;
using AlphaProject.SeFunctions;

namespace AlphaProject.Helper
{
    public unsafe static class TicketHelper
    {
        public static bool closed { get; set; } = false;

        public static int white { get; set; } = 0;
        public static int purple { get; set; } = 0;

        public unsafe static void test() { 
        
        }

        public unsafe static void GetWhiteAndPurple()
        {
            try {
                var collectabesShopPTR = DalamudApi.GameGui.GetAddonByName("CollectablesShop", 1);
                var collectabesShopWindow = (AtkUnitBase*)collectabesShopPTR;
                if (collectabesShopWindow == null)
                    return;

                // 白票
                var whiteComponent = (AtkComponentNode*)collectabesShopWindow->UldManager.NodeList[38];
                if (whiteComponent == null)
                    return;
                var whiteNode = whiteComponent->Component->UldManager.NodeList[1]->GetAsAtkTextNode();
                string whiteText = whiteNode->NodeText.ToString().Split("/")[0].Replace(",", "");
                white = int.Parse(whiteText);

                // 紫票
                var purpleComponent = (AtkComponentNode*)collectabesShopWindow->UldManager.NodeList[39];
                if (purpleComponent == null)
                    return;
                var purpleNode = purpleComponent->Component->UldManager.NodeList[1]->GetAsAtkTextNode();
                string purpleText = purpleNode->NodeText.ToString().Split("/")[0].Replace(",", "");
                purple = int.Parse(purpleText);
            } catch (Exception e) {
                PluginLog.Error($"获取票据错误...");
                white = 0;
                purple = 0;
            }
            return;
        }

        public unsafe static bool CraftUploadAndExchange(string itemName, string exchangeItem)
        {
            // 位置
            Teleporter.Teleport(Positions.ShopATp);
            AlphaProject.GameData.KeyOperates.MovePositions(Positions.ExchangeAndUploadANPC, false);
            // 提交物品
            (uint Category, uint Sub, uint ItemId) = RecipeItems.UploadApply(itemName);
            for (int i = 0; i <= 3 && BagManager.GetInventoryItemCountById(ItemId) > 0; i++) {
                CraftUpload(Category, Sub, ItemId);
                CraftExchange(exchangeItem);
            }
            return true;
        }

        // 交收藏品
        public unsafe static void CraftUpload(uint Category, uint Sub, uint ItemId)
        {
            PluginLog.Log($"CraftUploading");
            if (Category == 0 && Sub == 0)
            {
                return;
            }
            CommonHelper.SetTarget("收藏品交易员");
            if (CommonUi.AddonCollectablesShopIsOpen())
            {
                for (int i = 1; i < Category; i++)
                {
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                }

                for (int i = 0; i < Sub; i++)
                {
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                }

                int count = BagManager.GetInventoryItemCountById(ItemId);
                AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                Thread.Sleep(900 + new Random().Next(100, 200));
                int error = 0;
                while (!RecipeNoteUi.SelectYesnoIsOpen() && count > 0)
                {
                    GetWhiteAndPurple();
                    if (white >= 3800 || purple >= 3800 || closed) {
                        break;
                    }
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    Thread.Sleep(100 + new Random().Next(100, 200));
                    if (RecipeNoteUi.SelectYesnoIsOpen())
                    {
                        break;
                    }
                    int after = BagManager.GetInventoryItemCountById(ItemId);
                    if (count == after) {
                        error++;
                        if (error >= 2) {
                            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                            break;
                        }
                    }
                    count = BagManager.GetInventoryItemCountById(ItemId);
                }

                if (RecipeNoteUi.SelectYesnoIsOpen())
                {
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                    Thread.Sleep(600 + new Random().Next(100, 200));
                }
                while (CommonUi.AddonCollectablesShopIsOpen())
                {
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                    Thread.Sleep(600 + new Random().Next(100, 200));
                }
                Thread.Sleep(3000);
            }
            return;
        }

        // 交换道具
        public unsafe static void CraftExchange(string exchangeItem)
        {
            PluginLog.Log($"CraftExchanging");

            if (white <= 200 && purple <= 200) {
                return;
            }

            (int Category, int Sub) = RecipeItems.ExchangeApply(exchangeItem);
            if (Category == 0)
            {
                PluginLog.Error($"not support Category: {Category}");
            }
            CommonHelper.SetTarget("工票交易员");
            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
            AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
            Thread.Sleep(2000);

            if (closed)
            {
                PluginLog.Log($"exchange stopping");
                return;
            }

            if (CommonUi.AddonInclusionShopIsOpen())
            {
                // 1 
                if (Category == 1)
                {
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                }
                // 2-大地白票 随机采集魔晶石
                else if (Category == 2)
                {
                    Random rd = new();
                    int r = rd.Next(19);
                    if (r <= 0)
                    {
                        r = 0;
                    }
                    if (r >= 18)
                    {
                        r = 18;
                    }
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    for (int k = 0; k < r; k++)
                    {
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    }
                }
                else if (Category == 72)
                {
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                }
                else if (Category >= 80 && Category <= 99)
                {
                    int r = Category - 80;
                    if (r <= 0)
                    {
                        r = 0;
                    }
                    if (r >= 18)
                    {
                        r = 18;
                    }
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    for (int k = 0; k < r; k++)
                    {
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    }
                }
                else if (Category == 101)
                {
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                }
                else if (Category == 102)
                {
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                }
                else if (Category >= 1000 && Category <= 1100)
                {
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    for (int k = 0; k < Sub; k++)
                    {
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    }
                }

                else if (Category == 100007)
                {
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num8_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                    for (int k = 0; k < Sub; k++)
                    {
                        AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num2_key);
                    }
                }

                if (closed)
                {
                    PluginLog.Log($"exchange stopping");
                    return;
                }

                AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num6_key);
                AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num9_key);
                AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num4_key);
                AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);
                AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num4_key);
                AlphaProject.GameData.KeyOperates.KeyMethod(Keys.num0_key);

                while (CommonUi.AddonInclusionShopIsOpen())
                {
                    AlphaProject.GameData.KeyOperates.KeyMethod(Keys.esc_key);
                    Thread.Sleep(1000);
                }
            }
            return;
        }
    }
}
