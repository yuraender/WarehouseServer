using System.IO;

namespace WarehouseServer.PacketsApi.Packets {
    public interface IPacket {
        void Write(BinaryWriter writer);
    }
}
