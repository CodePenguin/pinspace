using Pinspaces.Data;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pinspaces.Pins
{
    public abstract class PinPanel : DraggablePanel, INotifyPropertiesChanged
    {
        protected Pin pin;
        private readonly DebounceMethodExecutor sendPropertiesNotificationMethodExecutor;
        private Label titleLabel;
        private Panel titlePanel;

        public PinPanel()
        {
            InitializeControl();
            ContextMenuStripChanged += PinboardPanel_ContextMenuStripChanged;
            sendPropertiesNotificationMethodExecutor = new(() => PropertiesChanged?.Invoke(this, new EventArgs()), 1000);
        }

        public event INotifyPropertiesChanged.PropertiesChangedEventHandler PropertiesChanged;

        public Color PinColor
        {
            get => BackColor;
            set
            {
                BackColor = value;
                titlePanel.BackColor = BackColor;
                titlePanel.ForeColor = BackColor.TextColor();
                pin.Color = BackColor.ToHtmlString();
                SendPropertiesChangedNotification();
            }
        }

        public string Title
        {
            get => titleLabel.Text;
            set
            {
                titleLabel.Text = value;
                pin.Title = value;
                SendPropertiesChangedNotification();
            }
        }

        public virtual void AddContextMenuItems(ContextMenuStrip contextMenu)
        {
            // Override to add additional context menu items
        }

        public virtual void LoadPin(Pin pin)
        {
            this.pin = pin;
            Height = pin.Height > 0 ? pin.Height : Height;
            PinColor = ColorExtensions.FromHtmlString(pin.Color, BackColor);
            Left = pin.Left;
            Title = pin.Title;
            Top = pin.Top;
            Width = pin.Width > 0 ? pin.Width : Width;
        }

        public abstract Type PinType();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                sendPropertiesNotificationMethodExecutor.Dispose();
                titleLabel.Dispose();
                titlePanel.Dispose();
            }
            base.Dispose(disposing);
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

        protected void SendPropertiesChangedNotification()
        {
            sendPropertiesNotificationMethodExecutor.Execute();
        }

        private void PinboardPanel_ContextMenuStripChanged(object sender, EventArgs e)
        {
            titleLabel.ContextMenuStrip = ContextMenuStrip;
            titlePanel.ContextMenuStrip = ContextMenuStrip;
        }
    }
}
