using Pinspaces.Core.Data;
using System;
using System.Collections.Generic;

namespace Pinspaces.Interfaces
{
    public interface IDataRepository
    {
        void DeletePin(Guid pinspaceId, Pin pin);

        bool DeletePinData(Guid pinspaceId, Guid pinId, string key);

        string[] GetPinDataKeys(Guid pinspaceId, Guid pinId);

        public IList<Pin> GetPins(Guid pinspaceId);

        public Pinspace GetPinspace(Guid id);

        public IList<Pinspace> GetPinspaces();

        public IList<PinWindow> GetPinWindows();

        public bool RetrievePinData(Guid pinspaceId, Guid pinId, string key, out byte[] data);

        public void StorePinData(Guid pinspaceId, Guid pinId, string key, byte[] data);

        public void UpdatePin(Guid pinspaceId, Pin pin);

        public void UpdatePinspace(Pinspace pinspace);

        public void UpdatePinWindow(PinWindow pinWindow);
    }
}
