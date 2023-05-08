using WarehouseServer.Server.Entities;

namespace WarehouseServer.PacketsApi.Contexts {
    public class ClientContext {

        public Connection Connection {
            get; private set;
        }
        public User User {
            get; private set;
        }

        public ClientContext(Connection connection, User user) {
            Connection = connection;
            User = user;
        }
    }
}
