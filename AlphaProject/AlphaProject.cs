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
using AlphaProject.Craft;
using AlphaProject.Enums;
using System.Diagnostics;

namespace AlphaProject;
public unsafe class AlphaProject : IDalamudPlugin
{
    public string Name => "AlphaProject";

    internal static AlphaProject AP;

    internal TaskManager TM;

    public static SeTime Time { get; private set; }
    private PluginUI PluginUi { get; init; }
    public static GameData GameData { get; private set; }
    internal MarketEventHandler MarketEventHandler { get; private set; }

    internal AutoCraft AutoCraft { get; private set; }

    public static Configuration Configuration { get; private set; }

    public static Executor Executor;
    public static bool newRequest;
    //public static GetFilePointer getFilePtr;
    public static List<MarketBoardCurrentOfferings> _cache = new();
    public static Lumina.Excel.ExcelSheet<Item> items;


    public static string beforePrice = "";
    public static bool getPriceSucceed = false;

    public static byte status = (byte)TaskState.READY;

    public static bool Debug = true;

    public static WindowSystem WindowSystem;

    // 定时器
    private Timer timer = new();

    public AlphaProject(DalamudPluginInterface pluginInterface, GameNetwork network)
    {
        DalamudApi.Initialize(pluginInterface);

        Configuration = DalamudApi.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(DalamudApi.PluginInterface);

        //Commands.InitializeCommands();
        //Configuration.Initialize(DalamudApi.PluginInterface);
        Click.Initialize();
        newRequest = false;
        MarketEventHandler = new MarketEventHandler();
        DalamudApi.GameNetwork.NetworkMessage += MarketEventHandler.OnNetworkEvent;
        DalamudApi.ClientState.Login += OnLoginEvent;
        DalamudApi.ClientState.Logout += OnLogoutEvent;

        timer.Interval = new Random().Next(990, 1020) * 60 * 1000; // 16.5h - 17h
        timer.Elapsed += AutoKillGame;
        timer.Start();

        try
        {
            GameData = new GameData(DalamudApi.DataManager);
            //items = GameData.DataManager.GetExcelSheet<Item>();
            Time = new SeTime();
            Executor = new Executor();

            WindowSystem = new WindowSystem(Name);
            WindowSystem.AddWindow(new SpearfishingHelper(GameData));

            PluginUi = new(Configuration);

            DalamudApi.PluginInterface.UiBuilder.Draw += WindowSystem.Draw;
            DalamudApi.PluginInterface.UiBuilder.Draw += DrawUI;
            //DalamudApi.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            TM = new();
            AP = this;
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
        MarketEventHandler.Dispose();
        DalamudApi.GameNetwork.NetworkMessage -= MarketEventHandler.OnNetworkEvent;
        DalamudApi.ClientState.Login -= OnLoginEvent;
        DalamudApi.ClientState.Logout -= OnLogoutEvent;
        MarketCommons.Dispose();

    AutoCraft.Dispose();
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
        DalamudApi.GameNetwork.NetworkMessage += MarketEventHandler.OnNetworkEvent;
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

        DalamudApi.GameNetwork.NetworkMessage -= MarketEventHandler.OnNetworkEvent;
    }

    private void AutoKillGame(object sender, ElapsedEventArgs e) {
        PluginLog.LogError("too long for running, kill game");
        Process.GetCurrentProcess().Kill();
        // 文本指令  	/shutdown
    }
}
