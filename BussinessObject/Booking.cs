using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
        public DateTime BookingDate { get; set; }
        public DateTime? From { get; set; }
        public string? FeedbackPartner { get; set; }
        public string? FeedbackLumos { get; set; }
        public string? FeedbackImage { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string Address { get; set; }
        public int bookingTime { get; set; }
        public decimal? Rating { get; set; }
        public string? PaymentLinkId { get; set; }
        [JsonIgnore]
        public virtual PaymentMethod? Payment { get; set; }
        [JsonIgnore]
        public virtual ICollection<BookingDetail> BookingDetails { get; set; }
        [JsonIgnore]

        public virtual ICollection<BookingLog> BookingLogs { get; set; }
    }
}
