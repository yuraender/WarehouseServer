using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Entities.Warehouse;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet10UnitList : PacketBase {

        private readonly List<Unit> _units;

        public Packet10UnitList(List<Unit> units) : base(10) {
            _units = units;
        }

        public override void WriteBody(BinaryWriter writer) {
            writer.Write(JsonConvert.SerializeObject(_units));
        }
    }

    public class Packet10UnitListHandler : PacketHandlerBase {
        public override bool Handle() {
            using (ServerContext context = new ServerContext()) {
                List<Unit> units = context.Units.OrderBy(u => u.ID).ToList();
                Connection.Send(new Packet10UnitList(units));
            }
            return true;
        }

        public override object Clone() {
            return new Packet10UnitListHandler();
        }

        public override string ToString() {
            return null;
        }
    }
}
