using Microsoft.Extensions.DependencyInjection;
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
                .AddSingleton<WindowApplicationContext>()
                .AddTransient<FormFactory>()
                .AddTransient<PinWindow>();
            return services.BuildServiceProvider();
        }
    }
}
