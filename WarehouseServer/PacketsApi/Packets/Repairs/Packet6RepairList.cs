using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Entities.Warehouse;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet6RepairList : PacketBase {

        private readonly List<Repair> _repairs;

        public Packet6RepairList(List<Repair> repairs) : base(6) {
            _repairs = repairs;
        }

        public override void WriteBody(BinaryWriter writer) {
            writer.Write(JsonConvert.SerializeObject(_repairs));
        }
    }

    public class Packet6RepairListHandler : PacketHandlerBase {

        private Part _part;

        public override void Read(BinaryReader reader) {
            _part = JsonConvert.DeserializeObject<Part>(reader.ReadString());
        }

        public override bool Handle() {
            if (_part == null) {
                return false;
            }
            using (ServerContext context = new ServerContext()) {
                List<Repair> repairs = context.Repairs.Include("Part")
                    .Where(r => r.Part.ID == _part.ID).ToList();
                Connection.Send(new Packet6RepairList(repairs));
            }
            return true;
        }

        public override object Clone() {
            return new Packet6RepairListHandler();
        }

        public override string ToString() {
            return null;
        }
    }
}
