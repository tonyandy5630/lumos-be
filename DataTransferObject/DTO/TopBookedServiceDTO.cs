using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class TopBookedServiceDTO
    {
        public int ServiceId { get; set; }
        public string ServiceCode { get; set; }
        public int PartnerId { get; set; }
        public string ServiceName { get; set; }
        public string PartnerName { get; set; }
        public decimal Rating { get; set; }
        public int NumberOfBooking { get; set; }
        public decimal Price { get; set; }
    }
}
