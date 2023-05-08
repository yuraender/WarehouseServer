using Newtonsoft.Json;
using System.IO;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Entities.Warehouse;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet13PartWriteOff : PacketBase {
        public Packet13PartWriteOff() : base(13) {
        }
    }

    public class Packet13PartWriteOffHandler : PacketHandlerBase {

        private Part _part;
        private int _amount;
        private string _reason;

        public override void Read(BinaryReader reader) {
            _part = JsonConvert.DeserializeObject<Part>(reader.ReadString());
            _amount = reader.ReadInt32();
            _reason = reader.ReadString();
        }

        public override bool Handle() {
            if (!User.IsAdmin && (User.Unit == null || !User.IsUnitAdmin)) {
                return false;
            }
            using (ServerContext context = new ServerContext()) {
                Part part = context.Parts.Find(_part.ID);
                if (part == null || _amount < 1 || _amount > part.Amount) {
                    return false;
                }
                part.Amount -= _amount;
                context.SaveChanges();
            }
            return true;
        }

        public override object Clone() {
            return new Packet13PartWriteOffHandler();
        }

        public override string ToString() {
            return $"списал {_amount} шт. детали {_part}. Причина: {_reason}";
        }
    }
}
