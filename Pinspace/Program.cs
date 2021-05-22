using Microsoft.Extensions.DependencyInjection;
using Pinspace.Data;
using Pinspace.Interfaces;
using System;
using System.Windows.Forms;

namespace Pinspace
{
    public static class Program
    {
        [STAThread]
        internal static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var context = BuildContainer().GetService<WindowApplicationContext>();
            Application.Run(context);
        }

        private static IServiceProvider BuildContainer()
        {
            var services = new ServiceCollection()
                .AddSingleton<IDataContext, JsonDataContext>()
                .AddSingleton<WindowApplicationContext>()
                .AddTransient<FormFactory>()
                .AddTransient<PinWindowForm>();
            return services.BuildServiceProvider();
        }
    }
}
