using EasyTabs;
using System;

namespace FilePinboard
{
    public partial class ApplicationContainer : TitleBarTabs
    {
        private readonly Func<PinboardForm> pinboardFormFactory;
        public ApplicationContainer(Func<PinboardForm> pinboardFormFactory)
        {
            this.pinboardFormFactory = pinboardFormFactory;
            InitializeComponent();

            AeroPeekEnabled = true;
            TabRenderer = new ChromeTabRenderer(this);
        }

        public override TitleBarTab CreateTab()
        {
            var pinboardForm = pinboardFormFactory();
            pinboardForm.Text = "New Pinboard";
            return new TitleBarTab(this)
            {
                Content = pinboardForm
            };
        }
    }
}
