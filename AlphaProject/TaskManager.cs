using AlphaProject.Data;
using AlphaProject.Enums;
using AlphaProject.Exceptions;
using AlphaProject.Helper;
using AlphaProject.RawInformation;
using AlphaProject.Ui;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AlphaProject;

public class TaskManager
{

    internal List<(uint, string, byte, uint)> TaskList = new();

    public TaskManager()
    {
        
    }

    public void RunTask() {
        if (CraftHelper.SynthesisWindowOpen())
        {
            CraftHelper.CloseCraftingMenu();
        }

        AlphaProject.status = (byte)TaskState.READY;
        PluginLog.Log($"总共任务: {TaskList.Count}");

        for (int i = 0; i <= TaskList.Count - 1; i++) {
            var task = TaskList[i];
            PluginLog.Log($"总共{i}: {task.Item2}");
        }

        for (int i = 0; i <= TaskList.Count - 1; i++)
        {
            var task = TaskList[i];
            while (AlphaProject.status != (byte)TaskState.READY) {
                PluginLog.Log($"等待可执行任务状态... {AlphaProject.status}");
                Thread.Sleep(new Random().Next(7000, 12000));
            }

            PluginLog.Log($"任务: {task.Item1} {task.Item2} {task.Item3}");
            switch (task.Item3)
            {
                case (byte)GetWay.GATHER:
                    RunGatherTask(task.Item1, task.Item2);
                    break;
                case (byte)GetWay.WHITE:
                    RunWhiteTask(task.Item1, task.Item2);
                    break;
                case (byte)GetWay.PURPLE:
                    RunPurpleTask(task.Item1, task.Item2);
                    break;
                case (byte)GetWay.AETHERSAND:
                    RunAethersandTask(task.Item1, task.Item2);
                    break;
                case (byte)GetWay.MARKET:
                    RunMarketTask(task.Item1, task.Item2);
                    break;
                case (byte)GetWay.CRAFT:
                    RunCraftTask(task.Item1, task.Item2, task.Item4);
                    break;
                default:
                    break;
            }

            Thread.Sleep(5000);

            if (CraftHelper.RecipeNoteWindowOpen())
            {
                CraftHelper.CloseCraftingMenu();
            }
        }
        TaskList.Clear();
  
    }

    private void RunCraftTask(uint item1, string item2, uint item4)
    {
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        int r = new Random().Next(50, 60);

        Task task = new(() =>
        {
            AlphaProject.status = (byte)TaskState.CRAFT;
            while (end.Subtract(start).Minutes < r && AlphaProject.status == (byte)TaskState.CRAFT)
            {
                end = DateTime.Now;
                PluginLog.Log($"CRAFT 已经执行时间: {end.Subtract(start).Minutes}, 总共: {r}");
                Thread.Sleep(10000);
            }
            AlphaProject.GameData.CraftBot.StopScript();
        });
        task.Start();

        AlphaProject.GameData.CraftBot.CraftScript(item2, item4);
        AlphaProject.status = (byte)TaskState.READY;
    }

    private void RunMarketTask(uint item1, string item2)
    {
        throw new NotImplementedException();
    }

    private void RunAethersandTask(uint item1, string item2)
    {
        throw new NotImplementedException();
    }

    private void RunPurpleTask(uint item1, string item2)
    {
        throw new NotImplementedException();
    }

    private void RunWhiteTask(uint item1, string item2)
    {
        throw new NotImplementedException();
    }

    private void RunGatherTask(uint item1, string item2)
    {
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        int r = new Random().Next(50, 60);

        Task task = new(() =>
        {
            AlphaProject.status = (byte)TaskState.GATHER;
            while (end.Subtract(start).Minutes < r && AlphaProject.status == (byte)TaskState.GATHER)
            {
                end = DateTime.Now;
                PluginLog.Log($"GATHER 已经执行时间: {end.Subtract(start).Minutes}, 总共: {r}");
                Thread.Sleep(10000);
            }
            AlphaProject.GameData.GatherBot.StopScript();
        });
        task.Start();

        AlphaProject.GameData.GatherBot.GatherByName(item2);
        AlphaProject.status = (byte)TaskState.READY;
    }

    public void AddTask(List<uint> lackItems)
    {
        TaskList = new();
        foreach (uint lckItem in lackItems) 
        {
            (uint id, string name, byte getWay, uint job) = RecipeItems.GetItem(lckItem);
            if (id == 0) { 
                throw new CraftError("do not support this item: " + lckItem);
            }
            if (getWay == (byte)GetWay.CRAFT) 
            {
                TaskList.AddRange(AddCraftTask(id));
            }
            TaskList.Add((id, name, getWay, job));
        }
    }

    public List<(uint, string, byte, uint job)> AddCraftTask(uint Id)
    {
        List<(uint, string, byte, uint)> list = new();
        Dictionary<uint, Recipe> dict = LuminaSheets.RecipeSheet.Values
            .DistinctBy(x => x.RowId)
            .OrderBy(x => x.RecipeLevelTable.Value.ClassJobLevel)
            .ThenBy(x => x.ItemResult.Value.Name.RawString)
            .Where(x => x.ItemResult.Value.RowId == Id)
            .ToDictionary(x => x.RowId, x => x);

        foreach (var kvp in dict) {
            PluginLog.Log($"分解配方: {kvp.Value.ItemResult.Value.Name}");
            Recipe recipe = kvp.Value;


            if (!CraftHelper.CheckForRecipeIngredients(recipe.RowId, out List<uint> lackItems, false) && lackItems.Count > 0)
            {
                foreach (uint item in lackItems) {
                    (uint id, string name, byte getWay, uint job) = RecipeItems.GetItem(item);
                    PluginLog.Log($"材料: {name} {getWay} {job}");
                    if (id == 0)
                    {
                        throw new CraftError("do not support this item craft: " + id);
                    }
                    if (getWay == (byte)GetWay.CRAFT)
                    {
                        list.AddRange(AddCraftTask(id));
                    }
                    list.Add((id, name, getWay, job));
                }
            }
        }
        return list;
    }
}
