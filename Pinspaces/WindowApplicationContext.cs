using Pinspaces.Controls;
using Pinspaces.Core.Data;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Pinspaces
{
    public class WindowApplicationContext
    {
        private readonly IDataRepository dataRepository;
        private readonly WindowFactory windowFactory;
        private readonly List<PinWindowForm> windows = new();

        public WindowApplicationContext(WindowFactory windowFactory, IDataRepository dataRepository)
        {
            this.windowFactory = windowFactory;
            this.dataRepository = dataRepository;
        }

        public void LoadData()
        {
            var windows = dataRepository.GetPinWindows();
            if (windows.Count == 0)
            {
                NewWindow();
            }
            foreach (var window in windows)
            {
                NewWindow(window);
            }
        }

        public void NewWindow()
        {
            var pinspace = new Pinspace();
            var pinWindow = new PinWindow
            {
                ActivePinspaceId = pinspace.Id,
                Height = PinWindow.DefaultHeight,
                Left = SystemParameters.WorkArea.Width / 2 - PinWindow.DefaultWidth / 2,
                Top = SystemParameters.WorkArea.Height / 2 - PinWindow.DefaultHeight / 2,
                Width = PinWindow.DefaultWidth
            };

            dataRepository.UpdatePinspace(pinspace);
            dataRepository.UpdatePinWindow(pinWindow);
            NewWindow(pinWindow);
        }

        public void NewWindow(PinWindow pinWindow)
        {
            var window = windowFactory.CreateForm<PinWindowForm>();
            windows.Add(window);
            window.Closed += Window_FormClosed;
            window.LoadWindow(pinWindow);
            window.Show();
        }

        public void Run()
        {
            LoadData();
        }

        private void Window_FormClosed(object sender, EventArgs e)
        {
            var window = sender as PinWindowForm;
            windows.Remove(window);
            window.Close();
        }
    }
}
