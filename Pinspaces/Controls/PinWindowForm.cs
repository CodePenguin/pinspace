using System;
using System.Windows.Forms;
using Pinspaces.Core.Data;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;

namespace Pinspaces.Controls
{
    public partial class PinWindowForm : Form
    {
        private readonly IDataContext dataContext;
        private readonly PinspacePanel pinspacePanel;
        private readonly DebounceMethodExecutor updateFormLocationAndSizeMethodExecutor;
        private bool isLoaded = false;
        private Pinspace pinspace;
        private PinWindow pinWindow;

        public PinWindowForm(IDataContext dataContext, PinspacePanel pinspacePanel)
        {
            this.dataContext = dataContext;

            InitializeComponent();

            this.pinspacePanel = pinspacePanel;
            pinspacePanel.AutoScroll = true;
            pinspacePanel.AutoScrollMargin = new System.Drawing.Size(10, 10);
            pinspacePanel.Dock = DockStyle.Fill;
            pinspacePanel.PropertiesChanged += PropertiesChanged;
            Controls.Add(pinspacePanel);

            Height = PinWindow.DefaultHeight;
            Width = PinWindow.DefaultWidth;

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
            var isMaximized = WindowState == FormWindowState.Maximized;
            pinWindow.Height = !isMaximized ? Height : PinWindow.DefaultHeight;
            pinWindow.IsMaximized = WindowState == FormWindowState.Maximized;
            pinWindow.Left = !isMaximized ? Left : Width / 2 - PinWindow.DefaultWidth / 2;
            pinWindow.Top = !isMaximized ? Top : Height / 2 - PinWindow.DefaultHeight / 2;
            pinWindow.Width = !isMaximized ? Width : PinWindow.DefaultWidth;
            SendPropertiesChangedNotification(this);
        }
    }
}
