using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class PartnerService
    {
        public PartnerService()
        {
            ServiceBookings = new HashSet<ServiceBooking>();
            ServiceDetails = new HashSet<ServiceDetail>();
        }

        public int ServiceId { get; set; }
        public int? PartnerId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int? Duration { get; set; }
        public int? Status { get; set; }
        public string? Description { get; set; }
        public int? Price { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual Partner? Partner { get; set; }
        public virtual ICollection<ServiceBooking> ServiceBookings { get; set; }
        public virtual ICollection<ServiceDetail> ServiceDetails { get; set; }
    }
}
