using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class PaymentMethod
    {
        public PaymentMethod()
        {
            Bookings = new HashSet<Booking>();
        }

        public int PaymentId { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
