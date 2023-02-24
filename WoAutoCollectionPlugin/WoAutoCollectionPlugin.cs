using ClickLib;
using Dalamud.Game.Command;
using Dalamud.Game.Network;
using Dalamud.Game.Network.Structures;
using Dalamud.Logging;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WoAutoCollectionPlugin.GameAddressDetectors;
using WoAutoCollectionPlugin.PingTrackers;
using WoAutoCollectionPlugin.SeFunctions;
using WoAutoCollectionPlugin.Time;
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
        private const string hfish = "/hfish";
        private const string collectionfish = "/collectionfish";
        private const string gather = "/gather";
        private const string ygather = "/ygather";
        private const string woTest = "/woTest";
        private const string actionTest = "/actionTest";
        private const string close = "/close";
        private const string craft = "/craft";
        private const string daily = "/daily";
        private const string market = "/market";

        public Configuration Configuration { get; private set; }
        public static SeTime Time { get; private set; }
        private PluginUI PluginUi { get; init; }
        public static GameData GameData { get; private set; }
        internal MarketEventHandler MarketEventHandler { get; private set; }

        public static Executor Executor;
        public static bool newRequest;
        public static GetFilePointer getFilePtr;
        public static List<MarketBoardCurrentOfferings> _cache = new();
        public static Lumina.Excel.ExcelSheet<Item> items;
        public static bool taskRunning = false;

        private readonly GameNetwork network;
        private readonly GameAddressDetector addressDetector;
        public static PingTracker pingTracker;
        internal ICallGateProvider<object, object> IpcProvider;

        public WoAutoCollectionPlugin(DalamudPluginInterface pluginInterface, GameNetwork network)
        {
            DalamudApi.Initialize(pluginInterface);

            // 可视化ui
            //Configuration = DalamudApi.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            //Configuration.Initialize();

            //Commands.InitializeCommands();
            //Configuration.Initialize(DalamudApi.PluginInterface);
            Click.Initialize();
            newRequest = false;
            MarketEventHandler = new MarketEventHandler();

            DalamudApi.GameNetwork.NetworkMessage += MarketEventHandler.OnNetworkEvent;

            try
            {
                var ptr = DalamudApi.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 85 C0 74 14 83 7B 44 00");
                getFilePtr = Marshal.GetDelegateForFunctionPointer<GetFilePointer>(ptr);
            }
            catch (Exception e)
            {
                getFilePtr = null;
                PluginLog.LogError(e.ToString());
            }

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
                Executor = new Executor();

                this.network = network;
                addressDetector = pluginInterface.Create<AggregateAddressDetector>();
                pingTracker = RequestNewPingTracker();
                pingTracker.Verbose = false;
                pingTracker.Start();

                try
                {
                    IpcProvider = pluginInterface.GetIpcProvider<object, object>("PingPlugin");
                    pingTracker.OnPingUpdated += payload =>
                    {
                        dynamic obj = new ExpandoObject();
                        obj.LastRTT = payload.LastRTT;
                        obj.AverageRTT = payload.AverageRTT;
                        IpcProvider.SendMessage(obj);
                    };
                }
                catch (Exception e)
                {
                    PluginLog.Error($"Error registering IPC provider:\n{e}");
                }
                pingTracker.OnPingUpdated += PacketPingTracker.UpdatePing;

                //DalamudApi.PluginInterface.UiBuilder.Draw += DrawUI;
                //DalamudApi.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            }
            catch (Exception e)
            {
                PluginLog.Error($"Failed loading WoAutoCollectionPlugin\n{e}");
            }
        }

        private PingTracker RequestNewPingTracker()
        {
            pingTracker?.Dispose();
            PingTracker newTracker = new PacketPingTracker(addressDetector, network);

            pingTracker = newTracker;
            if (pingTracker == null)
            {
                throw new InvalidOperationException("Failed to create ping tracker. The provided arguments may be incorrect.");
            }
            pingTracker.Start();
            return newTracker;
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
            DalamudApi.CommandManager.RemoveHandler(actionTest);
            DalamudApi.CommandManager.RemoveHandler(close);
            DalamudApi.CommandManager.RemoveHandler(craft);
            DalamudApi.CommandManager.RemoveHandler(daily);
            DalamudApi.CommandManager.RemoveHandler(market);
            MarketEventHandler.Dispose();
            DalamudApi.GameNetwork.NetworkMessage -= MarketEventHandler.OnNetworkEvent;
            MarketCommons.Dispose();

            pingTracker.OnPingUpdated -= PacketPingTracker.UpdatePing;
            pingTracker.Dispose();
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
            PluginLog.Log($"fish: {args}");
            if (args.Length == 0)
            {
                GameData.FishBot.StopYFishScript();
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
                GameData.CollectionFishBot.StopCollectionFishScript();
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

            CommonUi.test2();

            //CommonUi.test3();
        }

        private void OnActionTestCommand(string command, string args)
        {
            // 技能 测试
            Game.Initialize();
            Game.Test();
            //Game.DisAble();
        }

        private void OnCloseTestCommand(string command, string args)
        {
            Game.DisAble();
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
                GameData.MarketBot.RunScript();
                PluginLog.Log($"end...");
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
