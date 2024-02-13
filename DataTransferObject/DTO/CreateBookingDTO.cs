using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class CreateBookingDTO
    {
        public int PaymentId { get; set; }
        public int TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime From { get; set; }
        public string CreatedBy { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public int ReportId { get; set; }
    }

}
