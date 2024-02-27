using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class BookingDTO
    {
        public int bookingId { get; set; }
        public List<PartnerServiceDTO> services { get; set; }
        public int status { get; set; }
    }
}
