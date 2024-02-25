using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class PendingBookingDTO
    {
        public int BookingId { get; set; }
        public int Status { get; set; }
        public DateTime BookingDate { get; set; }
        public string PartnerName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public List<PartnerServiceDTO> Services { get; set; }
    }
}
