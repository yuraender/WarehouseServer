using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WarehouseServer.PacketsApi.Contexts;
using WarehouseServer.PacketsApi.Handlers;

namespace WarehouseServer.Server {
    public class WarehouseServer {

        private Thread _thread;
        private TcpListener _listener;
        private bool _started = false;

        public int Port {
            get; private set;
        }
        public PacketHandlerStorage Handlers {
            get; set;
        }
        public List<ClientContext> Clients {
            get; private set;
        }

        public WarehouseServer(int port, PacketHandlerStorage handlers) {
            Port = port;
            Handlers = handlers;
            Clients = new List<ClientContext>();
        }

        public void Run() {
            _thread = new Thread(() => {
                try {
                    _listener = new TcpListener(IPAddress.Any, Port);
                    _listener.Start();
                    _started = true;

                    while (_started) {
                        TcpClient client = _listener.AcceptTcpClient();
                        Connection connection = new Connection(client, Handlers);
                        connection.Run();
                    }
                } catch (SocketException ex) {
                    Program.Log($"{ex.GetType()}: {ex.Message}");
                    Environment.Exit(0);
                }
            });
            _thread.Start();
        }
    }
}
