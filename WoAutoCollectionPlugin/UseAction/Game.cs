using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Runtime.InteropServices;
using System.Text;
using WoAutoCollectionPlugin.SeFunctions;

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
            //ActionManager.Instance()->UseAction(ActionType.Mount, 1);    // 召唤坐骑
            //ActionManager.Instance()->UseAction(ActionType.General, 4); // 疾跑
            //ActionManager.Instance()->UseAction(ActionType.Spell, 19700);
            //ActionManager.Instance()->GetActionStatus(ActionType.Spell, 19700);
            float t = ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Spell, 4590); // 4589-采矿 大地的恩惠 4590-园艺 大地的恩惠
            PluginLog.Log($"{t}");
            bool tt = ActionManager.Instance()->IsRecastTimerActive(ActionType.Spell, 4590);
            PluginLog.Log($"{tt}");
            uint ttt = ActionManager.Instance()->GetActionStatus(ActionType.Spell, 4590);
            PluginLog.Log($"{ttt}");
            // 26521-采矿 理智同兴
        }

        // return 0 时表示技能冷却结束
        public static float GetSpellActionRecastTimeElapsed(uint id) {
            float b = 0;
            try
            {
                b = ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Spell, id);
            }
            catch (Exception e)
            {
                PluginLog.Error($"error!!!\n{e}");
            }
            return b;
        }

        // 19700-以太钻孔机
        public static bool UseSpellActionById(uint id)
        {
            bool b = false;
            try
            {
                b = ActionManager.Instance()->UseAction(ActionType.Spell, id);
            } catch (Exception e) {
                PluginLog.Error($"error!!!\n{e}");
            }
            return b;
        }

        public static void ChangeJob(string name)
        {
            string message = "/gearset change " + name;
            bool flag = DalamudApi.CommandManager.ProcessCommand(message);

            if (!flag) {
                //ExecuteMessage(message);
            }
        }

        //public static void ExecuteMessage(string message) {
        //    try {
        //        var (text, length) = PrepareString(message);
        //        var payload = PrepareContainer(text, length);
        //        ProcessChatBox _processChatBox = new ProcessChatBox(DalamudApi.SigScanner);
        //        IntPtr _uiModulePtr = DalamudApi.GameGui.GetUIModule();

        //        _processChatBox.Invoke(_uiModulePtr, payload, IntPtr.Zero, (byte)0);

        //        Marshal.FreeHGlobal(payload);
        //        Marshal.FreeHGlobal(text);
        //    }
        //    catch (Exception e) {
        //        PluginLog.Error($"error!!!\n{e}");
        //    }
        //}

        //private static (IntPtr, long) PrepareString(string message)
        //{
        //    var bytes = Encoding.UTF8.GetBytes(message);
        //    var mem = Marshal.AllocHGlobal(bytes.Length + 30);
        //    Marshal.Copy(bytes, 0, mem, bytes.Length);
        //    Marshal.WriteByte(mem + bytes.Length, 0);
        //    return (mem, bytes.Length + 1);
        //}

        //private static IntPtr PrepareContainer(IntPtr message, long length)
        //{
        //    var mem = Marshal.AllocHGlobal(400);
        //    Marshal.WriteInt64(mem, message.ToInt64());
        //    Marshal.WriteInt64(mem + 0x8, 64);
        //    Marshal.WriteInt64(mem + 0x10, length);
        //    Marshal.WriteInt64(mem + 0x18, 0);
        //    return mem;
        //}

        public static void DisAble()
        {
            UseActionHook.Disable();
        }
    }
}
