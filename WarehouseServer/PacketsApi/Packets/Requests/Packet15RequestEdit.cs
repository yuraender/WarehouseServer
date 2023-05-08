using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Entities.Warehouse;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet15RequestEdit : PacketBase {
        public Packet15RequestEdit() : base(15) {
        }
    }

    public class Packet15RequestEditHandler : PacketHandlerBase {

        private Request _request;
        private byte _action;

        public override void Read(BinaryReader reader) {
            _request = JsonConvert.DeserializeObject<Request>(reader.ReadString());
            _action = reader.ReadByte();
        }

        public override bool Handle() {
            if (!User.IsAdmin) {
                return false;
            }
            using (ServerContext context = new ServerContext()) {
                Request request = context.Requests.Find(_request.ID);
                if (request == null && _action != 0) {
                    return false;
                }
                if (_action == 0) {
                    List<int> ids = context.Requests
                        .Where(r => r.Date.Year == DateTime.Now.Year).ToList()
                        .Select(r => r.Number).ToList();
                    _request = context.Requests.Add(new Request {
                        Number = ids.Count != 0 ? ids.Max() + 1 : 1,
                        Date = DateTime.Now,
                        RequestInfo = _request.RequestInfo,
                        User = context.Users.Find(_request.User.ID)
                    });
                } else if (_action == 1) {
                    request.RequestInfo = _request.RequestInfo;
                } else {
                    context.Requests.Remove(request);
                }
                context.SaveChanges();
            }
            return true;
        }

        public override object Clone() {
            return new Packet15RequestEditHandler();
        }

        public override string ToString() {
            if (_action == 0) {
                return $"создал заявку в с.с. {_request}";
            } else if (_action == 1) {
                return $"изменил заявку в с.с. {_request}";
            } else {
                return $"удалил заявку в с.с. {_request}";
            }
        }
    }
}
