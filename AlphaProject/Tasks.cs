using Dalamud.Logging;
using System;
using System.Numerics;
using System.Threading.Tasks;
using AlphaProject.Utility;

namespace AlphaProject
{
    public static class Tasks
    {
        public const string Collect = "Collect";
        public const string IFish = "IFish";
        public const string SFish = "SFish";
        public const string CFish = "CFish";
        public const string GGather = "GGather";
        public const string IGather = "IGather";
        public const string OTest = "OTest";
        public const string GCraft = "GCraft";
        public const string Daily = "Daily";
        public const string Market = "Market";

        public const string SpearFish = "SpearFish";

        public static bool taskRunning = false;

        // 当前坐标信息
        public static void GetCurrentPosition()
        {
            if (DalamudApi.ClientState != null && DalamudApi.ClientState.LocalPlayer != null)
            {
                Vector3 playerPosition = DalamudApi.ClientState.LocalPlayer.Position;
                try
                {
                    ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
                    float x = Maths.GetCoordinate(playerPosition.X, SizeFactor);
                    float y = Maths.GetCoordinate(playerPosition.Y, SizeFactor);
                    float z = Maths.GetCoordinate(playerPosition.Z, SizeFactor);
                    PluginLog.Log($"{DalamudApi.ClientState.TerritoryType}  {x}   {y}   {z}");
                    //Vector3 position = AlphaProject.GameData.KeyOperates.GetUserPosition(SizeFactor);
                    //GameObject go = Util.CurrentFishCanGather(position, SizeFactor);
                }
                catch (Exception)
                {
                    PluginLog.Log($"error");
                }
            }
        }

        // 空岛钓鱼
        public static void FishInIsland(string args)
        {
            PluginLog.Log($"IFish:{IFish} --- {args}");
            if (args.Length == 0)
            {
                AlphaProject.GameData.FishBot.StopScript();
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
                AlphaProject.GameData.FishBot.YFishScript(args);
                PluginLog.Log($"end...");
                taskRunning = false;
                AlphaProject.GameData.FishBot.Init();
            });
            task.Start();
        }

        // 海钓
        public static void FishOnSea(string args)
        {
            PluginLog.Log($"SFish:{SFish} --- {args}");
            if (args.Length == 0)
            {
                taskRunning = false;
                AlphaProject.GameData.HFishBot.StopScript();
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
                AlphaProject.GameData.HFishBot.Script();
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        // 收藏品-鱼
        public static void CollectibleFish(string args)
        {
            PluginLog.Log($"collectionfish:{CFish} --- {args}");
            if (args.Length == 0)
            {
                AlphaProject.GameData.CollectionFishBot.StopScript();
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
                AlphaProject.GameData.CollectionFishBot.CollectionFishScript(args);
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        public static void GeneralGather(string GGather, string args)
        {
            PluginLog.Log($"gather: {args}");
            if (args.Length == 0)
            {
                AlphaProject.GameData.GatherBot.StopScript();
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
                AlphaProject.GameData.GatherBot.NormalScript(args);
                taskRunning = false;
                PluginLog.Log($"end...");
            });
            task.Start();
        }

        public static void GatherInisland(string IGather, string args)
        {
            PluginLog.Log($"ygather: {args}");
            if (args.Length == 0)
            {
                AlphaProject.GameData.GatherBot.StopScript();
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
                AlphaProject.GameData.GatherBot.YGatherScript(args);
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        public static void GeneralCraft(string GCraft, string args)
        {
            string[] str = args.Split(' ');
            PluginLog.Log($"craft: {args} length: {args.Length}");

            if (args.Length <= 1)
            {
                PluginLog.Log($"stop");
                AlphaProject.GameData.CraftBot.StopScript();
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
                AlphaProject.GameData.CraftBot.CraftScript(args);
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        public static void AutoDaily(string args)
        {
            PluginLog.Log($"AutoDaily:{Daily} -- {args}");
            if (args.Length == 0)
            {
                PluginLog.Log($"stop");
                AlphaProject.GameData.DailyBot.StopScript();
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
                AlphaProject.GameData.DailyBot.DailyScript(args);
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        public static void AutoMarket(string Market, string args)
        {
            string[] str = args.Split(' ');
            PluginLog.Log($"market: {args}");
            if (args.Length == 0)
            {
                PluginLog.Log($"stop");
                AlphaProject.GameData.DailyBot.StopScript();
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
                AlphaProject.GameData.MarketBot.RunScript(1);
                PluginLog.Log($"end...");
                taskRunning = false;
            });
            task.Start();
        }

        public static void OnlyTest(string OTest, string args)
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

            // CommonUi.test2();

            //CommonUi.test3();


            Task task = new(() =>
            {
                ushort SizeFactor = AlphaProject.GameData.GetSizeFactor(DalamudApi.ClientState.TerritoryType);
                Vector3 playerPosition = DalamudApi.ClientState.LocalPlayer.Position;
                float x = Maths.GetCoordinate(playerPosition.X, SizeFactor);
                float y = Maths.GetCoordinate(playerPosition.Y, SizeFactor);
                float z = Maths.GetCoordinate(playerPosition.Z, SizeFactor);
                Vector3 position = new(x, y, z);

                Vector3 A = new Vector3(1436, 1636, 1789);
                PluginLog.Log($"------------{Maths.Distance(position, A)}---------------");

                AlphaProject.GameData.KeyOperates.MoveToPoint(position, A, DalamudApi.ClientState.TerritoryType, false, false);

                playerPosition = DalamudApi.ClientState.LocalPlayer.Position;
                x = Maths.GetCoordinate(playerPosition.X, SizeFactor);
                y = Maths.GetCoordinate(playerPosition.Y, SizeFactor);
                z = Maths.GetCoordinate(playerPosition.Z, SizeFactor);
                position = new(x, y, z);
                PluginLog.Log($"------------{Maths.Distance(position, A)}---------------");
            });
            task.Start();
        }
    }
}
