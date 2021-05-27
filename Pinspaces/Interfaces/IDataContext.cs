using Pinspaces.Core.Data;
using System;
using System.Collections.Generic;

namespace Pinspaces.Interfaces
{
    public interface IDataContext
    {
        void DeletePin(Guid pinspaceId, Pin pin);

        public IList<Pin> GetPins(Guid pinspaceId);

        public Pinspace GetPinspace(Guid id);

        public IList<Pinspace> GetPinspaces();

        public IList<PinWindow> GetPinWindows();

        public void UpdatePin(Guid pinspaceId, Pin pin);

        public void UpdatePinspace(Pinspace pinspace);

        public void UpdatePinWindow(PinWindow pinWindow);
    }
}
