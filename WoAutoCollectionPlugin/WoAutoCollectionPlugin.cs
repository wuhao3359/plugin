﻿using ClickLib;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Command;
using Dalamud.Game.Network;
using Dalamud.Game.Network.Structures;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using WoAutoCollectionPlugin.Bot;
using WoAutoCollectionPlugin.Craft;
using WoAutoCollectionPlugin.SeFunctions;
using WoAutoCollectionPlugin.Spearfishing;
using WoAutoCollectionPlugin.Time;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.UseAction;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin
{
    public sealed class WoAutoCollectionPlugin : IDalamudPlugin
    {
        public string Name => "EasyGame";
        private const string collect = "/collect";
        private const string fish = "/fish";
        private const string hfish = "/hfish";
        private const string collectionfish = "/collectionfish";
        private const string gather = "/gather";
        private const string ygather = "/ygather";
        private const string woTest = "/woTest";
        private const string craft = "/craft";
        private const string daily = "/daily";
        private const string market = "/market";

        public Configuration Configuration { get; private set; }
        public static SeTime Time { get; private set; }
        private PluginUI PluginUi { get; init; }
        public static GameData GameData { get; private set; }
        public MarketEventHandler MarketEventHandler { get; private set; }

        public AutoCraft autoCraft { get; private set; }

        public static bool newRequest;
        public static GetFilePointer getFilePtr;
        public static List<MarketBoardCurrentOfferings> _cache = new();
        public static Lumina.Excel.ExcelSheet<Item> items;
        public static bool taskRunning = false;

        public static string beforePrice = "";
        public static bool getPriceSucceed = false;

        public static string status = "";

        public static bool Debug = true;

        internal readonly WindowSystem WindowSystem;

        public WoAutoCollectionPlugin(DalamudPluginInterface pluginInterface, GameNetwork network)
        {
            DalamudApi.Initialize(pluginInterface);

            // 可视化ui
            //Configuration = DalamudApi.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            //Configuration.Initialize();

            //Commands.InitializeCommands();
            //Configuration.Initialize(DalamudApi.PluginInterface);
            Click.Initialize();
            autoCraft = new AutoCraft();
            AutoCraftBot.Init();
            newRequest = false;
            //MarketEventHandler = new MarketEventHandler();
            //DalamudApi.GameNetwork.NetworkMessage += MarketEventHandler.OnNetworkEvent;
            DalamudApi.ClientState.Login += OnLoginEvent;
            DalamudApi.ClientState.Logout += OnLogoutEvent;

            try
            {
                DalamudApi.CommandManager.AddHandler(collect, new CommandInfo(OnCommand)
                {
                    HelpMessage = "当前坐标信息"
                });
                DalamudApi.CommandManager.AddHandler(fish, new CommandInfo(OnFishCommand)
                {
                    HelpMessage = "fish {param}"
                });
                DalamudApi.CommandManager.AddHandler(hfish, new CommandInfo(OnHFishCommand)
                {
                    HelpMessage = "hfish"
                });
                DalamudApi.CommandManager.AddHandler(collectionfish, new CommandInfo(OnCollectionFishCommand)
                {
                    HelpMessage = "collectionfish {param}"
                });
                DalamudApi.CommandManager.AddHandler(gather, new CommandInfo(OnGatherCommand)
                {
                    HelpMessage = "gather {param}"
                });
                DalamudApi.CommandManager.AddHandler(ygather, new CommandInfo(OnYGatherCommand)
                {
                    HelpMessage = "ygather {param}"
                });
                DalamudApi.CommandManager.AddHandler(woTest, new CommandInfo(OnWoTestCommand)
                {
                    HelpMessage = "woTest"
                });
                DalamudApi.CommandManager.AddHandler(craft, new CommandInfo(OnCraftCommand)
                {
                    HelpMessage = "Craft"
                });
                DalamudApi.CommandManager.AddHandler(daily, new CommandInfo(OnDailyCommand)
                {
                    HelpMessage = "Daily"
                });
                DalamudApi.CommandManager.AddHandler(market, new CommandInfo(OnMarketCommand)
                {
                    HelpMessage = "Market"
                });

                GameData = new GameData(DalamudApi.DataManager);
                items = GameData.DataManager.GetExcelSheet<Item>();
                Time = new SeTime();

                WindowSystem = new WindowSystem(Name);
                WindowSystem.AddWindow(new SpearfishingHelper(GameData));
                DalamudApi.PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

                //DalamudApi.PluginInterface.UiBuilder.Draw += DrawUI;
                //DalamudApi.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            }
            catch (Exception e)
            {
                PluginLog.Error($"Failed loading WoAutoCollectionPlugin\n{e}");
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetFilePointer(byte index);

        public void Dispose()
        {
            // PluginUi.Dispose();
            DalamudApi.CommandManager.RemoveHandler(collect);
            DalamudApi.CommandManager.RemoveHandler(fish);
            DalamudApi.CommandManager.RemoveHandler(hfish);
            DalamudApi.CommandManager.RemoveHandler(collectionfish);
            DalamudApi.CommandManager.RemoveHandler(gather);
            DalamudApi.CommandManager.RemoveHandler(ygather);
            DalamudApi.CommandManager.RemoveHandler(woTest);
            DalamudApi.CommandManager.RemoveHandler(craft);
            DalamudApi.CommandManager.RemoveHandler(daily);
            DalamudApi.CommandManager.RemoveHandler(market);
            //MarketEventHandler.Dispose();
            //DalamudApi.GameNetwork.NetworkMessage -= MarketEventHandler.OnNetworkEvent;
            DalamudApi.ClientState.Login -= OnLoginEvent;
            DalamudApi.ClientState.Logout -= OnLogoutEvent;
            //MarketCommons.Dispose();

            AutoCraftBot.Dispose();
            if (WindowSystem != null)
                DalamudApi.PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
            WindowSystem?.RemoveAllWindows();
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
                    //Vector3 position = WoAutoCollectionPlugin.GameData.KeyOperates.GetUserPosition(SizeFactor);
                    //GameObject go = Util.CurrentFishCanGather(position, SizeFactor);
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
            PluginLog.Log($"fish: {args}");
            if (args.Length == 0)
            {
                GameData.FishBot.StopScript();
                taskRunning = false;
                return;
            }

            if (taskRunning)
            {
                PluginLog.Log($"stop first");
                return;
            }

            taskRunning = true;
            Task task = new(() =>
            {
                PluginLog.Log($"start...");
                GameData.FishBot.YFishScript(args);
                PluginLog.Log($"end...");
                taskRunning = false;
                GameData.FishBot.Init();
            });
            task.Start();
        }

        private void OnHFishCommand(string command, string args)
        {
            PluginLog.Log($"Hfish: {args}");
            if (args.Length == 0)
            {
                taskRunning = false;
                GameData.HFishBot.StopScript();
                return;
            }

            if (taskRunning)
            {
                PluginLog.Log($"stop first");
                return;
            }

            taskRunning = true;
            Task task = new(() =>
            {
                PluginLog.Log($"start...");
                GameData.HFishBot.Script();
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        private void OnCollectionFishCommand(string command, string args) {
            PluginLog.Log($"collectionfish: {args}");
            if (args.Length == 0)
            {
                GameData.CollectionFishBot.StopScript();
                taskRunning = false;
                return;
            }

            if (taskRunning)
            {
                PluginLog.Log($"stop first");
                return;
            }

            taskRunning = true;
            Task task = new(() =>
            {
                PluginLog.Log($"start...");
                GameData.CollectionFishBot.CollectionFishScript(args);
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        private void OnGatherCommand(string command, string args)
        {
            PluginLog.Log($"gather: {args}");
            if (args.Length == 0)
            {
                GameData.GatherBot.StopScript();
                taskRunning = false;
                return;
            }

            if (taskRunning)
            {
                PluginLog.Log($"stop first");
                return;
            }

            taskRunning = true;
            Task task = new(() =>
            {
                PluginLog.Log($"start...");
                GameData.GatherBot.NormalScript(args);
                taskRunning = false;
                PluginLog.Log($"end...");
            });
            task.Start();
        }

        private void OnYGatherCommand(string command, string args)
        {
            PluginLog.Log($"ygather: {args}");
            if (args.Length == 0)
            {
                GameData.GatherBot.StopScript();
                taskRunning = false;
                return;
            }

            if (taskRunning)
            {
                PluginLog.Log($"stop first");
                return;
            }

            taskRunning = true;
            Task task = new(() =>
            {
                PluginLog.Log($"start...");
                GameData.GatherBot.YGatherScript(args);
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        // 测试专用
        private void OnWoTestCommand(string command, string args)
        {

            autoCraft.FetchRecommendation(1);
            // 当前区域
            //GameData.TerritoryType.TryGetValue(DalamudApi.ClientState.TerritoryType, out var v);
            //PluginLog.Log($"PlaceName: {v.PlaceName.Value.Name}");

            //GameData.Recipes.TryGetValue(31652, out var r);
            //PluginLog.Log($"r: {r.RecipeLevelTable}, {r.ItemResult.Value.RowId}");
            // 430  收藏用...
            //UnkData5Obj[] UnkData5 = r.UnkData5;
            //foreach (UnkData5Obj obj in UnkData5) {
            //    PluginLog.Log($"ItemIngredient : {obj.ItemIngredient}, AmountIngredient : {obj.AmountIngredient}");
            //}

            // 鼠标点击测试
            //GatherBot GatherBot = new GatherBot(GameData);
            //GatherBot.test();

            // 天气
            //(Weather.Weather LastWeather, Weather.Weather CurrentWeather, Weather.Weather NextWeather) = WeatherManager.FindLastCurrentNextWeather(DalamudApi.ClientState.TerritoryType);
            //PluginLog.Log($"LastWeather: {LastWeather.Name} CurrentWeather: {CurrentWeather.Name} NextWeather: {NextWeather.Name}");

            //CommonUi.test1();

            //CommonUi.test2();

            //CommonUi.test3();
        }

        // 生产 
        private void OnCraftCommand(string command, string args)
        {
            string[] str = args.Split(' ');
            PluginLog.Log($"craft: {args} length: {args.Length}");

            if (args.Length <= 1)
            {
                PluginLog.Log($"stop");
                GameData.CraftBot.StopScript();
                taskRunning = false;
                return;
            }

            if (taskRunning)
            {
                PluginLog.Log($"stop first");
                return;
            }

            taskRunning = true;
            Task task = new(() =>
            {
                PluginLog.Log($"start...");
                GameData.CraftBot.CraftScript(args);
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        private void OnDailyCommand(string command, string args)
        {
            string[] str = args.Split(' ');
            PluginLog.Log($"daily: {args}");
            if (args.Length == 0)
            {
                PluginLog.Log($"stop");
                GameData.DailyBot.StopScript();
                taskRunning = false;
                return;
            }

            if (taskRunning)
            {
                PluginLog.Log($"stop first");
                return;
            }

            taskRunning = true;
            Task task = new(() =>
            {
                PluginLog.Log($"start...");
                GameData.DailyBot.DailyScript(args);
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        private void OnMarketCommand(string command, string args)
        {
            string[] str = args.Split(' ');
            PluginLog.Log($"market: {args}");
            if (args.Length == 0)
            {
                PluginLog.Log($"stop");
                GameData.DailyBot.StopScript();
                taskRunning = false;
                return;
            }

            if (taskRunning)
            {
                PluginLog.Log($"stop first");
                return;
            }

            taskRunning = true;
            Task task = new(() =>
            {
                PluginLog.Log($"start...");
                GameData.MarketBot.RunScript(1);
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        private void DrawUI()
        {
            //PluginUi.Draw();
        }

        private void DrawConfigUI()
        {
            PluginUi.SettingsVisible = true;
        }

        private void OnLoginEvent(object? sender, EventArgs e)
        {
            PluginLog.Log($"=====>>> login...");
            PluginLog.Log($"=====>>> {DalamudApi.ClientState.IsLoggedIn}");
        }

        private void OnLogoutEvent(object? sender, EventArgs e)
        {
            PluginLog.Log($"=====>>> logout... stop all");
            PluginLog.Log($"=====>>> {DalamudApi.ClientState.IsLoggedIn}");
            GameData.CommonBot.StopScript();
            GameData.MarketBot.StopScript();
            GameData.DailyBot.StopScript();
            GameData.CraftBot.StopScript();
            GameData.GatherBot.StopScript();
            GameData.FishBot.StopScript();
            GameData.HFishBot.StopScript();
            GameData.CollectionFishBot.StopScript();
        }

        private void AutoKillGame(object sender, ElapsedEventArgs e) {
            PluginLog.LogError("too long for running, kill game");
        }
    }
}
