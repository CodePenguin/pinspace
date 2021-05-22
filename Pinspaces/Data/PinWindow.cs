using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System;

namespace Pinspaces.Data
{
    public class PinWindow : ICloneable<PinWindow>
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

        public void Assign(PinWindow source, out bool wasChanged)
        {
            CloneExtensions.Assign(GetType(), this, source, out wasChanged);
        }

        public PinWindow Clone()
        {
            var clone = (PinWindow)MemberwiseClone();
            Assign(this, out _);
            return clone;
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
