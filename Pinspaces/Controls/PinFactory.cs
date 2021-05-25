using Pinspaces.Core.Controls;
using Pinspaces.Core.Data;
using Pinspaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Pinspaces.Controls
{
    public class PinFactory : IPinFactory
    {
        private readonly Dictionary<string, Type> pinControlTypes = new();
        private readonly Dictionary<string, Type> pinTypes = new();

        public PinFactory(IServiceProvider serviceProvider, IRegisteredPinControlTypes pinControlTypesList)
        {
            ServiceProvider = serviceProvider;

            foreach (var pinControlType in pinControlTypesList)
            {
                RegisterPinControl(pinControlType);
            }
        }

        public IEnumerable<Type> PinControlTypes => pinControlTypes.Values;
        public IServiceProvider ServiceProvider { get; }

        public string GetDisplayName(string pinTypeName)
        {
            var pinControlType = GetPinControlType(pinTypeName);
            return GetDisplayName(pinControlType);
        }

        public string GetDisplayName(Type pinControlType)
        {
            var attribute = GetPinTypeAttribute(pinControlType);
            return attribute.DisplayName;
        }

        public Type GetPinControlType(string pinTypeName)
        {
            if (pinControlTypes.TryGetValue(pinTypeName, out var pinControlType))
            {
                return pinControlType;
            }
            throw new ArgumentException($"Unknown Pin Type: {pinTypeName}");
        }

        public Type GetPinType(Type pinControlType)
        {
            var pinTypeAttribute = GetPinTypeAttribute(pinControlType);
            return pinTypeAttribute.PinType;
        }

        public Type GetPinType(string pinTypeName)
        {
            if (pinTypes.TryGetValue(pinTypeName, out var pinType))
            {
                return pinType;
            }
            throw new ArgumentException($"Unknown Pin Type: {pinTypeName}");
        }

        public Pin NewPin(Type pinControlType)
        {
            var attribute = GetPinTypeAttribute(pinControlType);
            var pinType = attribute.PinType;
            return Activator.CreateInstance(pinType, null) as Pin;
        }

        public PinControl NewPinControl(Type pinType)
        {
            var pinTypeName = GetPinTypeName(pinType);
            var pinControlType = GetPinControlType(pinTypeName);
            return ServiceProvider.GetService(pinControlType) as PinControl;
        }

        private PinTypeAttribute GetPinTypeAttribute(Type pinControlType)
        {
            var attribute = pinControlType.GetCustomAttribute<PinTypeAttribute>();
            if (attribute == null)
            {
                throw new ArgumentException($"Pin Type Attribute not found: {pinControlType.FullName}");
            }
            return attribute;
        }

        private string GetPinTypeName(Type type)
        {
            Type pinType;
            if (type.IsAssignableTo(typeof(PinControl)))
            {
                var attribute = GetPinTypeAttribute(type);
                pinType = attribute.PinType;
            }
            else if (type.IsAssignableTo(typeof(Pin)))
            {
                pinType = type;
            }
            else
            {
                throw new ArgumentException($"Incompatible Pin Class: {type.FullName}");
            }
            return pinType.FullName;
        }

        private void RegisterPinControl(Type pinControlType)
        {
            var pinTypeAttribute = GetPinTypeAttribute(pinControlType);
            if (pinTypeAttribute == null)
            {
                throw new ArgumentException($"Incompatible Pin Class: {pinControlType.FullName}");
            }

            var typeName = GetPinTypeName(pinControlType);
            pinControlTypes.Add(typeName, pinControlType);
            pinTypes.Add(typeName, pinTypeAttribute.PinType);
        }
    }
}
