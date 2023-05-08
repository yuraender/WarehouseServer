using System.IO;
using System.Linq;
using WarehouseServer.PacketsApi.Contexts;
using WarehouseServer.Server.Entities;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Handlers {
    public abstract class PacketHandlerBase : IPacketHandler {

        public Connection Connection {
            get; set;
        }
        public User User {
            get {
                ClientContext cc = Program.Server.Clients
                    .FirstOrDefault(c => c.Connection.Equals(Connection));
                using (ServerContext context = new ServerContext()) {
                    return cc != null
                         ? context.Users.Include("Unit").FirstOrDefault(u => u.ID == cc.User.ID)
                         : null;
                }
            }
        }

        public virtual void Read(BinaryReader reader) {
        }

        public virtual bool Handle() {
            return false;
        }

        public abstract object Clone();
    }
}
