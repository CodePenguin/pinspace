using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Pinspaces.Data;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using Pinspaces.Pins;

namespace Pinspaces
{
    public partial class PinWindowForm : Form
    {
        private readonly int baseContextMenuItemCount;
        private readonly IDataContext dataContext;
        private readonly Color defaultPinColor = Color.FromArgb(51, 122, 183);
        private readonly List<Type> pinTypes = new();
        private readonly DebounceMethodExecutor updateFormLocationAndSizeMethodExecutor;
        private bool isLoaded = false;
        private Pinspace pinspace;
        private PinWindow pinWindow;
        private Point targetPoint;

        public PinWindowForm(IDataContext dataContext)
        {
            this.dataContext = dataContext;

            InitializeComponent();

            baseContextMenuItemCount = ContextMenuStrip.Items.Count;
            updateFormLocationAndSizeMethodExecutor = new(UpdateFormLocationAndSize, 1000);

            GenerateNewPinControlsMenu();
        }

        public WindowApplicationContext WindowApplicationContext { get; set; }

        public void LoadWindow(PinWindow pinWindow)
        {
            this.pinWindow = pinWindow;

            // Pin Window Settings
            BackColor = ColorExtensions.FromHtmlString(pinWindow.Color, SystemColors.Control);
            Height = pinWindow.Height;
            Left = pinWindow.Left;
            Top = pinWindow.Top;
            Width = pinWindow.Width;

            if (pinWindow.IsMaximized)
            {
                WindowState = FormWindowState.Maximized;
            }

            pinspace = dataContext.GetPinspace(pinWindow.ActivePinspaceId);

            // Pin Panels
            foreach (var pin in pinspace.Pins)
            {
                var typeName = "Pinspaces.Pins." + pin.GetType().Name + "Panel";
                var type = Type.GetType(typeName);
                if (type == null)
                {
                    throw new ArgumentException($"Unknown Pin Panel Type: {typeName}");
                }
                var panel = CreateNewPinPanel(type);
                panel.LoadPin(pin);
            }

            isLoaded = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                updateFormLocationAndSizeMethodExecutor.Dispose();
            }
            base.Dispose(disposing);
        }

        private static string GetPinDisplayName(Type type)
        {
            return type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name;
        }

        private void ChangeColorMenuItem_Click(object sender, EventArgs e)
        {
            var contextControl = GetContextControl();
            using var colorDialog = new ColorDialog
            {
                Color = contextControl.BackColor,
                FullOpen = true
            };
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                if (contextControl is PinWindowForm)
                {
                    BackColor = colorDialog.Color;
                    pinWindow.Color = colorDialog.Color.ToHtmlString();
                    SendPropertiesChangedNotification(this);
                }
                if (contextControl is PinPanel pinPanel)
                {
                    pinPanel.PinColor = colorDialog.Color;
                }
            }
        }

        private void ContextMenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            while (ContextMenuStrip.Items.Count > baseContextMenuItemCount)
            {
                ContextMenuStrip.Items.RemoveAt(baseContextMenuItemCount);
            }
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (contextMenuStrip.SourceControl is PinPanel)
            {
                // Do not show the context menu if coming from the panel itself
                e.Cancel = true;
                return;
            }

            targetPoint = PointToClient(contextMenuStrip.Bounds.Location);
            var contextControl = GetContextControl();

            // Pinboard Window
            newPinMenuItem.Visible = contextControl is PinWindowForm;
            changeColorMenuItem.Visible = (contextControl is PinWindowForm) || (contextControl is PinPanel);

            // Pinboard Pin
            renamePinMenuItem.Visible = contextControl is PinPanel;
            removePinMenuItem.Visible = contextControl is PinPanel;

            if (contextControl is PinPanel pinPanel)
            {
                pinPanel.AddContextMenuItems(contextMenuStrip);
            }

            if (contextMenuStrip.Items.Count > baseContextMenuItemCount)
            {
                contextMenuStrip.Items.Insert(baseContextMenuItemCount, new ToolStripSeparator());
            }
        }

        private PinPanel CreateNewPinPanel(Type pinType)
        {
            var panel = Activator.CreateInstance(pinType, null) as PinPanel;
            panel.ContextMenuStrip = contextMenuStrip;
            panel.PropertiesChanged += PropertiesChanged;
            Controls.Add(panel);
            return panel;
        }

        private void Form_LocationChanged(object sender, EventArgs e)
        {
            updateFormLocationAndSizeMethodExecutor.Execute();
        }

        private void Form_ResizeEnd(object sender, EventArgs e)
        {
            updateFormLocationAndSizeMethodExecutor.Execute();
        }

        private void GenerateNewPinControlsMenu()
        {
            var types = Assembly.GetAssembly(typeof(PinPanel)).GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(PinPanel)));
            foreach (var type in types)
            {
                var newMenuItem = new ToolStripMenuItem
                {
                    Text = GetPinDisplayName(type)
                };
                newMenuItem.Click += NewPinMenuItem_Click;
                newMenuItem.Tag = pinTypes.Count;
                newPinMenuItem.DropDownItems.Add(newMenuItem);
                pinTypes.Add(type);
            }
        }

        private Control GetContextControl()
        {
            var control = contextMenuStrip.SourceControl;
            while (control != null && !(control is PinPanel) && !(control is PinWindowForm))
            {
                control = control.Parent;
            }
            return control;
        }

        private void NewPinMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            var pinType = pinTypes[(int)menuItem.Tag];
            var pinPanel = CreateNewPinPanel(pinType);
            var pin = Activator.CreateInstance(pinPanel.PinType(), null) as Pin;
            pin.Color = defaultPinColor.ToHtmlString();
            pin.Height = pinPanel.Height;
            pin.Left = targetPoint.X;
            pin.Title = "New " + GetPinDisplayName(pinType);
            pin.Top = targetPoint.Y;
            pin.Width = pinPanel.Width;
            pinspace.Pins.Add(pin);
            pinPanel.LoadPin(pin);
            SendPropertiesChangedNotification(pinPanel);
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
            dataContext.UpdatePinspace(pinspace);
        }

        private void RemovePinMenuItem_Click(object sender, EventArgs e)
        {
            var contextControl = GetContextControl();
            Controls.Remove(contextControl);
            contextControl.Dispose();
        }

        private void RenamePinMenuItem_Click(object sender, EventArgs e)
        {
            var pinPanel = GetContextControl() as PinPanel;
            var title = pinPanel.Title;
            if (this.ShowInputDialog("Rename", ref title) == DialogResult.OK)
            {
                pinPanel.Title = title;
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
