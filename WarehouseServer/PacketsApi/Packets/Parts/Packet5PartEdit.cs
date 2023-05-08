using Newtonsoft.Json;
using System.IO;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Entities.Warehouse;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet5PartEdit : PacketBase {
        public Packet5PartEdit() : base(5) {
        }
    }

    public class Packet5PartEditHandler : PacketHandlerBase {

        private Part _part;
        private byte _action;

        public override void Read(BinaryReader reader) {
            _part = JsonConvert.DeserializeObject<Part>(reader.ReadString());
            _action = reader.ReadByte();
        }

        public override bool Handle() {
            if (!User.IsAdmin && (User.Unit == null || !User.IsUnitAdmin)) {
                return false;
            }
            using (ServerContext context = new ServerContext()) {
                Part part = context.Parts.Find(_part.ID);
                if (part == null && _action != 0) {
                    return false;
                }
                if (_action == 0) {
                    _part = context.Parts.Add(new Part {
                        Name = _part.Name,
                        Type = _part.Type,
                        Amount = _part.Amount,
                        Measure = _part.Measure,
                        Description = !string
                            .IsNullOrEmpty(_part.Description) ? _part.Description : null,
                        Group = context.Groups.Find(_part.Group.ID)
                    });
                } else if (_action == 1) {
                    if (!string.IsNullOrEmpty(_part.Name)) {
                        part.Name = _part.Name;
                    }
                    if (!string.IsNullOrEmpty(_part.Type)) {
                        part.Type = _part.Type;
                    }
                    part.Amount = _part.Amount;
                    part.Measure = _part.Measure;
                    part.Description = !string
                        .IsNullOrEmpty(_part.Description) ? _part.Description : null;
                    part.Group = context.Groups.Find(_part.Group.ID);
                } else {
                    context.Parts.Remove(part);
                }
                context.SaveChanges();
            }
            return true;
        }

        public override object Clone() {
            return new Packet5PartEditHandler();
        }

        public override string ToString() {
            if (_action == 0) {
                return $"создал деталь {_part}";
            } else if (_action == 1) {
                return $"изменил информацию о детали {_part}";
            } else {
                return $"удалил деталь {_part}";
            }
        }
    }
}
