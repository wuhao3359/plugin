using ClickLib;
using Dalamud.Game.Network;
using Dalamud.Game.Network.Structures;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Timers;
using AlphaProject.SeFunctions;
using AlphaProject.Spearfishing;
using AlphaProject.Time;
using AlphaProject.Utility;

namespace AlphaProject
{
    public sealed class AlphaProject : IDalamudPlugin
    {
        public string Name => "AlphaProject";

        public Configuration Configuration { get; private set; }
        public static SeTime Time { get; private set; }
        private PluginUI PluginUi { get; init; }
        public static GameData GameData { get; private set; }
        internal MarketEventHandler MarketEventHandler { get; private set; }

        public static Executor Executor;
        public static bool newRequest;
        //public static GetFilePointer getFilePtr;
        public static List<MarketBoardCurrentOfferings> _cache = new();
        public static Lumina.Excel.ExcelSheet<Item> items;


        public static string beforePrice = "";
        public static bool getPriceSucceed = false;

        public static string status = "";

        public static bool Debug = true;

        public static WindowSystem WindowSystem;

        public AlphaProject(DalamudPluginInterface pluginInterface, GameNetwork network)
        {
            DalamudApi.Initialize(pluginInterface);

            // 可视化ui
            //Configuration = DalamudApi.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            //Configuration.Initialize();

            //Commands.InitializeCommands();
            //Configuration.Initialize(DalamudApi.PluginInterface);
            Click.Initialize();
            //newRequest = false;
            //MarketEventHandler = new MarketEventHandler();
            //DalamudApi.GameNetwork.NetworkMessage += MarketEventHandler.OnNetworkEvent;
            DalamudApi.ClientState.Login += OnLoginEvent;
            DalamudApi.ClientState.Logout += OnLogoutEvent;

            try
            {
                GameData = new GameData(DalamudApi.DataManager);
                //items = GameData.DataManager.GetExcelSheet<Item>();
                Time = new SeTime();
                Executor = new Executor();

                WindowSystem = new WindowSystem(Name);
                WindowSystem.AddWindow(new SpearfishingHelper(GameData));

                DalamudApi.PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
                DalamudApi.PluginInterface.UiBuilder.Draw += DrawUI;
                //DalamudApi.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
            }
            catch (Exception e)
            {
                PluginLog.Error($"Failed loading AlphaProject\n{e}");
            }
        }

        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //public delegate IntPtr GetFilePointer(byte index);

        public void Dispose()
        {
            PluginUi.Dispose();
            //MarketEventHandler.Dispose();
            //DalamudApi.GameNetwork.NetworkMessage -= MarketEventHandler.OnNetworkEvent;
            DalamudApi.ClientState.Login -= OnLoginEvent;
            DalamudApi.ClientState.Logout -= OnLogoutEvent;
            //MarketCommons.Dispose();

            if (WindowSystem != null)
                DalamudApi.PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
            WindowSystem?.RemoveAllWindows();
        }

        private void DrawUI()
        {
            PluginUi.Draw();
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
