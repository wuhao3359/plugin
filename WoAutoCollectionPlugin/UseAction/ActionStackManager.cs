using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;

namespace WoAutoCollectionPlugin.UseAction
{
    public static unsafe class ActionStackManager
    {
        public enum TargetType
        {
            Target,
            SoftTarget,
            FocusTarget,
            UITarget,
            FieldTarget,
            TargetsTarget,
            Self,
            LastTarget,
            LastEnemy,
            LastAttacker,
            P2,
            P3,
            P4,
            P5,
            P6,
            P7,
            P8
        }

        public static byte OnUseAction(ActionManager* actionManager, uint actionType, uint actionID, long targetObjectID, uint param, uint useType, int pvp, bool* isGroundTarget)
        {
            var adjustedActionID = actionType == 1 ? actionManager->GetAdjustedActionId(actionID) : actionID;
            PluginLog.Log($"UseAction called {actionType}, {actionID} -> {adjustedActionID}, {targetObjectID:X}, {param}, {useType}, {pvp}");
            if (TryDismount(actionType, adjustedActionID, targetObjectID, useType, pvp, out var ret))
                return ret;
            try
            {
                ret = Game.UseActionHook.Original(actionManager, actionType, actionID, targetObjectID, param, useType, pvp, isGroundTarget);
            }
            catch (Exception e)
            {
                PluginLog.Log($"error action, {e}");
            }
            return ret;
        }

        private static bool TryDismount(uint actionType, uint actionID, long targetObjectID, uint useType, int pvp, out byte ret)
        {
            ret = 0;

            //if (!DalamudApi.Condition[ConditionFlag.Mounted]
            //    || actionType == 1 && ReAction.mountActionsSheet.ContainsKey(actionID)
            //    || (actionType != 5 || actionID is not (3 or 4)) && (actionType != 1 || actionID is 5 or 6) // +Limit Break / +Sprint / -Teleport / -Return
            //    || Game.actionManager->GetActionStatus((ActionType)actionType, actionID, targetObjectID, 0, 0) == 0)
            //    return false;

            //ret = Game.UseActionHook.Original(Game.actionManager, 5, 23, 0, 0, 0, 0, null);
            //if (ret == 0) return true;

            //PluginLog.Error($"Dismounting {actionType}, {actionID}, {targetObjectID}, {useType}, {pvp}");

            //isMountActionQueued = true;
            //queuedMountAction = (actionType, actionID, targetObjectID, useType, pvp);
            //mountActionTimer.Restart();
            return false;
        }
    }
}
