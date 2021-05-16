using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace FilePinboard
{
    public partial class PinboardWindow : Form
    {
        private List<Type> cellTypes = new List<Type>();
        private Point targetPoint;

        public PinboardWindow()
        {
            InitializeComponent();

            Controls.Add(new TextBoxCell
            {
                Left = 10,
                Top = 50,
                PanelColor = Color.FromArgb(217, 237, 247),
                Title = "Basic Panel"
            });

            Controls.Add(new FileListCell
            {
                Left = 300,
                Top = 50,
                Width = 400,
                Height = 200,
                Title = "File List Panel"
            });

            GenerateNewCellControlsMenu();
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            targetPoint = PointToClient(contextMenuStrip.Bounds.Location);
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void GenerateNewCellControlsMenu()
        {
            foreach (var type in Assembly.GetAssembly(typeof(PinboardCell)).GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(PinboardCell))))
            {
                var newMenuItem = new ToolStripMenuItem
                {
                    Text = GetCellDisplayName(type)
                };
                newMenuItem.Click += NewCellMenuItem_Click;
                newMenuItem.Tag = cellTypes.Count;
                NewMenuItem.DropDownItems.Add(newMenuItem);
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
            var type = cellTypes[(int)menuItem.Tag];

            var cell = Activator.CreateInstance(type, null) as PinboardCell;
            cell.Title = "New " + GetCellDisplayName(type);
            cell.Left = targetPoint.X;
            cell.Top = targetPoint.Y;
            Controls.Add(cell);
        }
    }
}
