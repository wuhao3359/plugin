using FFXIVClientStructs.FFXIV.Client.UI;
using System;
using AlphaProject.Manager;

namespace AlphaProject.Helper;

public unsafe static class CommandProcessorHelper
{

    private static CommandManager CommandManager = new(DalamudApi.GameGui, DalamudApi.SigScanner);

    static long nextCommandAt = 0;
    internal static bool ExecuteThrottled(string command)
    {
        if (Environment.TickCount64 > nextCommandAt)
        {
            nextCommandAt = Environment.TickCount64 + 500;
            CommandManager.Execute(command);
            return true;
        }
        else
        {
            return false;
        }
    }

    public unsafe static void DoGearChange(string set)
    {
        CommandManager.Execute($"/gearset change \"{set}\"");
    }

    public unsafe static void OpenCraftingMenu()
    {
        if (!GenericHelper.TryGetAddonByName<AddonRecipeNote>("RecipeNote", out var addon))
        {
            if (Throttler.Throttle(1000))
            {
                ExecuteThrottled("/clog");
            }
        }
    }
}
