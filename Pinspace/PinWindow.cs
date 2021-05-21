using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Pinspace.Config;
using Pinspace.Extensions;
using Pinspace.PinPanels;

namespace Pinspace
{
    public partial class PinWindow : Form
    {
        private readonly List<Type> pinTypes = new List<Type>();
        private Control contextControl;
        private Point targetPoint;

        public PinWindow()
        {
            InitializeComponent();

            GenerateNewPinControlsMenu();
        }

        public WindowApplicationContext WindowApplicationContext { get; set; }

        public PinWindowConfig Config()
        {
            var config = new PinWindowConfig();
            // Pin Window Settings
            config.Color = (BackColor == SystemColors.Control) ? "" : BackColor.ToHtmlString();

            // Pin Panels
            foreach (var control in Controls)
            {
                if (control is not PinPanel pinPanel)
                {
                    continue;
                }
                config.Panels.Add(pinPanel.Config());
            }
            return config;
        }

        public void LoadConfig(PinWindowConfig config)
        {
            // Pin Window Settings
            BackColor = ColorExtensions.FromHtmlString(config.Color, SystemColors.Control);

            // Pin Panels
            foreach (var panelConfig in config.Panels)
            {
                var typeName = "Pinspace.PinPanels." + panelConfig.GetType().Name.Replace("Config", "");
                var type = Type.GetType(typeName);
                var panel = CreateNewPinPanel(type);
                panel.LoadConfig(panelConfig);
            }
        }

        private void ChangeColorMenuItem_Click(object sender, EventArgs e)
        {
            using var colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                if (contextControl is PinWindow pinWindow)
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
            contextControl = FindContextParent(contextMenuStrip.SourceControl);

            // Pinboard Window
            newPinMenuItem.Visible = contextControl is PinWindow;
            changeColorMenuItem.Visible = (contextControl is PinWindow) || (contextControl is PinPanel);

            // Pinboard Pin
            renamePinMenuItem.Visible = contextControl is PinPanel;
            removePinMenuItem.Visible = contextControl is PinPanel;
        }

        private PinPanel CreateNewPinPanel(Type pinPanelType)
        {
            var panel = Activator.CreateInstance(pinPanelType, null) as PinPanel;
            panel.Title = "New " + GetPinDisplayName(pinPanelType);
            panel.ContextMenuStrip = contextMenuStrip;
            Controls.Add(panel);
            return panel;
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private Control FindContextParent(Control control)
        {
            while (control != null && !(control is PinPanel) && !(control is PinWindow))
            {
                control = control.Parent;
            }
            return control;
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

        private string GetPinDisplayName(Type type)
        {
            return type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name;
        }

        private void NewPinMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            var pinType = pinTypes[(int)menuItem.Tag];
            var pinPanel = CreateNewPinPanel(pinType);
            pinPanel.Left = targetPoint.X;
            pinPanel.Top = targetPoint.Y;
            Controls.Add(pinPanel);
        }

        private void RemovePinMenuItem_Click(object sender, EventArgs e)
        {
            Controls.Remove(contextControl);
            contextControl.Dispose();
        }

        private void RenamePinMenuItem_Click(object sender, EventArgs e)
        {
            var pinPanel = contextControl as PinPanel;
            var title = pinPanel.Title;
            if (this.ShowInputDialog("Rename", ref title) == DialogResult.OK)
            {
                pinPanel.Title = title;
            }
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            WindowApplicationContext.SaveConfig();
        }
    }
}
