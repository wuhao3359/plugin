using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;

namespace WoAutoCollectionPlugin.UseAction
{
    public unsafe class Game
    {

        public static ActionManager* actionManager;

        public delegate byte UseActionDelegate(ActionManager* actionManager, uint actionType, uint actionID, long targetObjectID, uint param, uint useType, int pvp, bool* isGroundTarget);
        public static Hook<UseActionDelegate> UseActionHook;

        private static byte UseActionDetour(ActionManager* actionManager, uint actionType, uint actionID, long targetObjectID, uint param, uint useType, int pvp, bool* isGroundTarget)
            => ActionStackManager.OnUseAction(actionManager, actionType, actionID, targetObjectID, param, useType, pvp, isGroundTarget);

        public static void Initialize()
        {
            actionManager = ActionManager.Instance();

            SignatureHelper.Initialise(new Game());
            UseActionHook = new Hook<UseActionDelegate>((IntPtr)ActionManager.fpUseAction, UseActionDetour);

            UseActionHook.Enable();
        }

        public static void Test()
        {
            // UseAction called {1}, {2470} -> {2470}, {103BFC80:X}, {0}, {0}, {0}
            // UseAction called {actionType}, {actionID} -> {adjustedActionID}, {targetObjectID:X}, {param}, {useType}, {pvp}
            // actionManager, actionType, actionID, targetObjectID, param, useType, pvp, isGroundTarget
            //ActionManager.Instance()->UseAction(ActionType.Mount, 1);
            ActionManager.Instance()->UseAction(ActionType.General, 4);

        }

        public static void DisAble()
        {
            UseActionHook.Disable();
        }
    }
}
