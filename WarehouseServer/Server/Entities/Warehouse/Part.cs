using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehouseServer.Server.Entities.Warehouse {
    public class Part {

        public int ID {
            get; set;
        }

        [StringLength(50)]
        [Required]
        public string Name {
            get; set;
        }

        [StringLength(50)]
        [Required]
        public string Type {
            get; set;
        }

        [Required]
        public int Amount {
            get; set;
        }

        [StringLength(10)]
        [Required]
        public string Measure {
            get; set;
        }

        [StringLength(1000)]
        public string Description {
            get; set;
        }

        [Required]
        public virtual Group Group {
            get; set;
        }

        public virtual List<Repair> Repairs {
            get; set;
        }

        public PartDto Dto() {
            return new PartDto {
                ID = ID,
                Name = Name,
                Type = Type,
                Amount = Amount,
                Measure = Measure,
                Description = Description,
                Group = Group.Dto(),
                Repairs = Repairs
            };
        }

        public override string ToString() {
            return $"{Name} {Type} (#{ID})";
        }
    }

    public class PartDto {

        public int ID {
            get; set;
        }

        public string Name {
            get; set;
        }

        public string Type {
            get; set;
        }

        public int Amount {
            get; set;
        }

        public string Measure {
            get; set;
        }

        public string Description {
            get; set;
        }

        public GroupDto Group {
            get; set;
        }

        public List<Repair> Repairs {
            get; set;
        }
    }
}
