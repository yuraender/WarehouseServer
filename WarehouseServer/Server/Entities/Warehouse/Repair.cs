using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseServer.Server.Entities.Warehouse {
    public class Repair {

        public int ID {
            get; set;
        }

        [StringLength(50)]
        [Required]
        public string Organization {
            get; set;
        }

        [Column(TypeName = "DATE")]
        [Required]
        public DateTime DispatchDate {
            get; set;
        }

        [Column(TypeName = "DATE")]
        public DateTime? ReturnDate {
            get; set;
        }

        [JsonIgnore]
        [Required]
        public virtual Part Part {
            get; set;
        }

        public override string ToString() {
            return $"{Part.Name} {Part.Type} (#{ID})";
        }
    }
}
