using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseServer.Server.Entities.Warehouse {
    public class Request {

        public int ID {
            get; set;
        }

        [Required]
        public int Number {
            get; set;
        }

        [Required]
        public DateTime Date {
            get; set;
        }

        [Column(TypeName = "NVARCHAR(MAX)")]
        [Required]
        public string RequestInfo {
            get; set;
        }

        [Required]
        public virtual User User {
            get; set;
        }

        public RequestDto Dto() {
            return new RequestDto {
                ID = ID,
                Number = Number,
                Date = Date,
                RequestInfo = RequestInfo,
                User = User.Dto()
            };
        }

        public override string ToString() {
            return $"Заявка #{ID} ({Date.Year}-{Number})";
        }
    }

    public class RequestDto {

        public int ID {
            get; set;
        }

        public int Number {
            get; set;
        }

        public DateTime Date {
            get; set;
        }

        public string RequestInfo {
            get; set;
        }

        public UserDto User {
            get; set;
        }
    }
}
