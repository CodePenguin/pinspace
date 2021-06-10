using Pinspaces.Core.Interfaces;
using Pinspaces.Interfaces;
using System;

namespace Pinspaces.Data
{
    public class PinDataRepository : IPinDataRepository
    {
        private readonly IDataRepository dataRepository;

        public PinDataRepository(IDataRepository dataRepository)
        {
            this.dataRepository = dataRepository;
        }

        public bool Retrieve(Guid pinspaceId, Guid pinId, string key, out byte[] data)
        {
            return dataRepository.RetrievePinData(pinspaceId, pinId, key, out data);
        }

        public void Store(Guid pinspaceId, Guid pinId, string key, byte[] data)
        {
            dataRepository.StorePinData(pinspaceId, pinId, key, data);
        }
    }
}
