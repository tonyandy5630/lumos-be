using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class BillDTO
    {
        public string BookingCode { get; set; }
        public int Status { get; set; }
        public string? PartnerName { get; set; }
        public int? TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public int bookingTime { get; set; } // workshift
        public string? Note { get; set; }
    }
}
