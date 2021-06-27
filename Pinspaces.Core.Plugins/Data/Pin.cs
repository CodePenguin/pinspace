using Pinspaces.Core.Extensions;
using Pinspaces.Core.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Pinspaces.Core.Data
{
    public abstract class Pin : ICloneable<Pin>, INotifyPropertyChanged
    {
        private string color;
        private double height = 200;
        private Guid id = Guid.NewGuid();
        private double left;
        private string title;
        private double top;
        private double width = 300;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Color { get => color; set => SetProperty(ref color, value); }
        public double Height { get => height; set => SetProperty(ref height, value); }
        public Guid Id { get => id; set => SetProperty(ref id, value); }
        public double Left { get => left; set => SetProperty(ref left, value); }
        public string Title { get => title; set => SetProperty(ref title, value); }
        public double Top { get => top; set => SetProperty(ref top, value); }
        public double Width { get => width; set => SetProperty(ref width, value); }

        public virtual void Assign(Pin source, out bool wasChanged)
        {
            CloneExtensions.Assign(GetType(), this, source, out wasChanged);
        }

        public Pin Clone()
        {
            var clone = (Pin)MemberwiseClone();
            clone.Assign(this, out _);
            return clone;
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
    }
}
