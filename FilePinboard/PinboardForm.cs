using System;
using System.Windows.Forms;

namespace FilePinboard
{
    public partial class PinboardForm : Form
    {
        public PinboardForm()
        {
            InitializeComponent();

            Controls.Add(new DraggablePanel
            {
                BackColor = System.Drawing.Color.Blue
            });

            Controls.Add(new DraggablePanel
            {
                BackColor = System.Drawing.Color.Green
            });
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
