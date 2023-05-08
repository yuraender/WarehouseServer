using Newtonsoft.Json;
using System.IO;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Entities;
using WarehouseServer.Server.Entities.Warehouse;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet7RepairEdit : PacketBase {
        public Packet7RepairEdit() : base(7) {
        }
    }

    public class Packet7RepairEditHandler : PacketHandlerBase {

        private Repair _repair;
        private Part _part;
        private byte _action;

        public override void Read(BinaryReader reader) {
            _repair = JsonConvert.DeserializeObject<Repair>(reader.ReadString());
            _part = JsonConvert.DeserializeObject<Part>(reader.ReadString());
            _action = reader.ReadByte();
        }

        public override bool Handle() {
            if (!User.IsAdmin) {
                return false;
            }
            using (ServerContext context = new ServerContext()) {
                Repair repair = context.Repairs.Find(_repair.ID);
                if (repair == null && _action != 0) {
                    return false;
                }
                if (_part != null) {
                    _repair.Part = context.Parts.Find(_part.ID);
                }
                if (_action == 0) {
                    _repair = context.Repairs.Add(new Repair {
                        Organization = _repair.Organization,
                        ReturnDate = _repair.ReturnDate,
                        DispatchDate = _repair.DispatchDate,
                        Part = context.Parts.Find(_repair.Part.ID)
                    });
                } else if (_action == 1) {
                    if (!string.IsNullOrEmpty(_repair.Organization)) {
                        repair.Organization = _repair.Organization;
                    }
                    repair.DispatchDate = _repair.DispatchDate;
                    if (_repair.ReturnDate.HasValue) {
                        repair.ReturnDate = _repair.ReturnDate;
                    } else {
                        repair.ReturnDate = null;
                    }
                } else {
                    context.Repairs.Remove(repair);
                }
                context.SaveChanges();
            }
            return true;
        }

        public override object Clone() {
            return new Packet7RepairEditHandler();
        }

        public override string ToString() {
            if (_action == 0) {
                return $"создал ремонт {_repair}";
            } else if (_action == 1) {
                return $"изменил информацию о ремонте {_repair}";
            } else {
                return $"удалил ремонт {_repair}";
            }
        }
    }
}
