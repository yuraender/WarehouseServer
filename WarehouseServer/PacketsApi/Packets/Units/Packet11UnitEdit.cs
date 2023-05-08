using Newtonsoft.Json;
using System.IO;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Entities;
using WarehouseServer.Server.Entities.Warehouse;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet11UnitEdit : PacketBase {
        public Packet11UnitEdit() : base(11) {
        }
    }

    public class Packet11UnitEditHandler : PacketHandlerBase {

        private Unit _unit;
        private byte _action;

        public override void Read(BinaryReader reader) {
            _unit = JsonConvert.DeserializeObject<Unit>(reader.ReadString());
            _action = reader.ReadByte();
        }

        public override bool Handle() {
            if (!User.IsAdmin) {
                return false;
            }
            using (ServerContext context = new ServerContext()) {
                Unit unit = context.Units.Find(_unit.ID);
                if (unit == null && _action != 0) {
                    return false;
                }
                if (_action == 0) {
                    _unit = context.Units.Add(new Unit {
                        Name = _unit.Name
                    });
                } else if (_action == 1) {
                    if (!string.IsNullOrEmpty(_unit.Name)) {
                        unit.Name = _unit.Name;
                    }
                } else {
                    context.Units.Remove(unit);
                }
                context.SaveChanges();
            }
            return true;
        }

        public override object Clone() {
            return new Packet11UnitEditHandler();
        }

        public override string ToString() {
            if (_action == 0) {
                return $"создал подразделение {_unit}";
            } else if (_action == 1) {
                return $"изменил информацию о подразделении {_unit}";
            } else {
                return $"удалил подразделение {_unit}";
            }
        }
    }
}
