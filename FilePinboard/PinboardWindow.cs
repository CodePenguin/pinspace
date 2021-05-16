using System;
using System.Drawing;
using System.Windows.Forms;

namespace FilePinboard
{
    public partial class PinboardWindow : Form
    {
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
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
