using Pinspaces.Core.Interfaces;
using Pinspaces.Interfaces;
using System;
using System.Text.RegularExpressions;

namespace Pinspaces.Data
{
    public class PinDataRepository : IPinDataRepository
    {
        private readonly IDataRepository dataRepository;
        private readonly Regex validKeyRegEx;

        public PinDataRepository(IDataRepository dataRepository)
        {
            this.dataRepository = dataRepository;
            validKeyRegEx = new Regex("(?:[ ]*[A-Z-a-z0-9-_.])+");
        }

        public bool Delete(Guid pinspaceId, Guid pinId, string key)
        {
            ValidateKey(key);
            return dataRepository.DeletePinData(pinspaceId, pinId, key);
        }

        public string[] GetKeys(Guid pinspaceId, Guid pinId)
        {
            return dataRepository.GetPinDataKeys(pinspaceId, pinId);
        }

        public bool Retrieve(Guid pinspaceId, Guid pinId, string key, out byte[] data)
        {
            ValidateKey(key);
            return dataRepository.RetrievePinData(pinspaceId, pinId, key, out data);
        }

        public void Store(Guid pinspaceId, Guid pinId, string key, byte[] data)
        {
            ValidateKey(key);
            dataRepository.StorePinData(pinspaceId, pinId, key, data);
        }

        private void ValidateKey(string key)
        {
            if (!validKeyRegEx.IsMatch(key))
            {
                throw new ArgumentException("Invalid pin data key");
            }
        }
    }
}
