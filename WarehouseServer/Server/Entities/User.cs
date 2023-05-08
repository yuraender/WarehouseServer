using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using WarehouseServer.Server.Entities.Warehouse;
using WarehouseServer.Server.Sql;

namespace WarehouseServer.Server.Entities {
    public class User {

        public int ID {
            get; set;
        }

        [StringLength(100)]
        [Required]
        public string Name {
            get; set;
        }

        [StringLength(16)]
        [Required]
        public string Login {
            get; set;
        }

        [StringLength(60)]
        [Required]
        public string Password {
            get; set;
        }

        [Required]
        public bool IsPasswordChanged {
            get; set;
        }

        [Required]
        public bool IsAdmin {
            get; set;
        }

        [Required]
        public bool IsUnitAdmin {
            get; set;
        }

        public virtual Unit Unit {
            get; set;
        }

        public UserDto Dto() {
            using (ServerContext context = new ServerContext()) {
                return new UserDto {
                    ID = ID,
                    Name = Name,
                    Login = Login,
                    IsPasswordChanged = IsPasswordChanged,
                    IsAdmin = IsAdmin,
                    IsUnitAdmin = Unit != null && IsUnitAdmin,
                    Unit = Unit
                };
            }
        }

        public override string ToString() {
            return $"{Name} (#{ID})";
        }
    }

    public class UserDto {

        public int ID {
            get; set;
        }

        public string Name {
            get; set;
        }

        public string Login {
            get; set;
        }

        public bool IsPasswordChanged {
            get; set;
        }

        public bool IsAdmin {
            get; set;
        }

        public bool IsUnitAdmin {
            get; set;
        }

        public Unit Unit {
            get; set;
        }
    }
}
