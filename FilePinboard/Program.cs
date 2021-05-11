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
            builder.Register(c =>
            {
                FilePinboardWindowContainer.LifetimeScopeFactory = c.Resolve<Func<ILifetimeScope>>();
                return new FilePinboardWindowContainer();
            });
            builder.RegisterType<FilePinboardApplicationContext>();
            builder.RegisterType<FormFactory>();
            builder.RegisterType<PinboardForm>();
            return builder.Build();
        }

        [STAThread]
        internal static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var context = BuildContainer().Resolve<FilePinboardApplicationContext>();
            Application.Run(context);
        }
    }
}
