using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class TopBookingSummaryDTO
    {
        public List<TopBookedServiceDTO> TopBookedServices { get; set; }
        public int TotalBookings { get; set; }
        public int ReturnPatients { get; set; }
        public int Operations { get; set; }
        public decimal Earning { get; set; }
    }
}
