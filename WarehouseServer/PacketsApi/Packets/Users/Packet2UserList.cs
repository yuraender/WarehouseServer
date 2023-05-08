using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Entities;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet2UserList : PacketBase {

        private readonly List<UserDto> _users;

        public Packet2UserList(List<UserDto> users) : base(2) {
            _users = users;
        }

        public override void WriteBody(BinaryWriter writer) {
            writer.Write(JsonConvert.SerializeObject(_users));
        }
    }

    public class Packet2UserListHandler : PacketHandlerBase {
        public override bool Handle() {
            using (ServerContext context = new ServerContext()) {
                List<UserDto> users = context.Users
                    .Include("Unit").ToList()
                    .Select(u => u.Dto()).ToList();
                Connection.Send(new Packet2UserList(users));
            }
            return true;
        }

        public override object Clone() {
            return new Packet2UserListHandler();
        }

        public override string ToString() {
            return null;
        }
    }
}
