using Autofac;
using Pinspace.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using static System.Environment;

namespace Pinspace
{
    public class WindowApplicationContext : ApplicationContext
    {
        private readonly ILifetimeScope lifetimeScope;
        private readonly List<PinWindow> windows = new List<PinWindow>();
        private readonly Dictionary<Form, ILifetimeScope> windowScopes = new Dictionary<Form, ILifetimeScope>();

        public WindowApplicationContext(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
            LoadConfig();
            if (windows.Count == 0)
            {
                NewWindow(new PinWindowConfig());
            }
        }

        public void LoadConfig()
        {
            var configFileName = GetConfigFileName();
            if (!File.Exists(configFileName))
            {
                return;
            }
            var text = File.ReadAllText(configFileName);
            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new PinPanelConfigConverter());
            var config = JsonSerializer.Deserialize<PinspaceConfig>(text, deserializeOptions);
            foreach (var windowConfig in config.Windows)
            {
                NewWindow(windowConfig);
            }
        }

        public void NewWindow(PinWindowConfig config)
        {
            var windowScope = lifetimeScope.BeginLifetimeScope();
            var window = lifetimeScope.Resolve<PinWindow>();
            windows.Add(window);
            windowScopes.Add(window, windowScope);
            window.Disposed += Window_Disposed;
            window.FormClosed += Window_FormClosed;
            window.WindowApplicationContext = this;
            window.LoadConfig(config);
            window.Show();
        }

        public void SaveConfig()
        {
            var config = new PinspaceConfig();
            foreach (var window in windows)
            {
                config.Windows.Add(window.Config());
            }

            var configFileName = GetConfigFileName();
            using (var fileStream = new FileStream(configFileName, FileMode.Create))
            {
                var options = new JsonWriterOptions
                {
                    Indented = true
                };
                var serializeOptions = new JsonSerializerOptions();
                serializeOptions.Converters.Add(new PinPanelConfigConverter());
                var writer = new Utf8JsonWriter(fileStream, options);
                JsonSerializer.Serialize(writer, config, serializeOptions);
            }
        }

        private string GetConfigFileName()
        {
            var localAppDataPath = Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), "Pinspace");
            Directory.CreateDirectory(localAppDataPath);
            return Path.Combine(localAppDataPath, "settings.json");
        }

        private void Window_Disposed(object sender, EventArgs e)
        {
            var window = sender as Form;
            var scope = windowScopes[window];
            windowScopes.Remove(window);
            scope.Dispose();
        }

        private void Window_FormClosed(object sender, FormClosedEventArgs e)
        {
            var window = sender as PinWindow;
            windows.Remove(window);

            if (windows.Count == 0)
            {
                ExitThread();
            }
        }
    }
}
