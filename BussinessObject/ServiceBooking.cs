using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class ServiceBooking
    {
        public int ServiceBookingId { get; set; }
        public int? PartnerServiceId { get; set; }
        public int? BookingId { get; set; }
        public string Code { get; set; }
        public int? Status { get; set; }
        public int? Price { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual PartnerService PartnerService { get; set; }
    }
}
