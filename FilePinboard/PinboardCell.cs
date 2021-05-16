using System;
using System.Drawing;
using System.Windows.Forms;

namespace FilePinboard
{
    public class PinboardCell : DraggablePanel
    {
        private Label titleLabel;
        private Panel titlePanel;

        public PinboardCell()
        {
            InitializeControl();
            ContextMenuStripChanged += PinboardCell_ContextMenuStripChanged;
        }

        public Color PanelColor
        {
            get => BackColor;
            set
            {
                BackColor = value;
                titlePanel.BackColor = BackColor;
                titlePanel.ForeColor = BackColor.TextColor();
            }
        }

        public string Title
        {
            get => titleLabel.Text;
            set => titleLabel.Text = value;
        }

        protected virtual void InitializeControl()
        {
            Padding = new Padding(2);
            titlePanel = new Panel
            {
                Dock = DockStyle.Top,
                Padding = new Padding(4),
                Height = 24
            };

            titleLabel = new Label
            {
                AutoSize = true,
                Dock = DockStyle.Left,
                Text = "New",
                TextAlign = ContentAlignment.MiddleLeft,
            };

            titlePanel.Controls.Add(titleLabel);
            HandleDraggablePanelEvents(titleLabel);
            HandleDraggablePanelEvents(titlePanel);

            PanelColor = Color.FromArgb(51, 122, 183);
            Controls.Add(titlePanel);
        }

        private void PinboardCell_ContextMenuStripChanged(object sender, EventArgs e)
        {
            titleLabel.ContextMenuStrip = ContextMenuStrip;
            titlePanel.ContextMenuStrip = ContextMenuStrip;
        }
    }
}
