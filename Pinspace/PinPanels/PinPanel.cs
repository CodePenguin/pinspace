using Pinspace.Config;
using Pinspace.Extensions;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pinspace.PinPanels
{
    public class PinPanel : DraggablePanel
    {
        private Label titleLabel;
        private Panel titlePanel;

        public PinPanel()
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

        public virtual PinPanelConfig Config()
        {
            var config = Activator.CreateInstance(ConfigType(), null) as PinPanelConfig;
            config.Height = Height;
            config.Left = Left;
            config.Title = Title;
            config.Top = Top;
            config.Width = Width;
            return config;
        }

        public virtual void LoadConfig(PinPanelConfig config)
        {
            Height = config.Height;
            Left = config.Left;
            Title = config.Title;
            Top = config.Top;
            Width = config.Width;
        }

        protected virtual Type ConfigType()
        {
            return typeof(PinPanelConfig);
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
