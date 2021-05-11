using Autofac;
using EasyTabs;
using System;

namespace FilePinboard
{
    public partial class FilePinboardWindowContainer : TitleBarTabs
    {
        // Store static ILifetimeScope factory because the window containers are created by EasyTabs
        public static Func<ILifetimeScope> LifetimeScopeFactory;

        // Maintain a ILifetimeScope for each window container so everything is disposed when the window closes
        private readonly ILifetimeScope scope;

        public FilePinboardWindowContainer()
        {
            InitializeComponent();

            scope = LifetimeScopeFactory().BeginLifetimeScope();
            Disposed += (s, e) =>
            {
                scope.Dispose();
            };

            AeroPeekEnabled = true;
            TabRenderer = new ChromeTabRenderer(this);
        }

        public override TitleBarTab CreateTab()
        {
            var pinboardForm = scope.Resolve<FormFactory>().CreateForm<PinboardForm>();
            pinboardForm.Text = "New Pinboard";
            return new TitleBarTab(this)
            {
                Content = pinboardForm
            };
        }
    }
}
