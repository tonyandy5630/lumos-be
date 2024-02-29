using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class ServiceBooking
    {
        public int ServiceBookingId { get; set; }
        public int? ServiceId { get; set; }
        public int DetailId { get; set; }
        public int? Price { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual BookingDetail? Detail { get; set; }
        public virtual PartnerService? Service { get; set; }
    }
}
