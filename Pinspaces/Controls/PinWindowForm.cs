using System;
using System.Collections.Generic;
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
        private bool isLoading = false;
        private Dictionary<Guid, Pinspace> pinspaces = new();
        private PinWindow pinWindow;

        public PinWindowForm(IDataContext dataContext, PinspacePanel pinspacePanel)
        {
            this.dataContext = dataContext;
            updateFormLocationAndSizeMethodExecutor = new(UpdateFormLocationAndSize, 1000);

            // Initialize Pin Window
            InitializeComponent();
            Height = PinWindow.DefaultHeight;
            Width = PinWindow.DefaultWidth;

            // Initialize Pinspace Panel
            this.pinspacePanel = pinspacePanel;
            pinspacePanel.AutoScroll = true;
            pinspacePanel.AutoScrollMargin = new System.Drawing.Size(10, 10);
            pinspacePanel.Dock = DockStyle.Fill;
            Controls.Add(pinspacePanel);
        }

        public void LoadWindow(PinWindow pinWindow)
        {
            isLoading = true;
            try
            {
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
                pinspaces.Clear();
                foreach (var pinspace in dataContext.GetPinspaces())
                {
                    pinspaces.Add(pinspace.Id, pinspace);
                }
                BuildPinspaceDropDown();

                SwitchActivePinspace(pinWindow.ActivePinspaceId);
            }
            finally
            {
                isLoading = false;
            }
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

        private void BuildPinspaceDropDown()
        {
            pinspaceDropDownButton.DropDownItems.Clear();

            var newMenuItem = new ToolStripMenuItem { Text = "New..." };
            newMenuItem.Click += NewPinspaceMenuItem_Click;

            pinspaceDropDownButton.DropDownItems.AddRange(new ToolStripItem[] {
                newMenuItem,
                new ToolStripSeparator()
            });

            foreach (var pinspace in pinspaces.Values)
            {
                var switchMenuItem = new ToolStripMenuItem { Text = pinspace.Title };
                switchMenuItem.Tag = pinspace.Id;
                switchMenuItem.Click += (s, e) => SwitchActivePinspace(pinspace.Id);
                pinspaceDropDownButton.DropDownItems.Add(switchMenuItem);
            }
        }

        private void Form_LocationOrPositionChanged(object sender, EventArgs e)
        {
            if (isLoading)
            {
                return;
            }
            updateFormLocationAndSizeMethodExecutor.Execute();
        }

        private void NewPinspaceMenuItem_Click(object sender, EventArgs e)
        {
            var title = "Pinspace";
            if (FormExtensions.ShowInputDialog("New Pinspace", ref title) == DialogResult.OK)
            {
                var pinspace = new Pinspace { Title = title };
                dataContext.UpdatePinspace(pinspace);
                pinspaces.Add(pinspace.Id, pinspace);
                BuildPinspaceDropDown();
                SwitchActivePinspace(pinspace.Id);
            }
        }

        private void SwitchActivePinspace(Guid pinspaceId)
        {
            var pinspace = pinspaces[pinspaceId];
            pinspaceDropDownButton.Text = pinspace.Title;
            pinspacePanel.LoadPinspace(pinspaceId);
            pinWindow.ActivePinspaceId = pinspaceId;
            UpdatePinWindow();
        }

        private void UpdateFormLocationAndSize()
        {
            var isMaximized = WindowState == FormWindowState.Maximized;
            pinWindow.Height = !isMaximized ? Height : PinWindow.DefaultHeight;
            pinWindow.IsMaximized = WindowState == FormWindowState.Maximized;
            pinWindow.Left = !isMaximized ? Left : Width / 2 - PinWindow.DefaultWidth / 2;
            pinWindow.Top = !isMaximized ? Top : Height / 2 - PinWindow.DefaultHeight / 2;
            pinWindow.Width = !isMaximized ? Width : PinWindow.DefaultWidth;
            UpdatePinWindow();
        }

        private void UpdatePinWindow()
        {
            if (isLoading)
            {
                return;
            }
            dataContext.UpdatePinWindow(pinWindow);
        }
    }
}
