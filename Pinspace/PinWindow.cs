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
        private readonly List<Type> cellTypes = new List<Type>();
        private Control contextControl;
        private Point targetPoint;

        public PinWindow()
        {
            InitializeComponent();

            GenerateNewCellControlsMenu();
        }

        public WindowApplicationContext WindowApplicationContext { get; set; }

        public PinWindowConfig Config()
        {
            var config = new PinWindowConfig();
            foreach (var control in Controls)
            {
                if (!(control is PinPanel pinPanel))
                {
                    continue;
                }
                config.Panels.Add(pinPanel.Config());
            }
            return config;
        }

        public void LoadConfig(PinWindowConfig config)
        {
            foreach (var panelConfig in config.Panels)
            {
                var typeName = "Pinspace.PinPanels." + panelConfig.GetType().Name.Replace("Config", "");
                var type = Type.GetType(typeName);
                var panel = CreateNewPinPanel(type);
                panel.LoadConfig(panelConfig);
            }
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            targetPoint = PointToClient(contextMenuStrip.Bounds.Location);
            contextControl = FindContextParent(contextMenuStrip.SourceControl);

            // Pinboard Window
            newCellMenuItem.Visible = contextControl is PinWindow;

            // Pinboard Cell
            renameCellMenuItem.Visible = contextControl is PinPanel;
            removeCellMenuItem.Visible = contextControl is PinPanel;
        }

        private PinPanel CreateNewPinPanel(Type pinPanelType)
        {
            var panel = Activator.CreateInstance(pinPanelType, null) as PinPanel;
            panel.Title = "New " + GetCellDisplayName(pinPanelType);
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

        private void GenerateNewCellControlsMenu()
        {
            var types = Assembly.GetAssembly(typeof(PinPanel)).GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(PinPanel)));
            foreach (var type in types)
            {
                var newMenuItem = new ToolStripMenuItem
                {
                    Text = GetCellDisplayName(type)
                };
                newMenuItem.Click += NewCellMenuItem_Click;
                newMenuItem.Tag = cellTypes.Count;
                newCellMenuItem.DropDownItems.Add(newMenuItem);
                cellTypes.Add(type);
            }
        }

        private string GetCellDisplayName(Type type)
        {
            return type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name;
        }

        private void NewCellMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            var cellType = cellTypes[(int)menuItem.Tag];
            var cell = CreateNewPinPanel(cellType);
            cell.Left = targetPoint.X;
            cell.Top = targetPoint.Y;
            Controls.Add(cell);
        }

        private void RemoveCellMenuItem_Click(object sender, EventArgs e)
        {
            Controls.Remove(contextControl);
            contextControl.Dispose();
        }

        private void RenameCellMenuItem_Click(object sender, EventArgs e)
        {
            var cell = contextControl as PinPanel;
            var title = cell.Title;
            if (this.ShowInputDialog("Rename", ref title) == DialogResult.OK)
            {
                cell.Title = title;
            }
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            WindowApplicationContext.SaveConfig();
        }
    }
}
