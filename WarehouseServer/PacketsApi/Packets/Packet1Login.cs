using DevOne.Security.Cryptography.BCrypt;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using WarehouseServer.PacketsApi.Contexts;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Entities;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet1Login : PacketBase {

        private readonly UserDto _user;

        public Packet1Login(UserDto user) : base(1) {
            _user = user;
        }

        public override void WriteBody(BinaryWriter writer) {
            writer.Write(JsonConvert.SerializeObject(_user));
        }
    }

    public class Packet1LoginHandler : PacketHandlerBase {

        private string _login;
        private string _password;

        public override void Read(BinaryReader reader) {
            _login = reader.ReadString();
            _password = reader.ReadString();
        }

        public override bool Handle() {
            if (Program.Server.Clients.Any(c => c.Connection.Equals(Connection))) {
                return false;
            }
            using (ServerContext context = new ServerContext()) {
                User user = context.Users.Include("Unit").FirstOrDefault(u => u.Login == _login);
                if (user == null || !BCryptHelper.CheckPassword(_password, user.Password)) {
                    Connection.Send(new Packet1Login(null));
                    return false;
                }
                Program.Server.Clients.Add(new ClientContext(Connection, user));
                Connection.Send(new Packet1Login(user.Dto()));
            }
            return true;
        }

        public override object Clone() {
            return new Packet1LoginHandler();
        }

        public override string ToString() {
            ClientContext cc = Program.Server
                .Clients.First(c => c.Connection.Equals(Connection));
            string ip = ((IPEndPoint) cc.Connection
                .Client.Client.RemoteEndPoint).Address.ToString();
            return $"({ip}) авторизовался";
        }
    }
}
