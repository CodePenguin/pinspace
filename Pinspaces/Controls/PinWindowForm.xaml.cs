using Pinspaces.Core.Data;
using Pinspaces.Core.Interfaces;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Pinspaces.Controls
{
    public partial class PinWindowForm : Window
    {
        private readonly IDataRepository dataRepository;
        private readonly IDelayedAction delayedUpdateFormLocationAndSizeAction;
        private int basePinspaceButtonContextMenuItemCount;
        private bool isLoading = false;
        private PinWindow pinWindow;
        private bool reloadPinspaceButtonContextMenu = true;

        public PinWindowForm(IDataRepository dataRepository, PinspacePanel pinspacePanel, IDelayedActionFactory delayedActionFactory)
        {
            this.dataRepository = dataRepository;
            delayedUpdateFormLocationAndSizeAction = delayedActionFactory.Debounce(UpdateFormLocationAndSize, 1000);

            // Initialize Pin Window
            InitializeComponent();
            DataContext = this;
            basePinspaceButtonContextMenuItemCount = PinspaceButton.ContextMenu.Items.Count;
            Height = PinWindow.DefaultHeight;
            Width = PinWindow.DefaultWidth;
            PinspacePanel = pinspacePanel;
            PinspacePanel.PropertyChanged += PinspacePanel_PropertyChanged;
            Pinspaces.CollectionChanged += Pinspaces_CollectionChanged;
        }

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
            delayedUpdateFormLocationAndSizeAction.Stop();
            base.OnClosed(e);
        }

        private void Form_LocationOrPositionChanged(object sender, EventArgs e)
        {
            if (isLoading)
            {
                return;
            }
            delayedUpdateFormLocationAndSizeAction.Execute();
        }

        private void NewPinspaceMenuItem_Click(object sender, EventArgs e)
        {
            var title = "Pinspace";
            if (InputDialog.ShowInputDialog("New Pinspace", "Enter a name for the new Pinspace:", ref title))
            {
                var pinspace = new Pinspace { Title = title };
                Pinspaces.Add(pinspace);
                dataRepository.UpdatePinspace(pinspace);
                SwitchActivePinspace(pinspace.Id);
            }
        }

        private void PinspaceButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var contextMenu = button.ContextMenu;

            if (reloadPinspaceButtonContextMenu)
            {
                reloadPinspaceButtonContextMenu = false;
                while (contextMenu.Items.Count > basePinspaceButtonContextMenuItemCount)
                {
                    contextMenu.Items.RemoveAt(basePinspaceButtonContextMenuItemCount);
                }
                foreach (var pinspace in Pinspaces)
                {
                    var menuItem = new MenuItem { Header = pinspace.Title, Tag = pinspace.Id };
                    menuItem.Click += PinspaceMenuItem_Click;
                    contextMenu.Items.Add(menuItem);
                }
            }

            contextMenu.PlacementTarget = button;
            contextMenu.IsOpen = true;
        }

        private void PinspaceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SwitchActivePinspace((Guid)(sender as MenuItem).Tag);
        }

        private void PinspacePanel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (isLoading)
            {
                return;
            }
            var pinspace = dataRepository.GetPinspace(pinWindow.ActivePinspaceId);
            Pinspaces.FirstOrDefault(p => p.Id.Equals(pinWindow.ActivePinspaceId))?.Assign(pinspace, out _);
        }

        private void Pinspaces_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            reloadPinspaceButtonContextMenu = true;
        }

        private void SwitchActivePinspace(Guid pinspaceId)
        {
            var pinspace = Pinspaces.Where(p => p.Id.Equals(pinspaceId)).FirstOrDefault();
            if (pinspace == null)
            {
                return;
            }
            pinWindow.ActivePinspaceId = pinspaceId;
            PinspacePanel.LoadPinspace(pinspaceId);
            UpdatePinWindow();
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
