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
        private readonly IDataContext dataContext;
        private readonly List<Type> pinTypes = new();
        private Pinspace pinspace;
        private PinWindow pinWindow;
        private Point targetPoint;

        public PinWindowForm(IDataContext dataContext)
        {
            this.dataContext = dataContext;

            InitializeComponent();

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
        }

        private static string GetPinDisplayName(Type type)
        {
            return type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name;
        }

        private void ChangeColorMenuItem_Click(object sender, EventArgs e)
        {
            var contextControl = GetContextControl();
            using var colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                if (contextControl is PinWindowForm pinWindow)
                {
                    pinWindow.BackColor = colorDialog.Color;
                }
                if (contextControl is PinPanel pinPanel)
                {
                    pinPanel.PinColor = colorDialog.Color;
                }
            }
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            targetPoint = PointToClient(contextMenuStrip.Bounds.Location);
            var contextControl = GetContextControl();

            // Pinboard Window
            newPinMenuItem.Visible = contextControl is PinWindowForm;
            changeColorMenuItem.Visible = (contextControl is PinWindowForm) || (contextControl is PinPanel);

            // Pinboard Pin
            renamePinMenuItem.Visible = contextControl is PinPanel;
            removePinMenuItem.Visible = contextControl is PinPanel;
        }

        private PinPanel CreateNewPinPanel(Type pinType)
        {
            var panel = Activator.CreateInstance(pinType, null) as PinPanel;
            panel.ContextMenuStrip = contextMenuStrip;
            Controls.Add(panel);
            return panel;
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
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
            pin.Height = pinPanel.Height;
            pin.Left = targetPoint.X;
            pin.Title = "New " + GetPinDisplayName(pinType);
            pin.Top = targetPoint.Y;
            pin.Width = pinPanel.Width;
            pinspace.Pins.Add(pin);
            pinPanel.LoadPin(pin);
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

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            pinWindow.ActivePinspaceId = pinspace.Id;
            pinWindow.Color = (BackColor == SystemColors.Control) ? "" : BackColor.ToHtmlString();
            pinWindow.Height = Height;
            pinWindow.IsMaximized = WindowState == FormWindowState.Maximized;
            pinWindow.Left = Left;
            pinWindow.Top = Top;
            pinWindow.Width = Width;

            dataContext.UpdatePinspace(pinspace);
            dataContext.UpdatePinWindow(pinWindow);
        }
    }
}
