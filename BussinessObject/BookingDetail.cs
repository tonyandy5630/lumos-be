using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public virtual Booking? Booking { get; set; }

        [JsonIgnore]
        public virtual MedicalReport? Report { get; set; }

        [JsonIgnore]
        public virtual ICollection<ServiceBooking> ServiceBookings { get; set; }
    }
}
