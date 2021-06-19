using Microsoft.Extensions.Options;
using Pinspaces.Configuration;
using Pinspaces.Core.Data;
using Pinspaces.Core.Interfaces;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Pinspaces.Controls
{
    public partial class PinspacePanel : UserControl, INotifyPropertyChanged, IDisposable
    {
        private readonly int baseContextMenuItemCount;
        private readonly IDataRepository dataRepository;
        private readonly IDelayedAction delayedProcessPendingPinChangesMethod;
        private readonly IDelayedAction delayedUpdateCanvasSizeAction;
        private readonly Dictionary<Guid, Pin> pendingPinChanges = new();
        private readonly IPinFactory pinFactory;
        private readonly Settings settings;
        private FrameworkElement contextElement;
        private bool disposedValue;
        private bool isLoading;
        private Pinspace pinspace;
        private Point targetPoint;

        public PinspacePanel(IDataRepository dataRepository, IPinFactory pinFactory, IDelayedActionFactory delayedActionFactory, IOptions<Settings> options)
        {
            InitializeComponent();
            DataContext = this;

            this.dataRepository = dataRepository;
            this.pinFactory = pinFactory;
            settings = options.Value;
            delayedProcessPendingPinChangesMethod = delayedActionFactory.Debounce(ProcessPendingPinChanges, 5000);
            delayedUpdateCanvasSizeAction = delayedActionFactory.Debounce(UpdateCanvasSize, 100);

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
                NotifyPropertyChanged(nameof(BackgroundColor));
            }
        }

        public string Title => pinspace.Title;

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
                NotifyPropertyChanged(null);
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
                    delayedProcessPendingPinChangesMethod.Stop();
                    delayedUpdateCanvasSizeAction.Stop();
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
            if (InputDialog.ShowInputDialog("Rename Pin", "Enter the new name for this pin:", ref title))
            {
                pinPanel.Title = title;
            }
        }

        private void AddPinPanel(Pin pin)
        {
            var pinControl = pinFactory.NewPinControl(pin.GetType());
            var pinPanel = new PinPanel(pinControl)
            {
                ContextMenu = canvas.ContextMenu,
                DefaultPinColor = settings.GetDefaultPinColor()
            };
            pinPanel.PropertyChanged += PinPanel_PropertyChanged;
            pinPanel.LoadPin(pinspace.Id, pin);
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
            var color = ColorExtensions.FromHtmlString(pinPanel.PinColor, settings.GetDefaultPinColor());
            if (ColorDialog.ShowDialog(ref color))
            {
                pinPanel.PinColor = color.ToHtmlString();
            }
        }

        private void ChangePinspaceColor()
        {
            var color = ColorExtensions.FromHtmlString(BackgroundColor, settings.GetDefaultPinspaceColor());
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
            pin.Color = settings.GetDefaultPinColor().ToHtmlString();
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

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PinPanel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pin.Left) || e.PropertyName == nameof(Pin.Top) || e.PropertyName == nameof(Pin.Height) || e.PropertyName == nameof(Pin.Width))
            {
                delayedUpdateCanvasSizeAction.Execute();
            }
            if (isLoading)
            {
                return;
            }
            var pinPanel = sender as PinPanel;
            pendingPinChanges.TryAdd(pinPanel.Pin.Id, pinPanel.Pin);
            delayedProcessPendingPinChangesMethod.Execute();
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
                if (pin is ErrorPin)
                {
                    continue;
                }
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
            pendingPinChanges.Remove(pinPanel.Pin.Id);
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
            if (InputDialog.ShowInputDialog("Rename Pinspace", "Enter a new name for this pinspace:", ref title))
            {
                pinspace.Title = title;
                dataRepository.UpdatePinspace(pinspace);
                NotifyPropertyChanged(nameof(Title));
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
            delayedUpdateCanvasSizeAction.Execute();
        }
    }
}
