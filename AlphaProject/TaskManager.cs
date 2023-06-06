using AlphaProject.Data;
using AlphaProject.Enums;
using AlphaProject.Exceptions;
using AlphaProject.RawInformation;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AlphaProject;

public class TaskManager
{

    internal List<(uint, string, byte)> TaskList = new();

    public TaskManager()
    {
        
    }

    public void RunTask() {
        for (int i = TaskList.Count - 1; i >= 0; i--)
        {
            var task = TaskList[i];
            while (AlphaProject.status != (byte)TaskState.READY) {
                PluginLog.Log("等待可执行任务状态...");
                Thread.Sleep(new Random().Next(7000, 12000));
            }

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
                    RunCraftTask(task.Item1, task.Item2);
                    break;
                default:
                    break;
            }
                
        }
        TaskList.Clear();
  
    }

    private void RunCraftTask(uint item1, string item2)
    {
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        int r = new Random().Next(50, 70);

        Task task = new(() =>
        {
            AlphaProject.status = (byte)TaskState.CRAFT;
            while (end.Subtract(start).Minutes < r && AlphaProject.status != (byte)TaskState.READY)
            {
                end = DateTime.Now;
            }
            AlphaProject.GameData.CraftBot.StopScript();
        });
        task.Start();

        AlphaProject.GameData.CraftBot.CraftScript(item2);
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
        int r = new Random().Next(50, 70);

        Task task = new(() =>
        {
            AlphaProject.status = (byte)TaskState.GATHER;
            while (end.Subtract(start).Minutes < r && AlphaProject.status != (byte)TaskState.READY)
            {
                end = DateTime.Now;
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
            (uint id, string name, byte getWay) = RecipeItems.GetItem(lckItem);
            if (id == 0) { 
                throw new CraftError("do not support this item: " + lckItem);
            }
            if (getWay == (byte)GetWay.CRAFT) 
            {
                TaskList.AddRange(AddCraftTask(id));
            }
            TaskList.Add((id, name, getWay));
        }
    }

    public List<(uint, string, byte)> AddCraftTask(uint Id)
    {
        List<(uint, string, byte)> list = new();
        Dictionary<uint, Recipe> dict = LuminaSheets.RecipeSheet.Values
            .DistinctBy(x => x.RowId)
            .OrderBy(x => x.RecipeLevelTable.Value.ClassJobLevel)
            .ThenBy(x => x.ItemResult.Value.Name.RawString)
            .Where(x => x.ItemResult.Value.RowId == Id)
            .ToDictionary(x => x.RowId, x => x);

        foreach (var kvp in dict) {
            Recipe recipe = kvp.Value;
            (uint id, string name, byte getWay) = RecipeItems.GetItem(recipe.ItemResult.Value.RowId);
            if (id == 0)
            {
                throw new CraftError("do not support this item: " + id);
            }
            if (getWay == (byte)GetWay.CRAFT)
            {
                list.AddRange(AddCraftTask(id));
            }
            list.Add((id, name, getWay));
        }
        return list;
    }
}
