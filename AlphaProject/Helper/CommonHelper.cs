using System;
using System.Linq;
using AlphaProject.Utility;
using System.Threading;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using AlphaProject.RawInformation;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.Game;
using AlphaProject.SeFunctions;
using AlphaProject.Bot;
using System.Diagnostics;
using System.Timers;
using ClickLib.Clicks;
using AlphaProject.Enums;
using System.Threading.Tasks;

namespace AlphaProject.Helper
{
    public unsafe static class CommonHelper
    {

        public unsafe static void test() { 
        
        }

        public unsafe static bool SetTarget(string targetName)
        {
            var target = DalamudApi.ObjectTable.FirstOrDefault(obj => obj.Name.TextValue.ToLowerInvariant() == targetName);
            if (target == default)
            {
                return false;
            }

            DalamudApi.TargetManager.SetTarget(target);
            Thread.Sleep(200 + new Random().Next(1000, 2000));
            KeyOperates.KeyMethod(Keys.num0_key);
            Thread.Sleep(1000 + new Random().Next(500, 1000));
            return true;
        }

        public unsafe static bool ExtractMateriaTask()
        {
            var isCrafting = DalamudApi.Condition[ConditionFlag.Crafting];
            var preparing = DalamudApi.Condition[ConditionFlag.PreparingToCraft];

            if (Spiritbond.IsSpiritbondReadyAny())
            {
                if (GenericHelper.TryGetAddonByName<AtkUnitBase>("RecipeNote", out var addon) && addon->IsVisible && DalamudApi.Condition[ConditionFlag.Crafting])
                {
                    CraftHelper.CloseCraftingMenu();
                }
                if (!Spiritbond.IsMateriaMenuOpen() && !isCrafting && !preparing)
                {
                    Spiritbond.OpenMateriaMenu();
                    return false;
                }
                if (Spiritbond.IsMateriaMenuOpen() && !isCrafting && !preparing)
                {
                    Spiritbond.ExtractFirstMateria();
                    return false;
                }
                return false;
            }
            else
            {
                if (Spiritbond.IsMateriaMenuOpen())
                {
                    Spiritbond.CloseMateriaMenu();
                    return false;
                }
            }
            return true;
        }

        internal unsafe static bool RepairTask()
        {
            if (GetMinEquippedPercent() <= 20) {
                PluginLog.Log("Repair...");
                if (Throttler.Throttle(2000)) {
                    if (CraftHelper.RecipeNoteWindowOpen())
                    {
                        CraftHelper.CloseCraftingMenu();
                    }

                    // TODO 多配置修理
                    Teleporter.Teleport(Positions.ShopTp);
                    KeyOperates.MovePositions(Positions.RepairNPC, false);
                    CommonBot.NpcRepair("阿塔帕");
                    return true;
                }
                return false;
            }
            return true;
        }

        internal static int GetMinEquippedPercent()
        {
            var ret = int.MaxValue;
            var equipment = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems);
            for (var i = 0; i < equipment->Size; i++)
            {
                var item = equipment->GetInventorySlot(i);
                if (item->Condition < ret) ret = item->Condition;
            }
            return (ret / 300);
        }

        internal static void ShutdownGame(object sender, ElapsedEventArgs e)
        {
            PluginLog.LogError("too long for running, shutdown");
            Tasks.TaskRun = false;

            Task task = new(() =>
            {
                while (Tasks.Status != (byte)TaskState.READY)
                {
                    PluginLog.Log("wait for curennt task...");
                    Thread.Sleep(5000);
                }

                // 拉扎罕
                Teleporter.Teleport(183);
                CommandProcessorHelper.ExecuteThrottled("/shutdown");
                var addon = (AtkUnitBase*)DalamudApi.GameGui.GetAddonByName("SelectYesno", 1);
                if (addon != null)
                {
                    ClickSelectYesNo.Using((nint)addon).Yes();
                }
            });
            task.Start();
        }
    }
}
