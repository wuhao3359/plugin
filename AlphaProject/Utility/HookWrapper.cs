﻿using System;
using Dalamud.Hooking;
using Dalamud.Logging;

namespace AlphaProject.Utility
{
    // based on https://github.com/Caraxi/SimpleTweaksPlugin/blob/main/Helper/HookWrapper.cs
    public interface IHookWrapper : IDisposable
    {
        public bool IsEnabled { get; }
        public bool IsDisposed { get; }
        public void Enable();
        public void Disable();
    }

    public class HookWrapper<T> : IHookWrapper where T : Delegate
    {
        private bool disposed;

        private readonly Hook<T> wrappedHook;

        public HookWrapper(Hook<T> hook)
        {
            wrappedHook = hook;
        }

        public T Original => wrappedHook.Original;

        public void Enable()
        {
            if (disposed) return;
            wrappedHook?.Enable();
        }

        public void Disable()
        {
            if (disposed) return;
            wrappedHook?.Disable();
        }

        public void Dispose()
        {
            PluginLog.Information("Disposing of {cdelegate}", typeof(T).Name);
            Disable();
            disposed = true;
            wrappedHook?.Dispose();
        }

        public bool IsEnabled => wrappedHook.IsEnabled;
        public bool IsDisposed => wrappedHook.IsDisposed;
    }
}