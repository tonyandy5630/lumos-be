using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class PendingBookingDTO
    {
        public int BookingId { get; set; }
        public int Status { get; set; }
        public DateTime BookingDate { get; set; }
        public string DisplayName { get; set; }
        [JsonIgnore]
        public string From { get; set; }
        [JsonIgnore]
        public string To { get; set; }
        public int WorkShift { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public string MedicalName { get; set; }
        public int bookingTime { get; set; }
        public List<PartnerServiceDTO> Services { get; set; }
    }
}
