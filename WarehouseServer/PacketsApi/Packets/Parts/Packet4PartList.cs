using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Entities.Warehouse;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet4PartList : PacketBase {

        private readonly List<PartDto> _parts;

        public Packet4PartList(List<PartDto> parts) : base(4) {
            _parts = parts;
        }

        public override void WriteBody(BinaryWriter writer) {
            writer.Write(JsonConvert.SerializeObject(_parts));
        }
    }

    public class Packet4PartListHandler : PacketHandlerBase {

        private string _name;
        private bool _isInRepair;
        private bool _wasInRepair;
        private Group _group;
        private Unit _unit;

        public override void Read(BinaryReader reader) {
            _name = reader.ReadString();
            _isInRepair = reader.ReadBoolean();
            _wasInRepair = reader.ReadBoolean();
            _group = JsonConvert.DeserializeObject<Group>(reader.ReadString());
            _unit = JsonConvert.DeserializeObject<Unit>(reader.ReadString());
        }

        public override bool Handle() {
            using (ServerContext context = new ServerContext()) {
                IEnumerable<PartDto> parts = context.Parts
                    .Include("Group").Include("Group.Unit")
                    .Include("Repairs").Include("Repairs.Part")
                    .Where(p => p.Name.Contains(_name)).ToList()
                    .Select(p => p.Dto());
                if (_isInRepair) {
                    parts = parts.Where(p => p.Repairs.Any(r => r.ReturnDate == null));
                }
                if (_wasInRepair) {
                    parts = parts.Where(p => p.Repairs.Any(r => r.ReturnDate != null));
                }
                if (_group != null) {
                    parts = parts.Where(p => p.Group.ID == _group.ID);
                }
                if (_unit != null) {
                    parts = parts.Where(p => p.Group.Unit.ID == _unit.ID);
                }
                Connection.Send(new Packet4PartList(parts.ToList()));
            }
            return true;
        }

        public override object Clone() {
            return new Packet4PartListHandler();
        }

        public override string ToString() {
            return null;
        }
    }
}
