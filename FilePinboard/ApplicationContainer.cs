using EasyTabs;
using System;

namespace FilePinboard
{
    public partial class ApplicationContainer : TitleBarTabs
    {
        public Func<PinboardForm> PinboardFormFactory { get; set; }
        public ApplicationContainer()
        {
            InitializeComponent();

            AeroPeekEnabled = true;
            TabRenderer = new ChromeTabRenderer(this);
        }

        public override TitleBarTab CreateTab()
        {
            var pinboardForm = PinboardFormFactory();
            pinboardForm.Text = "New Pinboard";
            return new TitleBarTab(this)
            {
                Content = pinboardForm
            };
        }
    }
}
