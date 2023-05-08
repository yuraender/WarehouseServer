using Newtonsoft.Json;
using System.IO;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Sql;
using WarehouseServer.Server.Entities;
using WarehouseServer.Server.Entities.Warehouse;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet9GroupEdit : PacketBase {
        public Packet9GroupEdit() : base(9) {
        }
    }

    public class Packet9GroupEditHandler : PacketHandlerBase {

        private Group _group;
        private byte _action;

        public override void Read(BinaryReader reader) {
            _group = JsonConvert.DeserializeObject<Group>(reader.ReadString());
            _action = reader.ReadByte();
        }

        public override bool Handle() {
            if (!User.IsAdmin) {
                return false;
            }
            using (ServerContext context = new ServerContext()) {
                Group group = context.Groups.Find(_group.ID);
                if (group == null && _action != 0) {
                    return false;
                }
                if (_action == 0) {
                    _group = context.Groups.Add(new Group {
                        Name = _group.Name,
                        Unit = context.Units.Find(_group.Unit.ID)
                    });
                } else if (_action == 1) {
                    if (!string.IsNullOrEmpty(_group.Name)) {
                        group.Name = _group.Name;
                    }
                    group.Unit = context.Units.Find(_group.Unit.ID);
                } else {
                    context.Groups.Remove(group);
                }
                context.SaveChanges();
            }
            return true;
        }

        public override object Clone() {
            return new Packet9GroupEditHandler();
        }

        public override string ToString() {
            if (_action == 0) {
                return $"создал группу оборудования {_group}";
            } else if (_action == 1) {
                return $"изменил информацию о группе оборудования {_group}";
            } else {
                return $"удалил группу оборудования {_group}";
            }
        }
    }
}
