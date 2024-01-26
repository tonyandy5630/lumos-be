using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class ServiceCategory
    {
        public ServiceCategory()
        {
            ServiceDetails = new HashSet<ServiceDetail>();
        }

        public int CategoryId { get; set; }
        public string? Category { get; set; }
        public string? Code { get; set; }
        public int? Status { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual ICollection<ServiceDetail> ServiceDetails { get; set; }
    }
}
