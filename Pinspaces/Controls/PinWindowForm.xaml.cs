using Pinspaces.Core.Data;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Pinspaces.Controls
{
    public partial class PinWindowForm : Window
    {
        private readonly IDataRepository dataRepository;
        private readonly Dictionary<Guid, Pinspace> pinspaces = new();
        private readonly DebounceMethodExecutor updateFormLocationAndSizeMethodExecutor;
        private bool isLoading = false;
        private PinWindow pinWindow;

        public PinWindowForm(IDataRepository dataRepository, PinspacePanel pinspacePanel)
        {
            this.dataRepository = dataRepository;
            updateFormLocationAndSizeMethodExecutor = new(UpdateFormLocationAndSize, 1000);

            // Initialize Pin Window
            InitializeComponent();
            DataContext = this;
            Height = PinWindow.DefaultHeight;
            Width = PinWindow.DefaultWidth;
            PinspacePanel = pinspacePanel;
        }

        public PinspacePanel PinspacePanel { get; private set; }

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
                    WindowState = WindowState.Maximized;
                }

                // Pinspace Settings
                pinspaces.Clear();
                foreach (var pinspace in dataRepository.GetPinspaces())
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

        protected override void OnClosed(EventArgs e)
        {
            updateFormLocationAndSizeMethodExecutor.Dispose();
            base.OnClosed(e);
        }

        private void BuildPinspaceDropDown()
        {
            //FIX!!
            //pinspaceDropDownButton.DropDownItems.Clear();

            //var newMenuItem = new ToolStripMenuItem { Text = "New..." };
            //newMenuItem.Click += NewPinspaceMenuItem_Click;

            //pinspaceDropDownButton.DropDownItems.AddRange(new ToolStripItem[] {
            //    newMenuItem,
            //    new ToolStripSeparator()
            //});

            //foreach (var pinspace in pinspaces.Values)
            //{
            //    var switchMenuItem = new ToolStripMenuItem { Text = pinspace.Title };
            //    switchMenuItem.Tag = pinspace.Id;
            //    switchMenuItem.Click += (s, e) => SwitchActivePinspace(pinspace.Id);
            //    pinspaceDropDownButton.DropDownItems.Add(switchMenuItem);
            //}
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
            //FIX!!!
            //if (FormExtensions.ShowInputDialog("New Pinspace", ref title) == DialogResult.OK)
            //{
            //    var pinspace = new Pinspace { Title = title };
            //    dataRepository.UpdatePinspace(pinspace);
            //    pinspaces.Add(pinspace.Id, pinspace);
            //    BuildPinspaceDropDown();
            //    SwitchActivePinspace(pinspace.Id);
            //}
        }

        private void SwitchActivePinspace(Guid pinspaceId)
        {
            var pinspace = pinspaces[pinspaceId];
            //FIX!!pinspaceDropDownButton.Text = pinspace.Title;
            PinspacePanel.LoadPinspace(pinspaceId);
            pinWindow.ActivePinspaceId = pinspaceId;
            UpdatePinWindow();
        }

        private void UpdateFormLocationAndSize()
        {
            var isMaximized = WindowState == WindowState.Maximized;
            pinWindow.Height = !isMaximized ? Height : PinWindow.DefaultHeight;
            pinWindow.IsMaximized = WindowState == WindowState.Maximized;
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
            dataRepository.UpdatePinWindow(pinWindow);
        }
    }
}
