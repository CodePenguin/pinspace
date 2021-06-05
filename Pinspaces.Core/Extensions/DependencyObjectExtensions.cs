using System.Windows;
using System.Windows.Media;

namespace Pinspaces.Core.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static T FindParent<T>(this DependencyObject current) where T : DependencyObject
        {
            while (current != null && !(current is T))
            {
                current = VisualTreeHelper.GetParent(current);
            }
            return (T)current;
        }
    }
}
