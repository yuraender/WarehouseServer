using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet12Log : PacketBase {

        private readonly List<string> _logs;

        public Packet12Log(List<string> logs) : base(12) {
            _logs = logs;
        }

        public override void WriteBody(BinaryWriter writer) {
            writer.Write(JsonConvert.SerializeObject(_logs));
        }
    }

    public class Packet12LogHandler : PacketHandlerBase {

        private DateTime _from;
        private DateTime _to;

        public override void Read(BinaryReader reader) {
            _from = DateTime.Parse(reader.ReadString());
            _to = DateTime.Parse(reader.ReadString());
        }

        public override bool Handle() {
            if (!User.IsAdmin) {
                return false;
            }
            using (ServerContext context = new ServerContext()) {
                List<string> logs = context.Logs.Include("User").ToList()
                    .Where(l => l.Date >= _from)
                    .Where(l => l.Date <= _to).ToList()
                    .Select(l => l.ToString()).ToList();
                Connection.Send(new Packet12Log(logs));
            }
            return true;
        }

        public override object Clone() {
            return new Packet12LogHandler();
        }

        public override string ToString() {
            return null;
        }
    }
}
