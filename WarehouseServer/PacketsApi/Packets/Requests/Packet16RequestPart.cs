using Newtonsoft.Json;
using System.IO;
using System.Linq;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Entities.Warehouse;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet16RequestPart : PacketBase {
        public Packet16RequestPart() : base(16) {
        }
    }

    public class Packet16RequestPartHandler : PacketHandlerBase {

        private Part _part;

        public override void Read(BinaryReader reader) {
            _part = JsonConvert.DeserializeObject<Part>(reader.ReadString());
        }

        public override bool Handle() {
            if (!User.IsAdmin) {
                return false;
            }
            using (ServerContext context = new ServerContext()) {
                Part part = context.Parts.Include("Group")
                    .Where(p => p.Name == _part.Name)
                    .Where(p => p.Type == _part.Type)
                    .Where(p => p.Description == _part.Description)
                    .Where(p => p.Group.ID == _part.Group.ID).FirstOrDefault();
                if (part != null) {
                    part.Amount += _part.Amount;
                } else {
                    context.Parts.Add(new Part {
                        Name = _part.Name,
                        Type = _part.Type,
                        Amount = _part.Amount,
                        Measure = _part.Measure,
                        Description = _part.Description,
                        Group = context.Groups.Find(_part.Group.ID)
                    });
                }
                context.SaveChanges();
            }
            return true;
        }

        public override object Clone() {
            return new Packet16RequestPartHandler();
        }

        public override string ToString() {
            return null;
        }
    }
}
