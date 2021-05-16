using Autofac;
using System;
using System.Windows.Forms;

namespace FilePinboard
{
    static class Program
    {
        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<WindowApplicationContext>().SingleInstance();
            builder.RegisterType<FormFactory>();
            builder.RegisterType<PinboardWindow>();
            return builder.Build();
        }

        [STAThread]
        internal static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var context = BuildContainer().Resolve<WindowApplicationContext>();
            Application.Run(context);
        }
    }
}
