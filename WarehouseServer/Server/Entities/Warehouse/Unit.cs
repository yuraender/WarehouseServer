using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WarehouseServer.Server.Entities.Warehouse {
    public class Unit {

        public int ID {
            get; set;
        }

        [StringLength(50)]
        [Required]
        public string Name {
            get; set;
        }

        public override string ToString() {
            return $"{Name} (#{ID})";
        }
    }
}
