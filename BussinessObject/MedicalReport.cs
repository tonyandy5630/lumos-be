using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class MedicalReport
    {
        public MedicalReport()
        {
            BookingDetails = new HashSet<BookingDetail>();
        }

        public int ReportId { get; set; }
        public int? CustomerId { get; set; }
        public string? Code { get; set; }
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public DateTime? Dob { get; set; }
        public bool? Gender { get; set; }
        public int? Pronounce { get; set; }
        public int? BloodType { get; set; }
        public string? Note { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual ICollection<BookingDetail> BookingDetails { get; set; }
    }
}
