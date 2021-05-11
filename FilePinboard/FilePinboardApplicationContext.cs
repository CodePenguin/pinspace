using EasyTabs;

namespace FilePinboard
{
    internal class FilePinboardApplicationContext : TitleBarTabsApplicationContext
    {
        public FilePinboardApplicationContext(FilePinboardWindowContainer container)
        {
            container.Tabs.Add(container.CreateTab());
            container.SelectedTabIndex = 0;

            Start(container);
        }
    }
}
