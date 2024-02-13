using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class Booking
    {
        public Booking()
        {
            BookingDetails = new HashSet<BookingDetail>();
            BookingLogs = new HashSet<BookingLog>();
        }

        public int BookingId { get; set; }
        public int? PaymentId { get; set; }
        public string? Code { get; set; }
        public int? TotalPrice { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? From { get; set; }
        public string? FeedbackPartner { get; set; }
        public string? FeedbackLumos { get; set; }
        public string? FeedbackImage { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? Address { get; set; }

        public virtual PaymentMethod? Payment { get; set; }
        public virtual ICollection<BookingDetail> BookingDetails { get; set; }
        public virtual ICollection<BookingLog> BookingLogs { get; set; }
    }
}
