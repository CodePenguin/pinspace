using Autofac;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FilePinboard
{
    internal class WindowApplicationContext : ApplicationContext
    {
        private readonly List<Form> windowForms = new List<Form>();
        private readonly ILifetimeScope lifetimeScope;
        private readonly Dictionary<Form, ILifetimeScope> windowScopes = new Dictionary<Form, ILifetimeScope>();

        public WindowApplicationContext(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
            NewWindow();
        }

        public void NewWindow()
        {
            var windowScope = lifetimeScope.BeginLifetimeScope();
            var window = lifetimeScope.Resolve<PinboardForm>();
            windowForms.Add(window);
            windowScopes.Add(window, windowScope);
            window.Disposed += Window_Disposed;
            window.FormClosed += Window_FormClosed;
            window.Show();
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
            var window = sender as Form;
            windowForms.Remove(window);

            if (windowForms.Count == 0)
            {
                ExitThread();
            }
        }
    }
}
