using Pinspaces.Core.Data;
using Pinspaces.Core.Interfaces;
using Pinspaces.Extensions;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pinspaces.Controls
{
    public partial class PinPanel : UserControl, INotifyPropertyChanged
    {
        private readonly DraggableController dragController;
        private readonly IPinControl pinControl;
        private bool isLoading = false;

        public PinPanel(IPinControl pinControl)
        {
            InitializeComponent();
            DataContext = this;

            this.pinControl = pinControl;
            PinContentControl = pinControl.ContentControl;
            pinControl.PropertyChanged += PinControl_PropertyChanged;

            LayoutUpdated += PinPanel_LayoutUpdated;
            SizeChanged += PinPanel_SizeChanged;

            dragController = new DraggableController(this);
            dragController.AttachMouseEvents(HeaderPanel);
            dragController.AttachMouseEvents(BodyPanel);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Color DefaultPinColor { get; set; }
        public Pin Pin { get; private set; }

        public string PinColor
        {
            get => Pin.Color;
            set
            {
                Pin.Color = value;
                NotifyPropertyChanged(nameof(PinColor));
                TextColor = ColorExtensions.FromHtmlString(Pin.Color, Brushes.Black.Color).TextColor().ToHtmlString();
                NotifyPropertyChanged(nameof(TextColor));
            }
        }

        public Control PinContentControl { get; private set; }

        public string TextColor { get; private set; }

        public string Title
        {
            get => Pin.Title;
            set
            {
                Pin.Title = value;
                NotifyPropertyChanged(nameof(Title));
            }
        }

        public void AddContextMenuItems(ContextMenu contextMenu)
        {
            pinControl.AddContextMenuItems(contextMenu);
        }

        public void LoadPin(Guid pinspaceId, Pin pin)
        {
            isLoading = true;
            try
            {
                Pin = pin;
                Height = pin.Height > 0 ? pin.Height : Height;
                PinColor = pin.Color ?? DefaultPinColor.ToHtmlString();
                Canvas.SetLeft(this, Math.Max(0, pin.Left));
                Title = pin.Title;
                Canvas.SetTop(this, Math.Max(0, pin.Top));
                Width = pin.Width > 0 ? pin.Width : Width;
                pinControl.LoadPin(pinspaceId, pin);
            }
            finally
            {
                isLoading = false;
            }
        }

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void PinControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        private void PinPanel_LayoutUpdated(object sender, EventArgs e)
        {
            if (isLoading)
            {
                return;
            }
            var newLeft = Canvas.GetLeft(this);
            var newTop = Canvas.GetTop(this);
            if (Pin.Left != newLeft)
            {
                Pin.Left = newLeft;
                NotifyPropertyChanged(nameof(Pin.Left));
            }
            if (Pin.Top != newTop)
            {
                Pin.Top = newTop;
                NotifyPropertyChanged(nameof(Pin.Top));
            }
        }

        private void PinPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (isLoading)
            {
                return;
            }
            if (Pin.Height != Height)
            {
                Pin.Height = Height;
                NotifyPropertyChanged(nameof(Pin.Height));
            }
            if (Pin.Width != Width)
            {
                Pin.Width = Width;
                NotifyPropertyChanged(nameof(Pin.Width));
            }
        }
    }
}
