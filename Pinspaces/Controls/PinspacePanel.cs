using Pinspaces.Core.Data;
using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Pinspaces.Controls
{
    public class PinspacePanel : Panel
    {
        private readonly IDataContext dataContext;
        private readonly Color defaultPinColor = Color.FromArgb(51, 122, 183);
        private readonly IPinFactory pinFactory;
        private int baseContextMenuItemCount;
        private ToolStripMenuItem changeColorMenuItem;
        private ToolStripMenuItem newPinMenuItem;
        private Pinspace pinspace;
        private ContextMenuStrip pinspaceContextMenuStrip;
        private ToolStripMenuItem removePinMenuItem;
        private ToolStripMenuItem renamePinMenuItem;
        private Point targetPoint;

        public PinspacePanel(IDataContext dataContext, IPinFactory pinFactory)
        {
            this.dataContext = dataContext;
            this.pinFactory = pinFactory;

            AutoScroll = true;
            AutoScrollMargin = new Size(10, 10);

            InitializeControl();
        }

        public static void RenamePin(PinPanel pinPanel)
        {
            var title = pinPanel.Title;
            if (FormExtensions.ShowInputDialog("Rename", ref title) == DialogResult.OK)
            {
                pinPanel.Title = title;
            }
        }

        public void LoadPinspace(Guid pinspaceId)
        {
            pinspace = dataContext.GetPinspace(pinspaceId);
            BackColor = ColorExtensions.FromHtmlString(pinspace.Color, SystemColors.Control);

            foreach (var pin in dataContext.GetPins(pinspaceId))
            {
                AddPinPanel(pin);
            }
        }

        public void NewPin(Type pinControlType, Point position)
        {
            var pin = pinFactory.NewPin(pinControlType);
            pin.Color = defaultPinColor.ToHtmlString();
            pin.Left = position.X;
            pin.Title = "New " + pinFactory.GetDisplayName(pinControlType);
            pin.Top = position.Y;
            AddPinPanel(pin);
            dataContext.UpdatePin(pinspace.Id, pin);
        }

        public void RemovePin(PinPanel pinPanel)
        {
            Controls.Remove(pinPanel);
            pinPanel.Dispose();

            dataContext.DeletePin(pinspace.Id, pinPanel.Pin);
        }

        private void AddPinPanel(Pin pin)
        {
            var pinControl = pinFactory.NewPinControl(pin.GetType());
            var pinPanel = new PinPanel(pinControl) { ContextMenuStrip = ContextMenuStrip };
            pinPanel.PropertiesChanged += PinPanel_PropertiesChanged;
            pinPanel.LoadPin(pin);
            Controls.Add(pinPanel);
        }

        private void ChangeColorMenuItem_Click(object sender, EventArgs e)
        {
            var contextControl = GetContextControl();
            using var colorDialog = new ColorDialog
            {
                Color = contextControl.BackColor,
                FullOpen = true
            };
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                if (contextControl is PinspacePanel)
                {
                    BackColor = colorDialog.Color;
                    pinspace.Color = colorDialog.Color.ToHtmlString();
                    dataContext.UpdatePinspace(pinspace);
                }
                if (contextControl is PinPanel pinPanel)
                {
                    pinPanel.PinColor = colorDialog.Color;
                }
            }
        }

        private void GenerateNewPinControlsMenu()
        {
            foreach (var type in pinFactory.PinControlTypes)
            {
                var newMenuItem = new ToolStripMenuItem
                {
                    Text = pinFactory.GetDisplayName(type)
                };
                newMenuItem.Click += NewPinMenuItem_Click;
                newMenuItem.Tag = type;
                newPinMenuItem.DropDownItems.Add(newMenuItem);
            }
        }

        private Control GetContextControl()
        {
            var control = pinspaceContextMenuStrip.SourceControl;
            while (control != null && !(control is PinPanel) && !(control is PinspacePanel))
            {
                control = control.Parent;
            }
            return control;
        }

        private void InitializeControl()
        {
            pinspaceContextMenuStrip = new ContextMenuStrip();
            pinspaceContextMenuStrip.Closing += new ToolStripDropDownClosingEventHandler(PinspaceContextMenuStrip_Closing);
            pinspaceContextMenuStrip.Opening += new CancelEventHandler(PinspaceContextMenuStrip_Opening);

            changeColorMenuItem = new ToolStripMenuItem { Text = "Change color..." };
            changeColorMenuItem.Click += ChangeColorMenuItem_Click;

            newPinMenuItem = new ToolStripMenuItem { Text = "New" };

            renamePinMenuItem = new ToolStripMenuItem { Text = "Rename..." };
            renamePinMenuItem.Click += RenamePinMenuItem_Click;

            removePinMenuItem = new ToolStripMenuItem { Text = "Remove" };
            removePinMenuItem.Click += RemovePinMenuItem_Click;

            pinspaceContextMenuStrip.Items.AddRange(new ToolStripItem[] {
                changeColorMenuItem,
                newPinMenuItem,
                renamePinMenuItem,
                removePinMenuItem});

            baseContextMenuItemCount = pinspaceContextMenuStrip.Items.Count;

            ContextMenuStrip = pinspaceContextMenuStrip;

            GenerateNewPinControlsMenu();
        }

        private void NewPinMenuItem_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            var pinControlType = (Type)menuItem.Tag;
            NewPin(pinControlType, targetPoint);
        }

        private void PinPanel_PropertiesChanged(object sender, EventArgs e)
        {
            var pinPanel = sender as PinPanel;
            dataContext.UpdatePin(pinspace.Id, pinPanel.Pin);
        }

        private void PinspaceContextMenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            while (pinspaceContextMenuStrip.Items.Count > baseContextMenuItemCount)
            {
                pinspaceContextMenuStrip.Items.RemoveAt(baseContextMenuItemCount);
            }
        }

        private void PinspaceContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (pinspaceContextMenuStrip.SourceControl is PinPanel)
            {
                // Do not show the context menu if coming from the panel itself
                e.Cancel = true;
                return;
            }

            targetPoint = PointToClient(pinspaceContextMenuStrip.Bounds.Location);
            var contextControl = GetContextControl();

            // Pinboard Window
            newPinMenuItem.Visible = contextControl is PinspacePanel;
            changeColorMenuItem.Visible = (contextControl is PinspacePanel) || (contextControl is PinPanel);

            // Pinboard Pin
            renamePinMenuItem.Visible = contextControl is PinPanel;
            removePinMenuItem.Visible = contextControl is PinPanel;

            if (contextControl is PinPanel pinPanel)
            {
                pinPanel.AddContextMenuItems(pinspaceContextMenuStrip);
            }

            if (pinspaceContextMenuStrip.Items.Count > baseContextMenuItemCount)
            {
                pinspaceContextMenuStrip.Items.Insert(baseContextMenuItemCount, new ToolStripSeparator());
            }
        }

        private void RemovePinMenuItem_Click(object sender, EventArgs e)
        {
            RemovePin(GetContextControl() as PinPanel);
        }

        private void RenamePinMenuItem_Click(object sender, EventArgs e)
        {
            RenamePin(GetContextControl() as PinPanel);
        }
    }
}
