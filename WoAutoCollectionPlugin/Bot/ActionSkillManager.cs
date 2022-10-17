using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;

namespace WoAutoCollectionPlugin.Bot
{
    public class ActionSkillManager
    {

        private unsafe ActionManager* AM;

        public unsafe ActionSkillManager()
        {
            AM = ActionManager.Instance();
        }

        public unsafe bool UseSpellActionByIdAndType(uint id, ActionType actionTpe)
        {
            bool b = false;
            try
            {
                b = AM->UseAction(actionTpe, id);
            }
            catch (Exception e)
            {
                PluginLog.Error($"error!!!\n{e}");
            }
            return b;
        }
    }
}