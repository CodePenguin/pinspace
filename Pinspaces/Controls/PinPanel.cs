using Pinspaces.Core.Controls;
using Pinspaces.Core.Data;
using Pinspaces.Core.Extensions;
using Pinspaces.Core.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pinspaces.Controls
{
    public class PinPanel : DraggablePanel, INotifyPropertiesChanged
    {
        private readonly PinControl pinControl;
        private bool isLoaded = false;
        private Label titleLabel;
        private Panel titlePanel;

        public PinPanel(PinControl pinControl)
        {
            this.pinControl = pinControl;

            InitializeControl();
            ContextMenuStripChanged += PinboardPanel_ContextMenuStripChanged;
        }

        public event INotifyPropertiesChanged.PropertiesChangedEventHandler PropertiesChanged;

        public Pin Pin { get; private set; }

        public Color PinColor
        {
            get => BackColor;
            set
            {
                BackColor = value;
                titlePanel.BackColor = BackColor;
                titlePanel.ForeColor = BackColor.TextColor();
                Pin.Color = BackColor.ToHtmlString();
                SendPropertiesChangedNotification();
            }
        }

        public string Title
        {
            get => titleLabel.Text;
            set
            {
                titleLabel.Text = value;
                Pin.Title = value;
                SendPropertiesChangedNotification();
            }
        }

        public void AddContextMenuItems(ContextMenuStrip contextMenu)
        {
            pinControl.AddContextMenuItems(contextMenu);
        }

        public void InitializeControl()
        {
            Padding = new Padding(2);

            pinControl.Dock = DockStyle.Fill;
            pinControl.PropertiesChanged += PropertiesChanged;
            Controls.Add(pinControl);

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

        public void LoadPin(Pin pin)
        {
            isLoaded = false;
            Pin = pin;
            Height = pin.Height > 0 ? pin.Height : Height;
            PinColor = ColorExtensions.FromHtmlString(pin.Color, BackColor);
            Left = pin.Left;
            Title = pin.Title;
            Top = pin.Top;
            Width = pin.Width > 0 ? pin.Width : Width;
            pinControl.LoadPin(pin);
            isLoaded = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                titleLabel.Dispose();
                titlePanel.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            if (!isLoaded)
            {
                return;
            }
            Pin.Left = Left;
            Pin.Top = Top;
            SendPropertiesChangedNotification();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!isLoaded)
            {
                return;
            }
            Pin.Height = Height;
            Pin.Width = Width;
            SendPropertiesChangedNotification();
        }

        protected void SendPropertiesChangedNotification()
        {
            if (!isLoaded)
            {
                return;
            }
            PropertiesChanged?.Invoke(this, new EventArgs());
        }

        private void PinboardPanel_ContextMenuStripChanged(object sender, EventArgs e)
        {
            titleLabel.ContextMenuStrip = ContextMenuStrip;
            titlePanel.ContextMenuStrip = ContextMenuStrip;
        }
    }
}
