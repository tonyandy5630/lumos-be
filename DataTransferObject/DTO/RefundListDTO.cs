using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class RefundListDTO
    {
        public int BookingId { get; set; }
        public int Status { get; set; }
        public int? TotalPrice { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? CancelationReason { get; set;}
    }
}
