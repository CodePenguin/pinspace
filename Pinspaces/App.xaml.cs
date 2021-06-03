using Microsoft.Extensions.DependencyInjection;
using Pinspaces.Controls;
using Pinspaces.Data;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System;
using System.Windows;

namespace Pinspaces
{
    public partial class App : Application
    {
        private readonly IServiceProvider serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
            Startup += OnStartup;
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDataRepository, JsonDataContext>();
            services.AddTransient<WindowFactory>();
            services.AddSingleton<IPinFactory, PinFactory>();
            services.AddTransient<PinJsonConverter>();
            services.AddTransient<PinspacePanel>();
            services.AddSingleton<WindowApplicationContext>();
            services.AddSingleton<PinWindowForm>();
            services.AddPinControls();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var appContext = serviceProvider.GetService<WindowApplicationContext>();
            appContext.Run();
        }
    }
}
