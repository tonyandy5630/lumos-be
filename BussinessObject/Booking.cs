using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class Booking
    {
        public Booking()
        {
            PartnerServices = new HashSet<PartnerService>();
        }

        public int BookingId { get; set; }
        public int? PartnerServiceId { get; set; }
        public int? ReportId { get; set; }
        public int? PaymentId { get; set; }
        public string Code { get; set; }
        public int? Status { get; set; }
        public DateTime? BookingDate { get; set; }
        public TimeSpan? From { get; set; }
        public TimeSpan? To { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public int? TotalPrice { get; set; }
        public string FeedbackLumos { get; set; }
        public string FeedbackPartner { get; set; }
        public string FeedbackImage { get; set; }

        public virtual PaymentMethod Payment { get; set; }
        public virtual MedicalReport Report { get; set; }
        public virtual ICollection<PartnerService> PartnerServices { get; set; }
    }
}
