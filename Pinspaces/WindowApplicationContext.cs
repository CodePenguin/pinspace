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
        private readonly FormFactory formFactory;
        private readonly List<PinWindowForm> windows = new();
        private readonly Dictionary<Form, IServiceScope> windowScopes = new();

        public WindowApplicationContext(FormFactory formFactory, IDataContext dataContext)
        {
            this.formFactory = formFactory;
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
            var form = formFactory.CreateForm<PinWindowForm>();
            windows.Add(form);
            form.FormClosed += Window_FormClosed;
            form.LoadWindow(pinWindow);
            form.Show();
        }

        private void Window_Disposed(object sender, EventArgs e)
        {
            var window = sender as Form;
            if (windowScopes.Remove(window, out var scope))
            {
                scope.Dispose();
            }
        }

        private void Window_FormClosed(object sender, FormClosedEventArgs e)
        {
            var window = sender as PinWindowForm;
            windows.Remove(window);
            window.Dispose();

            if (windows.Count == 0)
            {
                ExitThread();
            }
        }
    }
}
