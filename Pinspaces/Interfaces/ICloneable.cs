namespace Pinspaces.Interfaces
{
    public interface ICloneable<T>
    {
        public void Assign(T source, out bool wasChanged);

        public T Clone();
    }
}
