using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Logging;
using Dalamud.Plugin;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using WoAutoCollectionPlugin.Bot;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.UseAction;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin
{
    public sealed class WoAutoCollectionPlugin : IDalamudPlugin
    {
        public string Name => "WoAutoCollectionPlugin";

        private const string collect = "/collect";

        private const string fish = "/fish";

        private const string collectionfish = "/collectionfish";

        private const string gather = "/gather";

        private const string woTest = "/woTest";

        private const string actionTest = "/actionTest";

        private const string close = "/close";

        private const string craft = "/craft";

        public WoAutoCollectionPlugin Plugin { get; private set; }

        public Configuration Configuration { get; private set; }

        private PluginUI PluginUi { get; init; }
        public GameData GameData { get; init; }

        private FishBot? FishBot;
        private CollectionFishBot? CollectionFishBot;
        private GatherBot? GatherBot;
        private CraftBot? CraftBot;

        public bool isRunning = true;

        public bool taskRunning = false;

        public WoAutoCollectionPlugin(DalamudPluginInterface pluginInterface)
        {
            Plugin = this;
            DalamudApi.Initialize(pluginInterface);

            Configuration = DalamudApi.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize();

            //Commands.InitializeCommands();
            //Configuration.Initialize(DalamudApi.PluginInterface);
            //ClickLib.Click.Initialize();

            try
            {
                //Game.Initialize();
                DalamudApi.CommandManager.AddHandler(collect, new CommandInfo(OnCommand)
                {
                    HelpMessage = "当前坐标信息"
                });

                DalamudApi.CommandManager.AddHandler(fish, new CommandInfo(OnFishCommand)
                {
                    HelpMessage = "fish {param}"
                });

                DalamudApi.CommandManager.AddHandler(collectionfish, new CommandInfo(OnCollectionFishCommand)
                {
                    HelpMessage = "collectionfish {param}"
                });

                DalamudApi.CommandManager.AddHandler(gather, new CommandInfo(OnGatherCommand)
                {
                    HelpMessage = "gather {param}"
                });

                DalamudApi.CommandManager.AddHandler(woTest, new CommandInfo(OnWoTestCommand)
                {
                    HelpMessage = "wotest"
                });

                DalamudApi.CommandManager.AddHandler(actionTest, new CommandInfo(OnActionTestCommand)
                {
                    HelpMessage = "actionTest"
                });

                DalamudApi.CommandManager.AddHandler(close, new CommandInfo(OnCloseTestCommand)
                {
                    HelpMessage = "close"
                });

                DalamudApi.CommandManager.AddHandler(craft, new CommandInfo(OnCraftCommand)
                {
                    HelpMessage = "Craft"
                });

                GameData = new GameData(DalamudApi.DataManager);
                FishBot = new FishBot(GameData);
                CollectionFishBot = new CollectionFishBot(GameData);
                GatherBot = new GatherBot(GameData);
                CraftBot = new CraftBot(GameData);

                //DalamudApi.PluginInterface.UiBuilder.Draw += DrawUI;
                //DalamudApi.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            }
            catch (Exception e)
            {
                PluginLog.Error($"Failed loading WoAutoCollectionPlugin\n{e}");
            }
        }

        public void Dispose()
        {
            // PluginUi.Dispose();
            DalamudApi.CommandManager.RemoveHandler(collect);
            DalamudApi.CommandManager.RemoveHandler(fish);
            DalamudApi.CommandManager.RemoveHandler(collectionfish);
            DalamudApi.CommandManager.RemoveHandler(gather);
            DalamudApi.CommandManager.RemoveHandler(woTest);
            DalamudApi.CommandManager.RemoveHandler(actionTest);
            DalamudApi.CommandManager.RemoveHandler(close);
            DalamudApi.CommandManager.RemoveHandler(craft);
            // Game.DisAble();
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            // PluginUi.Visible = false;
            if (DalamudApi.ClientState != null && DalamudApi.ClientState.LocalPlayer != null)
            {
                Vector3 playerPosition = DalamudApi.ClientState.LocalPlayer.Position;
                try
                {
                    ushort SizeFactor = GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
                    float x = Maths.GetCoordinate(playerPosition.X, SizeFactor);
                    float y = Maths.GetCoordinate(playerPosition.Y, SizeFactor);
                    float z = Maths.GetCoordinate(playerPosition.Z, SizeFactor);
                    PluginLog.Log($"{DalamudApi.ClientState.TerritoryType}  {x}   {y}   {z}");
                }
                catch (Exception)
                {
                    PluginLog.Log($"error");
                }
            }
        }

        // 去空岛指定地方钓鱼
        private void OnFishCommand(string command, string args)
        {
            string[] str = args.Split(' ');
            int area = int.Parse(str[0]);
            PluginLog.Log($"fish: {args} length: {str.Length}");

            if (area <= 0)
            {
                FishBot.StopYFishScript();
                isRunning = false;
                taskRunning = false;
                return;
            }

            if (taskRunning)
            {
                PluginLog.Log($"stop first");
                return;
            }

            taskRunning = true;
            isRunning = true;
            Task task = new(() =>
            {
                PluginLog.Log($"start task...");
                DalamudApi.Framework.Update += FishBot.OnYFishUpdate;
                while (isRunning)
                {
                    try {
                        //if (DalamudApi.Condition[ConditionFlag.OccupiedInQuestEvent]
                        //|| DalamudApi.Condition[ConditionFlag.WaitingForDutyFinder]) {
                        //    PluginLog.Log($"当前状态无法进行任务, skip...");
                        //    cycle++;
                        //    Thread.Sleep(3000);
                        //    continue;
                        //}
                        if (GameData.TerritoryType.TryGetValue(DalamudApi.ClientState.TerritoryType, out var territoryType))
                        {
                            PluginLog.Log($"当前位置: {DalamudApi.ClientState.TerritoryType} {territoryType.PlaceName.Value.Name}");
                        }
                        if (DalamudApi.ClientState.TerritoryType - Position.TianQiongJieTerritoryType == 0)
                        {
                            FishBot.RunIntoYunGuanScript();
                        }

                        if (DalamudApi.ClientState.TerritoryType - Position.YunGuanTerritoryType == 0)
                        {
                            FishBot.RunYFishScript(args);
                        }
                        else
                        {
                            PluginLog.Log($"当前位置不在空岛, {DalamudApi.ClientState.TerritoryType} ,skip...");
                            Thread.Sleep(2000);
                        }
                    } catch (Exception e) {
                        PluginLog.Error($"error!!!\n{e}");
                    }

                    PluginLog.Log($"准备开始下一轮...");
                    Thread.Sleep(3000);
                }
                PluginLog.Log($"end");
                taskRunning = false;
                DalamudApi.Framework.Update -= FishBot.OnYFishUpdate;

                FishBot.Init();
            });
            task.Start();
        }

        private void OnCollectionFishCommand(string command, string args) {
            string[] str = args.Split(' ');
            int area = int.Parse(str[0]);
            PluginLog.Log($"collectionfish: {args}");

            if (area <= 0)
            {
                CollectionFishBot.StopCollectionFishScript();
                isRunning = false;
                taskRunning = false;
                return;
            }

            if (taskRunning)
            {
                PluginLog.Log($"stop first");
                return;
            }

            taskRunning = true;
            isRunning = true;
            Task task = new(() =>
            {
                DalamudApi.Framework.Update += CollectionFishBot.OnCollectionFishUpdate;
                while (isRunning)
                {
                    try
                    {
                        CollectionFishBot.RunCollectionFishScript(args);
                    }
                    catch (Exception e) {
                        PluginLog.Error($"error!!!\n{e}");
                    }

                    isRunning = false;
                    PluginLog.Log($"准备开始下一轮...");
                    Thread.Sleep(3000);
                }
                PluginLog.Log($"end");
                DalamudApi.Framework.Update -= CollectionFishBot.OnCollectionFishUpdate;
                taskRunning = false;

                FishBot.Init();
            });
            task.Start();
        }

        private void OnGatherCommand(string command, string args)
        {
            string[] str = args.Split(' ');
            int area = int.Parse(str[0]);
            PluginLog.Log($"gather: {area}");

            if (area <= 0)
            {
                isRunning = false;
                GatherBot.StopNormalScript();
                taskRunning = false;
                return;
            }

            if (taskRunning)
            {
                PluginLog.Log($"stop first");
                return;
            }

            taskRunning = true;
            isRunning = true;
            Task task = new(() =>
            {
                int n = 0;
                while (isRunning & n < 1000)
                {
                    try
                    {
                        GatherBot.RunNormalScript(area);
                    } catch (Exception e) {
                        PluginLog.Error($"error!!!\n{e}");
                    }
                   
                    n++;
                    PluginLog.Log($"{n} 次结束");
                }
                PluginLog.Log($"all end");
            });
            task.Start();
        }

        // 测试专用
        private void OnWoTestCommand(string command, string args)
        {
            // 技能 hook 测试
            //DalamudApi.CommandManager.ProcessCommand($"/gearset change \"{set}\"");

            // 人物状态测试
            //DalamudApi.Condition.ConditionChange += ChangeCondition;

            string recipeName = "上级以太药";
            //PluginLog.Log($"{recipeName}");
            uint recipeId = RecipeNoteUi.SearchRecipeId(recipeName);
            //PluginLog.Log($"{recipeId}");
            RecipeNoteUi.OpenRecipeNote(recipeId);

        }

        private void OnActionTestCommand(string command, string args)
        {
            // 技能 测试
            Game.Initialize();
            Game.Test();
            Game.DisAble();
        }

        private void OnCloseTestCommand(string command, string args)
        {
            taskRunning = false;
        }

        // 生产 
        // {param0}-宏按键
        // {param1}-周期 
        // {param2}-兑换物品(id)
        private void OnCraftCommand(string command, string args)
        {
            string[] str = args.Split(' ');
            PluginLog.Log($"craft: {args} length: {args.Length}");

            if (args.Length <= 1)
            {
                PluginLog.Log($"stop");
                CraftBot.StopCraftScript();
                isRunning = false;
                taskRunning = false;
                return;
            }

            if (taskRunning)
            {
                PluginLog.Log($"stop first");
                return;
            }

            isRunning = true;
            taskRunning = true;
            Task task = new(() =>
            {
                while (isRunning)
                {
                    try
                    {
                        CraftBot.RunCraftScript(args);
                    } catch (Exception e) {
                        PluginLog.Error($"error!!!\n{e}");
                    }

                    PluginLog.Log($"end");
                    //isRunning = false;
                }
                PluginLog.Log($"all end");
                isRunning = false;
                taskRunning = false;
            });
            task.Start();
        }

        private void DrawUI()
        {
            PluginUi.Draw();
        }

        private void DrawConfigUI()
        {
            PluginUi.SettingsVisible = true;
        }
    }
}
