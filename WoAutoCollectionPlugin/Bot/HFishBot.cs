﻿using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WoAutoCollectionPlugin.SeFunctions;
using WoAutoCollectionPlugin.Utility;

namespace WoAutoCollectionPlugin.Bot
{
    public class HFishBot
    {
        private KeyOperates KeyOperates { get; init; }
        private EventFramework EventFramework { get; init; }
        private static SeTugType TugType { get; set; } = null!;

        private FishRecord Record;

        private FishingState LastState = FishingState.None;
        private FishingState FishingState = FishingState.None;
        private bool closed = false;

        public HFishBot(GameData GameData)
        {
            KeyOperates = new KeyOperates(GameData);
            EventFramework = new EventFramework(DalamudApi.SigScanner);

            TugType = new SeTugType(DalamudApi.SigScanner);
            Record = new FishRecord();
        }

        public void RunScript() {
            if (!DalamudApi.Condition[ConditionFlag.OccupiedInCutSceneEvent] && !DalamudApi.Condition[ConditionFlag.Fishing])
            {
                KeyOperates.KeyMethod(Keys.n2_key);
            }
        }

        public void OnHFishUpdate(Framework _)
        {
            FishingState = EventFramework.FishingState;
            if (LastState == FishingState)
                return;
            LastState = FishingState;
            switch (FishingState)
            {
                case FishingState.PoleOut:
                    break;
                case FishingState.Bite:
                    OnHFishBite();
                    break;
                case FishingState.Reeling:
                    break;
                case FishingState.PoleReady:
                    HFishScript();
                    break;
                case FishingState.Waiting:
                    break;
                case FishingState.Quit:
                    break;
            }
        }

        private void OnHFishBite()
        {
            Record.SetTugHook(TugType.Bite, Record.Hook);
            Task task = new(() =>
            {
                PluginLog.Log($"HFish bit with {Record.Tug}");
                switch (Record.Tug.ToString())
                {
                    case "Weak":
                        KeyOperates.KeyMethod(Keys.n3_key);
                        break;
                    case "Strong":
                        KeyOperates.KeyMethod(Keys.n4_key);
                        break;
                    case "Legendary":
                        KeyOperates.KeyMethod(Keys.n4_key);
                        break;
                    default:
                        break;
                }
                KeyOperates.KeyMethod(Keys.n1_key);
            });
            task.Start();
        }

        private void HFishScript()
        {
            Task task = new(() =>
            {
                Thread.Sleep(800);
                PlayerCharacter? player = DalamudApi.ClientState.LocalPlayer;
                bool existStatus = false;
                byte stackCount = 0;
                IEnumerator<Dalamud.Game.ClientState.Statuses.Status> statusList = player.StatusList.GetEnumerator();
                while (statusList.MoveNext())
                {
                    // 2778-捕鱼人之识 850-耐心
                    Dalamud.Game.ClientState.Statuses.Status status = statusList.Current;
                    uint statusId = status.StatusId;
                    byte StackCount = status.StackCount;
                    if (statusId == 850)
                    {
                        existStatus = true;
                    }
                    if (statusId == 2778)
                    {
                        stackCount = StackCount;
                    }
                }

                uint gp = player.CurrentGp;
                uint maxGp = player.MaxGp;
                if (gp < maxGp * 0.6)
                {
                    if (stackCount >= 3)
                    {
                        KeyOperates.KeyMethod(Keys.n0_key);
                        gp += 150;
                        Thread.Sleep(1000);
                    }
                }
                if (gp < maxGp * 0.5)
                {
                    KeyOperates.KeyMethod(Keys.minus_key);
                    Thread.Sleep(1500);
                }
                if (!existStatus)
                {
                    if (gp > 560)
                    {
                        KeyOperates.KeyMethod(Keys.F4_key);
                        Thread.Sleep(1000);
                        existStatus = true;
                        gp -= 560;
                    }
                    else if (gp > 200)
                    {
                        KeyOperates.KeyMethod(Keys.F3_key);
                        Thread.Sleep(1000);
                        existStatus = true;
                        gp -= 200;
                    }
                }
                Thread.Sleep(500);
                KeyOperates.KeyMethod(Keys.n2_key);
            });
            task.Start();
        }
    }
}