using System;
using System.Windows.Forms;
using Pinspaces.Data;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;

namespace Pinspaces
{
    public partial class PinWindowForm : Form
    {
        private readonly IDataContext dataContext;
        private readonly DebounceMethodExecutor updateFormLocationAndSizeMethodExecutor;
        private bool isLoaded = false;
        private Pinspace pinspace;
        private PinWindow pinWindow;

        public PinWindowForm(IDataContext dataContext)
        {
            this.dataContext = dataContext;

            InitializeComponent();
            pinspacePanel.PropertiesChanged += PropertiesChanged;

            updateFormLocationAndSizeMethodExecutor = new(UpdateFormLocationAndSize, 1000);
        }

        public void LoadWindow(PinWindow pinWindow)
        {
            isLoaded = false;

            this.pinWindow = pinWindow;

            // Pin Window Settings
            Height = pinWindow.Height;
            Left = pinWindow.Left;
            Top = pinWindow.Top;
            Width = pinWindow.Width;

            if (pinWindow.IsMaximized)
            {
                WindowState = FormWindowState.Maximized;
            }

            // Pinspace Settings
            pinspace = dataContext.GetPinspace(pinWindow.ActivePinspaceId);
            pinspacePanel.LoadPinspace(pinspace);

            isLoaded = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                updateFormLocationAndSizeMethodExecutor.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Form_LocationOrPositionChanged(object sender, EventArgs e)
        {
            if (!isLoaded)
            {
                return;
            }
            updateFormLocationAndSizeMethodExecutor.Execute();
        }

        private void PropertiesChanged(object sender, EventArgs e)
        {
            if (!isLoaded)
            {
                return;
            }
            if (sender is PinWindowForm)
            {
                dataContext.UpdatePinWindow(pinWindow);
            }
            if (sender is PinspacePanel)
            {
                dataContext.UpdatePinspace(pinspace);
            }
        }

        private void SendPropertiesChangedNotification(object sender)
        {
            PropertiesChanged(sender, new EventArgs());
        }

        private void UpdateFormLocationAndSize()
        {
            pinWindow.Height = Height;
            pinWindow.IsMaximized = WindowState == FormWindowState.Maximized;
            pinWindow.Left = Left;
            pinWindow.Top = Top;
            pinWindow.Width = Width;
            SendPropertiesChangedNotification(this);
        }
    }
}
