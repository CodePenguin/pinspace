using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Pinspaces.Extensions
{
    public class WindowFactory
    {
        private readonly IServiceScopeFactory scopeFactory;

        public WindowFactory(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public TWindow CreateForm<TWindow>() where TWindow : Window
        {
            var formScope = scopeFactory.CreateScope();
            var form = formScope.ServiceProvider.GetService<TWindow>();

            form.Closed += (s, e) =>
            {
                formScope.Dispose();
            };

            return form;
        }
    }
}
