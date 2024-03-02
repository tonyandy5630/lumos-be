using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class CreateBookingDTO
    {
        public int PartnerId { get; set; }
        public int PaymentId { get; set; }
        public int TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
/*        public DateTime From { get; set; }*/
        public int DayOfWeek { get; set; }
        public int bookingTime { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string? PaymentLinkId { get; set; }
        public List<CartModelDTO> CartModel { get; set; }
    }

}
