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
            builder.RegisterType<PinboardForm>();
            return builder.Build();
        }

        [STAThread]
        internal static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = BuildContainer().Resolve<PinboardForm>();
            Application.Run(mainForm);
        }
    }
}
