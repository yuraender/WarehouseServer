using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Entities.Warehouse;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet8GroupList : PacketBase {

        private readonly List<GroupDto> _groups;

        public Packet8GroupList(List<GroupDto> groups) : base(8) {
            _groups = groups;
        }

        public override void WriteBody(BinaryWriter writer) {
            writer.Write(JsonConvert.SerializeObject(_groups));
        }
    }

    public class Packet8GroupListHandler : PacketHandlerBase {

        private Unit _unit;

        public override void Read(BinaryReader reader) {
            _unit = JsonConvert.DeserializeObject<Unit>(reader.ReadString());
        }

        public override bool Handle() {
            using (ServerContext context = new ServerContext()) {
                List<Group> groups = context.Groups.Include("Unit").ToList();
                if (_unit != null) {
                    groups = groups.Where(g => g.Unit.ID == _unit.ID).ToList();
                }
                Connection.Send(new Packet8GroupList(groups.Select(g => g.Dto()).ToList()));
            }
            return true;
        }

        public override object Clone() {
            return new Packet8GroupListHandler();
        }

        public override string ToString() {
            return null;
        }
    }
}
