using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class BookingDetail
    {
        public BookingDetail()
        {
            ServiceBookings = new HashSet<ServiceBooking>();
        }

        public int DetailId { get; set; }
        public int? BookingId { get; set; }
        public int? ReportId { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public virtual Booking? Booking { get; set; }
        public virtual MedicalReport? Report { get; set; }
        public virtual ICollection<ServiceBooking> ServiceBookings { get; set; }
    }
}
