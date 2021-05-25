using Pinspaces.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pinspaces.Core.Extensions
{
    public static class CloneExtensions
    {
        public static void Assign(Type type, object destination, object source, out bool wasChanged)
        {
            wasChanged = false;
            var allowableTypes = new Type[] {
                typeof(string)
            };
            var properties = type.GetProperties().Where(p => p.CanRead && p.CanWrite && (p.PropertyType.IsPrimitive || p.PropertyType.IsValueType || allowableTypes.Contains(p.PropertyType))).ToList();
            foreach (var property in properties)
            {
                var sourceValue = property.GetValue(source, null);
                var destinationValue = property.GetValue(destination, null);
                if ((sourceValue == null && destinationValue != null) || sourceValue != destinationValue)
                {
                    wasChanged = true;
                    property.SetValue(destination, sourceValue);
                }
            }
        }

        public static void Assign<T>(this IList<T> destination, IList<T> source, out bool wasChanged) where T : ICloneable<T>
        {
            wasChanged = false;
            if (source == null)
            {
                return;
            }
            else if (destination.Count == source.Count)
            {
                for (var i = 0; i < source.Count; i++)
                {
                    destination[i].Assign(source[i], out var localWasChanged);
                    wasChanged = wasChanged || localWasChanged;
                }
            }
            else
            {
                wasChanged = true;
                destination.Clear();
                foreach (var item in source)
                {
                    destination.Add(item.Clone());
                }
            }
        }

        public static IList<T> Clone<T>(this IList<T> list) where T : ICloneable<T>
        {
            return list.Select(item => item.Clone()).ToList();
        }
    }
}
