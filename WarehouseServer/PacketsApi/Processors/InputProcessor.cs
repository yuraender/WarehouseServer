using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WarehouseServer.PacketsApi.Contexts;
using WarehouseServer.PacketsApi.Handlers;

namespace WarehouseServer.PacketsApi.Processors {
    public class InputProcessor {

        private Thread _thread;
        private readonly Connection _connection;
        private readonly BinaryReader _reader;
        private bool _started = false;

        public PacketHandlerStorage Handlers {
            get; private set;
        }

        public InputProcessor(Connection connection,
                              NetworkStream stream, PacketHandlerStorage handlers) {
            Handlers = handlers;
            _connection = connection;
            _reader = new BinaryReader(stream);
        }

        private void _handlePacket() {
            bool existClient = Program.Server.Clients.Any(c => c.Connection.Equals(_connection));
            try {
                int id = _reader.ReadInt32();
                if (id == 1 || existClient) {
                    PacketHandlerBase handler = Handlers.GetHandlerById(id);
                    handler.Read(_reader);
                    _connection.Receive(handler);
                }
            } catch (Exception ex) {
                if (existClient) {
                    ClientContext cc = Program.Server
                        .Clients.First(c => c.Connection.Equals(_connection));
                    string ip = ((IPEndPoint) cc.Connection
                        .Client.Client.RemoteEndPoint).Address.ToString();
                    Program.Log($"{cc.User} ({ip}) разорвал соединение");
                    Program.Server.Clients.RemoveAll(c => c.Connection.Equals(cc.Connection));
                }
                //Console.WriteLine(ex.ToString());
                _connection.Stop();
            }
        }

        public void Run() {
            _thread = new Thread(() => {
                while (!_started) {
                    _handlePacket();
                }
            });
            _thread.Start();
        }

        public void Stop() {
            _reader.Close();
            _thread.Interrupt();
            _thread.Abort();
        }
    }
}
