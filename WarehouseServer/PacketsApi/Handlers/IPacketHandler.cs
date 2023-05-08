using System;
using System.IO;

namespace WarehouseServer.PacketsApi.Handlers {
    public interface IPacketHandler : ICloneable {
        void Read(BinaryReader reader);

        bool Handle();
    }
}
