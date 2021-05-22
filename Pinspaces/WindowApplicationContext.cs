using Microsoft.Extensions.DependencyInjection;
using Pinspaces.Data;
using Pinspaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Pinspaces
{
    public class WindowApplicationContext : ApplicationContext
    {
        private readonly IDataContext dataContext;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly List<PinWindowForm> windows = new();
        private readonly Dictionary<Form, IServiceScope> windowScopes = new();

        public WindowApplicationContext(IServiceScopeFactory scopeFactory, IDataContext dataContext)
        {
            this.scopeFactory = scopeFactory;
            this.dataContext = dataContext;
            LoadData();
        }

        public void LoadData()
        {
            var windows = dataContext.GetPinWindows();
            foreach (var window in windows)
            {
                NewWindow(window);
            }
        }

        public void NewWindow(PinWindow pinWindow)
        {
            var windowScope = scopeFactory.CreateScope();
            var form = windowScope.ServiceProvider.GetService<PinWindowForm>();
            windows.Add(form);
            windowScopes.Add(form, windowScope);
            form.Disposed += Window_Disposed;
            form.FormClosed += Window_FormClosed;
            form.WindowApplicationContext = this;
            form.LoadWindow(pinWindow);
            form.Show();
        }

        private void Window_Disposed(object sender, EventArgs e)
        {
            var window = sender as Form;
            var scope = windowScopes[window];
            windowScopes.Remove(window);
            scope.Dispose();
        }

        private void Window_FormClosed(object sender, FormClosedEventArgs e)
        {
            var window = sender as PinWindowForm;
            windows.Remove(window);

            if (windows.Count == 0)
            {
                ExitThread();
            }
        }
    }
}
