using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WarehouseServer.Server.Entities.Warehouse {
    public class Group {

        public int ID {
            get; set;
        }

        [StringLength(50)]
        [Required]
        public string Name {
            get; set;
        }

        [Required]
        public virtual Unit Unit {
            get; set;
        }

        public GroupDto Dto() {
            return new GroupDto {
                ID = ID,
                Name = Name,
                Unit = Unit
            };
        }

        public override string ToString() {
            return $"{Name} (#{ID})";
        }
    }

    public class GroupDto {

        public int ID {
            get; set;
        }

        public string Name {
            get; set;
        }

        public Unit Unit {
            get; set;
        }
    }
}
