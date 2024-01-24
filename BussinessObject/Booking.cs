using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class Booking
    {
        public Booking()
        {
            BookingLogs = new HashSet<BookingLog>();
            ServiceBookings = new HashSet<ServiceBooking>();
        }

        public int BookingId { get; set; }
        public int? ReportId { get; set; }
        public int? PaymentId { get; set; }
        public string Code { get; set; }
        public int? TotalPrice { get; set; }
        public DateTime? BookingDate { get; set; }
        public TimeSpan? From { get; set; }
        public TimeSpan? To { get; set; }
        public string FeedbackPartner { get; set; }
        public string FeedbackLumos { get; set; }
        public string FeedbackImage { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public virtual PaymentMethod Payment { get; set; }
        public virtual MedicalReport Report { get; set; }
        public virtual ICollection<BookingLog> BookingLogs { get; set; }
        public virtual ICollection<ServiceBooking> ServiceBookings { get; set; }
    }
}
