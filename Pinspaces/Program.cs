using Microsoft.Extensions.DependencyInjection;
using Pinspaces.Data;
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
            services.AddSingleton<WindowApplicationContext>();
            services.AddTransient<FormFactory>();
            services.AddTransient<PinWindowForm>();
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
