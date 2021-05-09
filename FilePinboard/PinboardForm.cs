using System;
using System.Windows.Forms;

namespace FilePinboard
{
    public partial class PinboardForm : Form
    {
        public PinboardForm()
        {
            InitializeComponent();

            addCellToolStripMenuItem.PerformClick();
        }

        private void AddCellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableLayoutPanel.Controls.Add(new FileListCell { Dock = DockStyle.Fill });
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
