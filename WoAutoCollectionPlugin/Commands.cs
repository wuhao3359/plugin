using Dalamud.Game.Command;
using System.Collections.Generic;

namespace WoAutoCollectionPlugin
{
    // TODO 代码重构
    public class Commands
    {
        private const string collect = "/collect";

        private const string fish = "/fish";

        private const string gather = "/gather";

        private const string woTest = "/woTest";

        private const string actionTest = "/actionTest";

        private const string close = "/close";

        private const string craft = "/craft";

        private static Dictionary<string, CommandInfo> _commands = new();

        public static void InitializeCommands()
        {
            DalamudApi.CommandManager.AddHandler(collect, new CommandInfo(Test)
            {
                HelpMessage = "当前坐标信息"
            });

            DalamudApi.CommandManager.AddHandler(fish, new CommandInfo(Test)
            {
                HelpMessage = "fish {param}"
            });

            DalamudApi.CommandManager.AddHandler(gather, new CommandInfo(Test)
            {
                HelpMessage = "gather {param}"
            });

            DalamudApi.CommandManager.AddHandler(woTest, new CommandInfo(Test)
            {
                HelpMessage = "wotest"
            });

            DalamudApi.CommandManager.AddHandler(actionTest, new CommandInfo(Test)
            {
                HelpMessage = "actionTest"
            });

            DalamudApi.CommandManager.AddHandler(close, new CommandInfo(Test)
            {
                HelpMessage = "close"
            });

            DalamudApi.CommandManager.AddHandler(craft, new CommandInfo(Test)
            {
                HelpMessage = "Craft"
            });

            foreach (var (command, info) in _commands)
                DalamudApi.CommandManager.AddHandler(command, info);
        }

        private static void Test(string command, string args)
        {
            // test
        }
    }
}
