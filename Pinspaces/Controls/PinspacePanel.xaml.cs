using Pinspaces.Core.Data;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;

namespace Pinspaces.Controls
{
    public partial class PinspacePanel : UserControl, INotifyPropertyChanged, IDisposable
    {
        private readonly int baseContextMenuItemCount;
        private readonly IDataRepository dataRepository;
        private readonly Color defaultPinColor = Color.FromArgb(255, 51, 122, 183);
        private readonly Dictionary<Guid, Pin> pendingPinChanges = new();
        private readonly IPinFactory pinFactory;
        private readonly DebounceMethodExecutor processPendingPinChangesMethodExecutor;
        private readonly DebounceMethodExecutor updateCanvasSizeMethodExecutor;
        private FrameworkElement contextElement;
        private bool disposedValue;
        private bool isLoading;
        private Pinspace pinspace;
        private Point targetPoint;

        public PinspacePanel(IDataRepository dataRepository, IPinFactory pinFactory)
        {
            InitializeComponent();
            DataContext = this;

            this.dataRepository = dataRepository;
            this.pinFactory = pinFactory;
            processPendingPinChangesMethodExecutor = new(ProcessPendingPinChanges, 5000, Dispatcher);
            updateCanvasSizeMethodExecutor = new(UpdateCanvasSize, 100, Dispatcher);

            baseContextMenuItemCount = canvas.ContextMenu.Items.Count;
            GenerateNewPinControlsMenu();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string BackgroundColor
        {
            get => pinspace.Color;
            set
            {
                pinspace.Color = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BackgroundColor)));
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void LoadPinspace(Guid pinspaceId)
        {
            isLoading = true;
            try
            {
                pinspace = dataRepository.GetPinspace(pinspaceId);

                canvas.Children.Clear();
                foreach (var pin in dataRepository.GetPins(pinspaceId))
                {
                    AddPinPanel(pin);
                }
            }
            finally
            {
                isLoading = false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    processPendingPinChangesMethodExecutor.Dispose();
                    updateCanvasSizeMethodExecutor.Dispose();
                }
                disposedValue = true;
            }
        }

        private static FrameworkElement GetContextElement(object source)
        {
            var element = source as FrameworkElement;
            while (element != null && !(element is PinPanel) && !(element is PinspacePanel))
            {
                element = element.Parent as FrameworkElement;
            }
            return element;
        }

        private static void RenamePin(PinPanel pinPanel)
        {
            var title = pinPanel.Title;
            if (InputDialog.ShowInputDialog("Rename", "Enter the new name:", ref title))
            {
                pinPanel.Title = title;
            }
        }

        private void AddPinPanel(Pin pin)
        {
            var pinControl = pinFactory.NewPinControl(pin.GetType());
            var pinPanel = new PinPanel(pinControl) { ContextMenu = canvas.ContextMenu };
            pinPanel.PropertyChanged += PinPanel_PropertyChanged;
            pinPanel.LoadPin(pin);
            canvas.Children.Add(pinPanel);
        }

        private void ChangeColorCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (contextElement is PinPanel) || (contextElement is PinspacePanel);
        }

        private void ChangeColorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (contextElement is PinspacePanel)
            {
                ChangePinspaceColor();
            }
            else if (contextElement is PinPanel pinPanel)
            {
                ChangePinColor(pinPanel);
            }
        }

        private void ChangePinColor(PinPanel pinPanel)
        {
            var color = ColorExtensions.FromHtmlString(pinPanel.PinColor, defaultPinColor);
            if (ColorDialog.ShowDialog(ref color))
            {
                pinPanel.PinColor = color.ToHtmlString();
            }
        }

        private void ChangePinspaceColor()
        {
            var color = ColorExtensions.FromHtmlString(BackgroundColor, defaultPinColor);
            if (ColorDialog.ShowDialog(ref color))
            {
                BackgroundColor = color.ToHtmlString();
                dataRepository.UpdatePinspace(pinspace);
            }
        }

        private void GenerateNewPinControlsMenu()
        {
            var newPinMenuItem = canvas.ContextMenu.Items[0] as MenuItem;
            foreach (var type in pinFactory.PinControlTypes)
            {
                var newMenuItem = new MenuItem
                {
                    Header = pinFactory.GetDisplayName(type)
                };
                newMenuItem.Click += NewPinMenuItem_Click;
                newMenuItem.Tag = type;
                newPinMenuItem.Items.Add(newMenuItem);
            }
        }

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var contextElement = GetContextElement(e.OriginalSource);
            e.CanExecute = contextElement is PinspacePanel;
        }

        private void NewPin(Type pinControlType, Point position)
        {
            var pin = pinFactory.NewPin(pinControlType);
            pin.Color = defaultPinColor.ToHtmlString();
            pin.Left = position.X;
            pin.Title = "New " + pinFactory.GetDisplayName(pinControlType);
            pin.Top = position.Y;
            AddPinPanel(pin);
            dataRepository.UpdatePin(pinspace.Id, pin);
        }

        private void NewPinMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            var pinControlType = (Type)menuItem.Tag;
            NewPin(pinControlType, targetPoint);
        }

        private void PinPanel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pin.Left) || e.PropertyName == nameof(Pin.Top) || e.PropertyName == nameof(Pin.Height) || e.PropertyName == nameof(Pin.Width))
            {
                updateCanvasSizeMethodExecutor.Execute();
            }
            if (isLoading)
            {
                return;
            }
            var pinPanel = sender as PinPanel;
            pendingPinChanges.TryAdd(pinPanel.Pin.Id, pinPanel.Pin);
            processPendingPinChangesMethodExecutor.Execute();
        }

        private void PinspaceContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var contextMenu = (sender as FrameworkElement).ContextMenu;
            contextElement = GetContextElement(e.OriginalSource);
            targetPoint = new Point(e.CursorLeft, e.CursorTop);

            while (contextMenu.Items.Count > baseContextMenuItemCount)
            {
                contextMenu.Items.RemoveAt(baseContextMenuItemCount);
            }

            if (contextElement is PinPanel pinPanel)
            {
                pinPanel.AddContextMenuItems(contextMenu);
            }

            if (contextMenu.Items.Count > baseContextMenuItemCount)
            {
                contextMenu.Items.Insert(baseContextMenuItemCount, new Separator());
            }
        }

        private void ProcessPendingPinChanges()
        {
            foreach (var pin in pendingPinChanges.Values)
            {
                dataRepository.UpdatePin(pinspace.Id, pin);
            }
            pendingPinChanges.Clear();
        }

        private void RemoveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = contextElement is PinPanel;
        }

        private void RemoveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RemovePin(GetContextElement(e.OriginalSource) as PinPanel);
        }

        private void RemovePin(PinPanel pinPanel)
        {
            canvas.Children.Remove(pinPanel);
            dataRepository.DeletePin(pinspace.Id, pinPanel.Pin);
        }

        private void RenameCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (contextElement is PinPanel) || (contextElement is PinspacePanel);
        }

        private void RenameCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (contextElement is PinspacePanel)
            {
                RenamePinspace();
            }
            else if (contextElement is PinPanel pinPanel)
            {
                RenamePin(pinPanel);
            }
        }

        private void RenamePinspace()
        {
            var title = pinspace.Title;
            if (InputDialog.ShowInputDialog("Rename", "Enter a new name:", ref title))
            {
                pinspace.Title = title;
                dataRepository.UpdatePinspace(pinspace);
                PropertyChanged?.Invoke(pinspace, new PropertyChangedEventArgs(nameof(pinspace.Title)));
            }
        }

        private void UpdateCanvasSize()
        {
            var height = scrollViewer.ViewportHeight;
            var width = scrollViewer.ViewportWidth;

            foreach (var child in canvas.Children.OfType<PinPanel>())
            {
                height = Math.Max(height, Canvas.GetTop(child) + child.ActualHeight);
                width = Math.Max(width, Canvas.GetLeft(child) + child.ActualWidth);
            }

            if (height > scrollViewer.ViewportHeight)
            {
                height += SystemParameters.HorizontalScrollBarHeight;
            }

            if (width > scrollViewer.ViewportWidth)
            {
                width += SystemParameters.VerticalScrollBarWidth;
            }

            canvas.Height = height;
            canvas.Width = width;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            updateCanvasSizeMethodExecutor.Execute();
        }
    }
}
