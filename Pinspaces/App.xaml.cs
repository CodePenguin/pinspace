using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pinspaces.Configuration;
using Pinspaces.Controls;
using Pinspaces.Core.Interfaces;
using Pinspaces.Data;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System.IO;
using System.Windows;

namespace Pinspaces
{
    public partial class App : Application
    {
        private IHost host;

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);
                    configurationBuilder.AddJsonFile("appsettings.json", optional: true);
                    configurationBuilder.AddJsonFile(Path.Combine(EnvironmentExtensions.GetLocalApplicationDataFolderPath, "Pinspaces.Settings.json"), optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<Settings>(context.Configuration);

                    services.AddSingleton<IDataRepository, JsonDataRepository>();
                    services.AddTransient<IPinDataRepository, PinDataRepository>();
                    services.AddTransient<IDelayedActionFactory, DelayedActionFactory>();
                    services.AddTransient<WindowFactory>();
                    services.AddSingleton<IPinFactory, PinFactory>();
                    services.AddTransient<PinJsonConverter>();
                    services.AddTransient<PinspacePanel>();
                    services.AddSingleton<WindowApplicationContext>();
                    services.AddSingleton<PinWindowForm>();
                    services.AddPinControls();
                });
        }

        protected async override void OnExit(ExitEventArgs e)
        {
            await host.StopAsync();
            host.Dispose();
            host = null;

            base.OnExit(e);
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            host = CreateHostBuilder(e.Args).Build();

            await host.StartAsync();

            var appContext = host.Services.GetService<WindowApplicationContext>();
            appContext.Run();
        }
    }
}
