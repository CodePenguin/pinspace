using Pinspaces.Core.Data;
using System;
using System.Collections.Generic;

namespace Pinspaces.Interfaces
{
    public interface IDataContext
    {
        public Pinspace GetPinspace(Guid id);

        public IList<PinWindow> GetPinWindows();

        public void UpdatePinspace(Pinspace pinspace);

        public void UpdatePinWindow(PinWindow pinWindow);
    }
}
