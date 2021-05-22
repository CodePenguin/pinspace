using Pinspace.Data;
using Pinspace.Extensions;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pinspace.Pins
{
    public class PinPanel : DraggablePanel
    {
        protected Pin pin;
        private readonly Color defaultPinColor = Color.FromArgb(51, 122, 183);
        private Label titleLabel;
        private Panel titlePanel;

        public PinPanel()
        {
            InitializeControl();
            ContextMenuStripChanged += PinboardPanel_ContextMenuStripChanged;
        }

        public Color PinColor
        {
            get => BackColor;
            set
            {
                BackColor = value;
                titlePanel.BackColor = BackColor;
                titlePanel.ForeColor = BackColor.TextColor();
                pin.Color = BackColor.ToHtmlString();
            }
        }

        public string Title
        {
            get => titleLabel.Text;
            set
            {
                titleLabel.Text = value;
                pin.Title = value;
            }
        }

        public virtual void Load(Pin pin)
        {
            this.pin = pin;
            Height = pin.Height > 0 ? pin.Height : Height;
            PinColor = ColorExtensions.FromHtmlString(pin.Color, defaultPinColor);
            Left = pin.Left;
            Title = pin.Title;
            Top = pin.Top;
            Width = pin.Width > 0 ? pin.Width : Width;
        }

        public virtual Type PinType()
        {
            return typeof(Pin);
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

            Controls.Add(titlePanel);
        }

        protected override void OnMovedPanel()
        {
            pin.Left = Left;
            pin.Top = Top;
        }

        protected override void OnResizedPanel()
        {
            pin.Height = Height;
            pin.Width = Width;
        }

        private void PinboardPanel_ContextMenuStripChanged(object sender, EventArgs e)
        {
            titleLabel.ContextMenuStrip = ContextMenuStrip;
            titlePanel.ContextMenuStrip = ContextMenuStrip;
        }
    }
}
