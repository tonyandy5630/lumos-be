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
        public List<PartnerServiceDTO> Services { get; set; }
    }
}
