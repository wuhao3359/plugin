using Dalamud.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WoAutoCollectionPlugin.Ui;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class DailyBot
    {
        //private GameData GameData { get; init; }
        private KeyOperates KeyOperates { get; init; }

        private CommonBot? CommonBot;
        private CraftBot? CraftBot;

        private static bool closed = false;

        // 0-默认(换生产白票 换天穹黄票)
        private int current = 0;
        private int next = 0;
        public DailyBot(GameData GameData)
        {
            //this.GameData = GameData;
            KeyOperates = new KeyOperates(GameData);
            CraftBot = new CraftBot(GameData);
            CommonBot = new CommonBot(KeyOperates);
        }

        public void Init()
        {
            closed = false;
            CraftBot.Init();
            CommonBot.Init();
        }

        public void StopScript()
        {
            closed = true;
            CraftBot.StopScript();
            CommonBot.StopScript();
        }

        // TODO 日常使用
        // 1.生产白票
        // 2.自动采集缺少的材料
        // 3.中间自动采集限时材料
        public void DailyScript(string args)
        {
            closed = false;

            TimePlan();
            while (!closed)
            {
                try
                {
                    if (next == 0)
                    {
                        DefaultTask();
                    }
                    // 默认
                }
                catch (Exception e)
                {
                    PluginLog.Error($"error!!!\n{e}");
                }
            }
        }

        public void TimePlan()
        {
            Task task = new(() =>
            {
                int n = 0;
                while (!closed && n < 8000)
                {
                    // 判断时间段 不同时间干不同事情
                }
            });
            task.Start();
        }

        public void DefaultTask()
        {
            int pressKey = 1;
            string recipeName = "";
            int exchangeItem = 10;
            // TODO 移动到商店旁边
            CraftBot.RunCraftScript(pressKey, recipeName, exchangeItem);
        }
    }
}