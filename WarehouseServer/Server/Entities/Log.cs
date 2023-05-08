using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseServer.Server.Entities {
    public class Log {

        public int ID {
            get; set;
        }

        [StringLength(50)]
        [Required]

        public string Packet {
            get; set;
        }

        [StringLength(100)]
        [Required]

        public string Description {
            get; set;
        }

        [Column(TypeName = "DATETIME2")]
        [Required]
        public DateTime Date {
            get; set;
        }

        [Required]
        public virtual User User {
            get; set;
        }

        public override string ToString() {
            return $"{Date} | {Packet} | {User} {Description}";
        }
    }
}
