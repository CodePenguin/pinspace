using Microsoft.Extensions.DependencyInjection;
using Pinspaces.Controls;
using Pinspaces.Core.Interfaces;
using Pinspaces.Data;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System.Windows;

namespace Pinspaces
{
    public partial class App : Application
    {
        private readonly ServiceProvider serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDataRepository, JsonDataRepository>();
            services.AddTransient<IDelayedActionFactory, DelayedActionFactory>();
            services.AddTransient<WindowFactory>();
            services.AddSingleton<IPinFactory, PinFactory>();
            services.AddTransient<PinJsonConverter>();
            services.AddTransient<PinspacePanel>();
            services.AddSingleton<WindowApplicationContext>();
            services.AddSingleton<PinWindowForm>();
            services.AddPinControls();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            serviceProvider.Dispose();
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var appContext = serviceProvider.GetService<WindowApplicationContext>();
            appContext.Run();
        }
    }
}
