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
        public string DisplayName { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int DayOfWeek { get; set; }
        public int WorkShift { get; set; }
        public List<PartnerServiceDTO> Services { get; set; }
    }
}
