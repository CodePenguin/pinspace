using EasyTabs;

namespace FilePinboard
{
    internal class ApplicationContext : TitleBarTabsApplicationContext
    {
        public ApplicationContext(ApplicationContainer container)
        {
            container.Tabs.Add(container.CreateTab());
            container.SelectedTabIndex = 0;

            Start(container);
        }
    }
}
