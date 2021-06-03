using Pinspaces.Core.Data;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Pinspaces.Controls
{
    public partial class PinWindowForm : Window, INotifyPropertyChanged
    {
        private readonly IDataRepository dataRepository;
        private readonly DebounceMethodExecutor updateFormLocationAndSizeMethodExecutor;
        private bool isLoading = false;
        private PinWindow pinWindow;

        public PinWindowForm(IDataRepository dataRepository, PinspacePanel pinspacePanel)
        {
            this.dataRepository = dataRepository;
            updateFormLocationAndSizeMethodExecutor = new(UpdateFormLocationAndSize, 1000);

            // Initialize Pin Window
            InitializeComponent();
            DataContext = this;
            Height = PinWindow.DefaultHeight;
            Width = PinWindow.DefaultWidth;
            PinspacePanel = pinspacePanel;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ActivePinspaceTitle => PinspacePanel.Title;
        public PinspacePanel PinspacePanel { get; private set; }
        public ObservableCollection<Pinspace> Pinspaces { get; private set; } = new();

        public void LoadWindow(PinWindow pinWindow)
        {
            isLoading = true;
            try
            {
                this.pinWindow = pinWindow;

                // Pin Window Settings
                Height = pinWindow.Height;
                Left = pinWindow.Left;
                Top = pinWindow.Top;
                Width = pinWindow.Width;

                if (pinWindow.IsMaximized)
                {
                    WindowState = WindowState.Maximized;
                }

                // Pinspace Settings
                Pinspaces.Clear();
                foreach (var pinspace in dataRepository.GetPinspaces())
                {
                    Pinspaces.Add(pinspace);
                }

                SwitchActivePinspace(pinWindow.ActivePinspaceId);
            }
            finally
            {
                isLoading = false;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            updateFormLocationAndSizeMethodExecutor.Dispose();
            base.OnClosed(e);
        }

        private void Form_LocationOrPositionChanged(object sender, EventArgs e)
        {
            if (isLoading)
            {
                return;
            }
            updateFormLocationAndSizeMethodExecutor.Execute();
        }

        private void NewPinspaceMenuItem_Click(object sender, EventArgs e)
        {
            var title = "Pinspace";
            if (InputDialog.ShowInputDialog("New Pinspace", "Enter a name for the new Pinspace:", ref title))
            {
                var pinspace = new Pinspace { Title = title };
                Pinspaces.Add(pinspace);
                dataRepository.UpdatePinspace(pinspace);
                BuildPinspaceDropDown();
                SwitchActivePinspace(pinspace.Id);
            }
        }

        private void PinspaceButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var contextMenu = button.ContextMenu;
            contextMenu.PlacementTarget = button;
            contextMenu.IsOpen = true;
        }

        private void PinspaceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SwitchActivePinspace((Guid)(sender as MenuItem).Tag);
        }

        private void SwitchActivePinspace(Guid pinspaceId)
        {
            var pinspace = Pinspaces.Where(p => p.Id.Equals(pinspaceId)).FirstOrDefault();
            if (pinspace == null)
            {
                return;
            }
            PinspacePanel.LoadPinspace(pinspaceId);
            pinWindow.ActivePinspaceId = pinspaceId;
            UpdatePinWindow();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActivePinspaceTitle)));
        }

        private void UpdateFormLocationAndSize()
        {
            var isMaximized = WindowState == WindowState.Maximized;
            pinWindow.Height = !isMaximized ? Height : PinWindow.DefaultHeight;
            pinWindow.IsMaximized = WindowState == WindowState.Maximized;
            pinWindow.Left = !isMaximized ? Left : Width / 2 - PinWindow.DefaultWidth / 2;
            pinWindow.Top = !isMaximized ? Top : Height / 2 - PinWindow.DefaultHeight / 2;
            pinWindow.Width = !isMaximized ? Width : PinWindow.DefaultWidth;
            UpdatePinWindow();
        }

        private void UpdatePinWindow()
        {
            if (isLoading)
            {
                return;
            }
            dataRepository.UpdatePinWindow(pinWindow);
        }
    }
}
