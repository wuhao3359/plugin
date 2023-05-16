﻿using Dalamud.Game;
using System;

namespace AlphaProject.SeFunctions
{
    public enum FishingState : byte
    {
        None = 0,
        PoleOut = 1,
        PullPoleIn = 2,
        Quit = 3,
        PoleReady = 4,
        Bite = 5,
        Reeling = 6,
        Waiting = 8,
        Waiting2 = 9,
    }

    public sealed class EventFramework : SeAddressBase
    {
        private const int FishingManagerOffset = 0x70;
        private const int FishingStateOffset = 0x220;

        internal unsafe IntPtr FishingManager
        {
            get
            {
                if (Address == IntPtr.Zero)
                    return IntPtr.Zero;

                var managerPtr = *(IntPtr*)Address + FishingManagerOffset;
                if (managerPtr == IntPtr.Zero)
                    return IntPtr.Zero;

                return *(IntPtr*)managerPtr;
            }
        }

        internal IntPtr FishingStatePtr
        {
            get
            {
                var ptr = FishingManager;
                if (ptr == IntPtr.Zero)
                    return IntPtr.Zero;

                return ptr + FishingStateOffset;
            }
        }

        public unsafe FishingState FishingState
        {
            get
            {
                var ptr = FishingStatePtr;
                return ptr != IntPtr.Zero ? *(FishingState*)ptr : FishingState.None;
            }
        }

        public EventFramework(SigScanner sigScanner)
            : base(sigScanner,
                "48 8B 2D ?? ?? ?? ?? 48 8B F1 48 8B 85 ?? ?? ?? ?? 48 8B 18 48 3B D8 74 35 0F 1F 00 F6 83 ?? ?? ?? ?? ?? 75 1D 48 8B 46 28 48 8D 4E 28 48 8B 93 ?? ?? ?? ??")
        { }
    }
}
