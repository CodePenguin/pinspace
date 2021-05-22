using System;

namespace Pinspace.Data
{
    public class PinWindow : ICloneable
    {
        private const int DefaultWindowHeight = 600;
        private const int DefaultWindowWidth = 900;

        private int height;
        private bool isMaximized;
        private int width;

        public PinWindow()
        {
            Id = Guid.NewGuid();
            Height = DefaultWindowHeight;
            Width = DefaultWindowWidth;
        }

        public Guid ActivePinspaceId { get; set; }
        public string Color { get; set; }

        public int Height
        {
            get => height;
            set
            {
                height = value;
                CheckMaximized();
            }
        }

        public Guid Id { get; set; }

        public bool IsMaximized
        {
            get => isMaximized;
            set
            {
                isMaximized = value;
                CheckMaximized();
            }
        }

        public int Left { get; set; }

        public int Top { get; set; }

        public int Width
        {
            get => width;
            set
            {
                width = value;
                CheckMaximized();
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        private void CheckMaximized()
        {
            if (IsMaximized)
            {
                height = DefaultWindowHeight;
                width = DefaultWindowWidth;
            }
        }
    }
}
