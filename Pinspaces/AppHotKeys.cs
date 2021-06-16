using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Pinspaces
{
    public class AppHotKeys : IDisposable
    {
        private const int WM_HOTKEY = 0x0312;
        private readonly Dictionary<int, HotKey> hotkeys = new();
        private bool disposedValue;
        private WindowInteropHelper hostWindow;
        private Window window;

        public AppHotKeys()
        {
            ComponentDispatcher.ThreadPreprocessMessage += ProcessMessage;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void RegisterHotkey(Key key, ModifierKeys modifierKeys, Action callback)
        {
            var hotkey = new HotKey(key, modifierKeys, callback);
            hotkeys.Add(hotkey.GetHashCode(), hotkey);
            if (window != null)
            {
                _ = NativeMethods.RegisterHotKey(hostWindow.Handle, hotkey.GetHashCode(), (uint)hotkey.ModifierKeys, (uint)KeyInterop.VirtualKeyFromKey(hotkey.Key));
            }
        }

        public void SetWindow(Window window)
        {
            if (this.window != null)
            {
                UnregisterHotkeys();
                hostWindow = null;
            }
            this.window = window;
            if (window != null)
            {
                hostWindow = new WindowInteropHelper(window);
                hostWindow.EnsureHandle();
                RegisterHotkeys();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ComponentDispatcher.ThreadPreprocessMessage -= ProcessMessage;
                    UnregisterHotkeys();
                    hostWindow = null;
                }
                disposedValue = true;
            }
        }

        private void ProcessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == WM_HOTKEY && hotkeys.TryGetValue(msg.wParam.ToInt32(), out var hotkey))
            {
                hotkey.Callback.Invoke();
            }
        }

        private void RegisterHotkeys()
        {
            if (window == null)
            {
                return;
            }
            foreach (var hotkey in hotkeys.Values)
            {
                _ = NativeMethods.RegisterHotKey(hostWindow.Handle, hotkey.GetHashCode(), (uint)hotkey.ModifierKeys, (uint)KeyInterop.VirtualKeyFromKey(hotkey.Key));
            }
        }

        private void UnregisterHotkeys()
        {
            if (window == null)
            {
                return;
            }
            foreach (var hotkey in hotkeys.Values)
            {
                _ = NativeMethods.UnregisterHotKey(hostWindow.Handle, hotkey.GetHashCode());
            }
        }

        internal static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fdModifiers, uint vk);

            [DllImport("user32.dll")]
            public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        }

        private class HotKey
        {
            public HotKey(Key key, ModifierKeys modifierKeys, Action callback)
            {
                Key = key;
                ModifierKeys = modifierKeys;
                Callback = callback;
            }

            public Action Callback { get; private set; }
            public Key Key { get; private set; }
            public ModifierKeys ModifierKeys { get; private set; }
        }
    }
}
