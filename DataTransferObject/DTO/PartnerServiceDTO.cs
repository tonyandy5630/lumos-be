using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class PartnerServiceDTO
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public string? Code { get; set; }
        public int? Duration { get; set; }
        public int? Status { get; set; }
        public string? Description { get; set; }
        public int? Price { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }
        public int BookedQuantity { get; set; }
        public IEnumerable<ServiceCategoryDTO>? Categories { get; set; }

    }
}
