using Microsoft.Extensions.DependencyInjection;
using Pinspaces.Controls;
using Pinspaces.Data;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System;
using System.Windows.Forms;

namespace Pinspaces
{
    public static class Program
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDataContext, JsonDataContext>();
            services.AddTransient<FormFactory>();
            services.AddSingleton<IPinFactory, PinFactory>();
            services.AddTransient<PinJsonConverter>();
            services.AddTransient<PinspacePanel>();
            services.AddTransient<PinWindowForm>();
            services.AddSingleton<WindowApplicationContext>();
            services.AddPinControls();
        }

        [STAThread]
        internal static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            using var ServiceProvider = serviceCollection.BuildServiceProvider();
            var context = ServiceProvider.GetService<WindowApplicationContext>();
            Application.Run(context);
        }
    }
}
