using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class IncomingBookingDTO
    {
        public int BookingId { get; set; }
        public string Status { get; set; }
        public string? Partner { get; set; }
        public DateTime BookingDate { get; set; }
        public int bookingTime { get; set; } // workshift
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public List<MedicalServiceDTO> MedicalServices { get; set; }
    }
}
