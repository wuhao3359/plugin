﻿using Dalamud.Game.Command;
using Dalamud.Logging;
using Dalamud.Plugin;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WoAutoCollectionPlugin.Bot;
using WoAutoCollectionPlugin.Managers;
using WoAutoCollectionPlugin.SeFunctions;
using WoAutoCollectionPlugin.Time;
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

        public WoAutoCollectionPlugin Plugin { get; private set; }

        public Configuration Configuration { get; private set; }

        public static SeTime Time { get; private set; } = null!;

        private PluginUI PluginUi { get; init; }

        public GameData GameData { get; init; }

        public bool taskRunning = false;

        public WoAutoCollectionPlugin(DalamudPluginInterface pluginInterface)
        {
            Plugin = this;
            DalamudApi.Initialize(pluginInterface);

            Configuration = DalamudApi.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize();

            //Commands.InitializeCommands();
            //Configuration.Initialize(DalamudApi.PluginInterface);
            ClickLib.Click.Initialize();
            //DalamudApi.ChatManager = new ChatManager();
            Time = new SeTime();

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

                GameData = new GameData(DalamudApi.DataManager);
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
            DalamudApi.CommandManager.RemoveHandler(hfish);
            DalamudApi.CommandManager.RemoveHandler(collectionfish);
            DalamudApi.CommandManager.RemoveHandler(gather);
            DalamudApi.CommandManager.RemoveHandler(ygather);
            DalamudApi.CommandManager.RemoveHandler(woTest);
            DalamudApi.CommandManager.RemoveHandler(actionTest);
            DalamudApi.CommandManager.RemoveHandler(close);
            DalamudApi.CommandManager.RemoveHandler(craft);
            DalamudApi.CommandManager.RemoveHandler(daily);

            Time.Dispose();
            //DalamudApi.ChatManager?.Dispose();
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

            FishBot FishBot = new FishBot(GameData);
            if (area <= 0)
            {
                FishBot.StopYFishScript();
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
                FishBot.YFishScript(args);
                PluginLog.Log($"end...");
                taskRunning = false;
                FishBot.Init();
            });
            task.Start();
        }

        private void OnHFishCommand(string command, string args)
        {
            string[] str = args.Split(' ');
            int area = int.Parse(str[0]);
            PluginLog.Log($"Hfish: {args}");

            HFishBot HFishBot = new HFishBot(GameData);
            if (area <= 0)
            {
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
                HFishBot.Script();
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        private void OnCollectionFishCommand(string command, string args) {
            string[] str = args.Split(' ');
            int area = int.Parse(str[0]);
            PluginLog.Log($"collectionfish: {args}");

            CollectionFishBot CollectionFishBot = new CollectionFishBot(GameData);
            if (area <= 0)
            {
                CollectionFishBot.StopCollectionFishScript();
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
                CollectionFishBot.CollectionFishScript(args);
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        private void OnGatherCommand(string command, string args)
        {
            string[] str = args.Split(' ');
            int area = int.Parse(str[0]);
            PluginLog.Log($"gather: {area}");

            GatherBot GatherBot = new GatherBot(GameData);
            if (area <= 0)
            {
                GatherBot.StopScript();
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
                GatherBot.NormalScript(area);
                taskRunning = false;
                PluginLog.Log($"end...");
            });
            task.Start();
        }

        private void OnYGatherCommand(string command, string args)
        {
            string[] str = args.Split(' ');
            int area = int.Parse(str[0]);
            PluginLog.Log($"ygather: {area}");

            GatherBot GatherBot = new GatherBot(GameData);
            if (area <= 0)
            {
                GatherBot.StopScript();
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
                GatherBot.YGatherScript(args);
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        // 测试专用
        private void OnWoTestCommand(string command, string args)
        {
            // 技能 hook 测试
            //Game.Initialize();
            //DalamudApi.CommandManager.ProcessCommand($"/gearset change \"{set}\"");

            //string recipeName = "橙汁";
            //PluginLog.Log($"{recipeName}");
            //uint recipeId = RecipeNoteUi.SearchRecipeId(recipeName);
            //PluginLog.Log($"{recipeId}");
            //RecipeNoteUi.OpenRecipeNote(recipeId);

            // target ok
            //CommonBot commonBot = new CommonBot(new KeyOperates(GameData));
            //string targetName = "艾妮";
            //commonBot.SetTarget(targetName);

            //// 使用技能 Miner Botanist Fisher
            ////DalamudApi.ChatManager.SendMessage("/ac 技能名");
            //DalamudApi.CommandManager.ProcessCommand("/ac 冲刺");
            string message = "/gearset change Miner";

            DalamudApi.CommandManager.ProcessCommand(message);

            var (text, length) = PrepareString(message);
            var payload = PrepareContainer(text, length);
            ProcessChatBox _processChatBox = new ProcessChatBox(DalamudApi.SigScanner);
            IntPtr _uiModulePtr = DalamudApi.GameGui.GetUIModule();

            _processChatBox.Invoke(_uiModulePtr, payload, IntPtr.Zero, (byte)0);

            Marshal.FreeHGlobal(payload);
            Marshal.FreeHGlobal(text);

            // 鼠标点击测试
            //GatherBot.test();

            // 时间测试
            var hour = Time.ServerTime.CurrentEorzeaHour();
            var minute = Time.ServerTime.CurrentEorzeaMinute();
            PluginLog.Log($"{hour} {minute}");

            // 背包测试 ok
            //BagManager bagManager = new BagManager();
            //bagManager.test();
        }

        private static (IntPtr, long) PrepareString(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var mem = Marshal.AllocHGlobal(bytes.Length + 30);
            Marshal.Copy(bytes, 0, mem, bytes.Length);
            Marshal.WriteByte(mem + bytes.Length, 0);
            return (mem, bytes.Length + 1);
        }

        private static IntPtr PrepareContainer(IntPtr message, long length)
        {
            var mem = Marshal.AllocHGlobal(400);
            Marshal.WriteInt64(mem, message.ToInt64());
            Marshal.WriteInt64(mem + 0x8, 64);
            Marshal.WriteInt64(mem + 0x10, length);
            Marshal.WriteInt64(mem + 0x18, 0);
            return mem;
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
            Game.DisAble();
        }

        // 生产 
        // {param0}-宏按键
        // {param1}-周期 
        // {param2}-兑换物品(id)
        // TODO 
        private void OnCraftCommand(string command, string args)
        {
            string[] str = args.Split(' ');
            PluginLog.Log($"craft: {args} length: {args.Length}");

            CraftBot CraftBot = new CraftBot(GameData);
            if (args.Length <= 1)
            {
                PluginLog.Log($"stop");
                CraftBot.StopScript();
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
                CraftBot.CraftScript(args);
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        private void OnDailyCommand(string command, string args)
        {
            string[] str = args.Split(' ');
            PluginLog.Log($"daily: {args} length: {args.Length}");

            DailyBot DailyBot = new DailyBot(GameData);
            if (args.Length <= 1)
            {
                PluginLog.Log($"stop");
                // stop
                DailyBot.StopScript();
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
                
                DailyBot.DailyScript(args);
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
