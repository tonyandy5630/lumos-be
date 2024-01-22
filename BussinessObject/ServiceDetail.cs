using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class ServiceDetail
    {
        public int DetailId { get; set; }
        public int? ServiceId { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ServiceCategory Category { get; set; }
        public virtual PartnerService Service { get; set; }
    }
}
