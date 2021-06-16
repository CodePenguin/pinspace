using Microsoft.Extensions.Options;
using Pinspaces.Configuration;
using Pinspaces.Controls;
using Pinspaces.Core.Data;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Pinspaces
{
    public class WindowApplicationContext : IDisposable
    {
        private readonly AppHotKeys appHotKeys;
        private readonly IDataRepository dataRepository;
        private readonly Settings settings;
        private readonly WindowFactory windowFactory;
        private readonly List<PinWindowForm> windows = new();
        private bool disposedValue;

        public WindowApplicationContext(WindowFactory windowFactory, IDataRepository dataRepository, IOptions<Settings> options)
        {
            this.windowFactory = windowFactory;
            this.dataRepository = dataRepository;
            settings = options.Value;
            appHotKeys = new AppHotKeys();

            RegisterActions();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void LoadData()
        {
            var pinWindows = dataRepository.GetPinWindows();
            if (pinWindows.Count == 0)
            {
                NewWindow();
            }
            foreach (var window in pinWindows)
            {
                NewWindow(window);
            }

            appHotKeys.SetWindow(windows[0]);
        }

        public void NewWindow()
        {
            var pinspace = new Pinspace();
            var pinWindow = new PinWindow
            {
                ActivePinspaceId = pinspace.Id,
                Height = PinWindow.DefaultHeight,
                Left = SystemParameters.WorkArea.Width / 2 - PinWindow.DefaultWidth / 2,
                Top = SystemParameters.WorkArea.Height / 2 - PinWindow.DefaultHeight / 2,
                Width = PinWindow.DefaultWidth
            };

            dataRepository.UpdatePinspace(pinspace);
            dataRepository.UpdatePinWindow(pinWindow);
            NewWindow(pinWindow);
        }

        public void NewWindow(PinWindow pinWindow)
        {
            var window = windowFactory.CreateForm<PinWindowForm>();
            windows.Add(window);
            window.Closed += Window_FormClosed;
            window.LoadWindow(pinWindow);
            window.Show();
        }

        public void Run()
        {
            LoadData();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    appHotKeys.Dispose();
                }
                disposedValue = true;
            }
        }

        private void Hotkey_ActivateApplication()
        {
            var window = windows[0];
            window.Activate();
            if (window.WindowState == WindowState.Minimized)
            {
                window.WindowState = WindowState.Normal;
            }
        }

        private void RegisterActions()
        {
            var availableApplicationCommands = new Dictionary<string, Action>
            {
                { "activate", Hotkey_ActivateApplication }
            };

            foreach (var action in settings.Actions)
            {
                if (!action.TryConvertKeys(out var key, out var modifierKeys))
                {
                    return;
                }
                // Global Application Shortcuts
                if (availableApplicationCommands.TryGetValue(action.Command, out var actionHandler))
                {
                    appHotKeys.RegisterHotkey(key, modifierKeys, actionHandler);
                    continue;
                }

                // Internal Command Shortcuts
                foreach (var field in typeof(CustomCommands).GetFields())
                {
                    if (!field.FieldType.Equals(typeof(RoutedUICommand)))
                    {
                        return;
                    }
                    var routedUICommand = field.GetValue(null) as RoutedUICommand;
                    if (routedUICommand.Name == action.Command)
                    {
                        routedUICommand.InputGestures.Add(new KeyGesture(key, modifierKeys));
                    }
                }
            }
        }

        private void Window_FormClosed(object sender, EventArgs e)
        {
            var window = sender as PinWindowForm;
            var wasMainWindow = windows.IndexOf(window) == 0;
            windows.Remove(window);
            window.Close();
            if (wasMainWindow)
            {
                appHotKeys.SetWindow(windows.Count > 0 ? windows[0] : null);
            }
        }
    }
}
