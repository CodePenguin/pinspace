using Autofac;
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

            var context = BuildContainer().Resolve<WindowApplicationContext>();
            Application.Run(context);
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<WindowApplicationContext>().SingleInstance();
            builder.RegisterType<FormFactory>();
            builder.RegisterType<PinWindow>();
            return builder.Build();
        }
    }
}
