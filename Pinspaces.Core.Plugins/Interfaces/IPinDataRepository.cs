using System;

namespace Pinspaces.Core.Interfaces
{
    public interface IPinDataRepository
    {
        public bool Delete(Guid pinspaceId, Guid pinId, string key);

        public string[] GetKeys(Guid pinspaceId, Guid pinId);

        public bool Retrieve(Guid pinspaceId, Guid pinId, string key, out byte[] data);

        public void Store(Guid pinspaceId, Guid pinId, string key, byte[] data);
    }
}
