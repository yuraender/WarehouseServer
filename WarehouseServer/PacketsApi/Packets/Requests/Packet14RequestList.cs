using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Entities.Warehouse;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet14RequestList : PacketBase {

        private readonly List<RequestDto> _requests;

        public Packet14RequestList(List<RequestDto> requests) : base(14) {
            _requests = requests;
        }

        public override void WriteBody(BinaryWriter writer) {
            writer.Write(JsonConvert.SerializeObject(_requests));
        }
    }

    public class Packet14RequestListHandler : PacketHandlerBase {

        private int _year;

        public override void Read(BinaryReader reader) {
            _year = reader.ReadInt32();
        }

        public override bool Handle() {
            using (ServerContext context = new ServerContext()) {
                List<RequestDto> requests = context.Requests
                    .Include("User").Include("User.Unit").ToList()
                    .Where(r => r.Date.Year == _year)
                    .Select(r => r.Dto()).ToList();
                Connection.Send(new Packet14RequestList(requests));
            }
            return true;
        }

        public override object Clone() {
            return new Packet14RequestListHandler();
        }

        public override string ToString() {
            return null;
        }
    }
}
