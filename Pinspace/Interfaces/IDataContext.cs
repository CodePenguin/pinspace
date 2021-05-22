using Pinspace.Data;
using System;
using System.Collections.Generic;

namespace Pinspace.Interfaces
{
    public interface IDataContext
    {
        public Data.Pinspace GetPinspace(Guid id);

        public IList<PinWindow> GetPinWindows();

        public void UpdatePinspace(Data.Pinspace pinspace);

        public void UpdatePinWindow(PinWindow pinWindow);
    }
}
