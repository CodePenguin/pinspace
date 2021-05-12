using System;
using System.Windows.Forms;

namespace FilePinboard
{
    public enum SplitDirection
    {
        Horizontal,
        Vertical
    }

    public class SplitView : Panel
    {
        public Control MainControl { get; private set; }
        public Control SplitControl { get; private set; }
        public int SplitPercentage { get; set; } = 50;

        private readonly SplitDirection splitDirection;
        private readonly Splitter splitter;

        public SplitView(SplitDirection splitDirection, Control mainControl, Control splitControl)
        {
            this.splitDirection = splitDirection;

            Dock = DockStyle.Fill;

            MainControl = mainControl;
            Controls.Add(MainControl);
            MainControl.Dock = DockStyle.Fill;

            splitter = new Splitter { Dock = splitDirection == SplitDirection.Horizontal ? DockStyle.Bottom : DockStyle.Right };
            splitter.SplitterMoved += OnSplitterMoved;
            Controls.Add(splitter);

            SplitControl = splitControl;
            Controls.Add(SplitControl);
            SplitControl.Dock = splitDirection == SplitDirection.Horizontal ? DockStyle.Bottom : DockStyle.Right;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (splitDirection == SplitDirection.Horizontal)
            {
                SplitControl.Height = Convert.ToInt32((decimal)Height * SplitPercentage / 100);
            }
            else
            {
                SplitControl.Width = Convert.ToInt32((decimal)Width * SplitPercentage / 100);
            }
        }

        private void OnSplitterMoved(object sender, SplitterEventArgs e)
        {
            if (splitDirection == SplitDirection.Horizontal)
            {
                SplitPercentage = Convert.ToInt32((decimal)SplitControl.Height / Height * 100);
            }
            else
            {
                SplitPercentage = Convert.ToInt32((decimal)SplitControl.Width / Width * 100);
            }
        }

    }
}
