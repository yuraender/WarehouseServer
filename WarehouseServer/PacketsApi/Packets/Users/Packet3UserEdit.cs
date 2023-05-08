using DevOne.Security.Cryptography.BCrypt;
using Newtonsoft.Json;
using System.IO;
using WarehouseServer.PacketsApi.Handlers;
using WarehouseServer.Server.Entities;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.PacketsApi.Packets {
    public class Packet3UserEdit : PacketBase {
        public Packet3UserEdit() : base(3) {
        }
    }

    public class Packet3UserEditHandler : PacketHandlerBase {

        private User _user;
        private byte _action;

        public override void Read(BinaryReader reader) {
            _user = JsonConvert.DeserializeObject<User>(reader.ReadString());
            _action = reader.ReadByte();
        }

        public override bool Handle() {
            using (ServerContext context = new ServerContext()) {
                User user = context.Users.Find(_user.ID);
                if (user == null && _action != 0) {
                    return false;
                }
                if (_action == 0) {
                    _user = context.Users.Add(new User {
                        Name = _user.Name,
                        Login = _user.Login,
                        Password = BCryptHelper
                            .HashPassword(_user.Password, BCryptHelper.GenerateSalt()),
                        IsPasswordChanged = false,
                        IsAdmin = _user.IsAdmin,
                        IsUnitAdmin = _user.IsUnitAdmin,
                        Unit = _user.Unit != null ? context.Units.Find(_user.Unit.ID) : null
                    });
                } else if (_action == 1) {
                    if (!string.IsNullOrEmpty(_user.Name)) {
                        user.Name = _user.Name;
                    }
                    if (!string.IsNullOrEmpty(_user.Login)) {
                        user.Login = _user.Login;
                    }
                    if (!string.IsNullOrEmpty(_user.Password)) {
                        user.Password = BCryptHelper
                            .HashPassword(_user.Password, BCryptHelper.GenerateSalt());
                    }
                    user.IsPasswordChanged = _user.IsPasswordChanged;
                    user.IsAdmin = _user.IsAdmin;
                    user.IsUnitAdmin = _user.IsUnitAdmin;
                    context.Entry(user).Reference(u => u.Unit).CurrentValue
                        = _user.Unit != null ? context.Units.Find(_user.Unit.ID) : null;
                } else {
                    context.Users.Remove(user);
                }
                context.SaveChanges();
            }
            return true;
        }

        public override object Clone() {
            return new Packet3UserEditHandler();
        }

        public override string ToString() {
            if (_action == 0) {
                return $"создал пользователя {_user}";
            } else if (_action == 1) {
                return $"изменил информацию о пользователе {_user}";
            } else {
                return $"удалил пользователя {_user}";
            }
        }
    }
}
