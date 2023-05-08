using System;
using System.Net;
using System.Net.Sockets;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.PacketsApi.Packets;
using WarehouseServer.PacketsApi.Processors;
using WarehouseServer.Server.Entities;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Contexts {
    public class Connection {

        private readonly InputProcessor _inputProccessor;
        private readonly OutputProccessor _outputProccessor;

        public TcpClient Client {
            get; private set;
        }

        public Connection(TcpClient client, PacketHandlerStorage handlers) {
            Client = client;
            NetworkStream stream = client.GetStream();
            _inputProccessor = new InputProcessor(this, stream, handlers);
            _outputProccessor = new OutputProccessor(stream);
        }

        public void Run() {
            _inputProccessor.Run();
            _outputProccessor.Run();
        }

        public void Stop() {
            _inputProccessor.Stop();
            _outputProccessor.Stop();
        }

        public void Send(PacketBase packet) {
            _outputProccessor.Send(packet);
        }

        public void Receive(PacketHandlerBase handler) {
            handler.Connection = this;
            if (handler.Handle() && handler.ToString() != null) {
                using (ServerContext context = new ServerContext()) {
                    Log log = new Log() {
                        Packet = handler.GetType().Name.Replace("Handler", ""),
                        Description = handler.ToString(),
                        Date = DateTime.Now,
                        User = context.Users.Find(handler.User.ID)
                    };
                    if (log.Packet != "Packet1Login") {
                        context.Logs.Add(log);
                        context.SaveChanges();
                    }
                    Console.WriteLine(log.ToString());
                }
            }
        }

        public override bool Equals(object obj) {
            if (!(obj is Connection)) {
                return false;
            }
            string ip = ((IPEndPoint) Client.Client.RemoteEndPoint).Address.ToString();
            string connIp = ((IPEndPoint)
                ((Connection) obj).Client.Client.RemoteEndPoint).Address.ToString();
            return ip == connIp;
        }
    }
}
