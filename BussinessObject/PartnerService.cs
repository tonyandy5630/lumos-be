using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace BussinessObject
{
    public partial class PartnerService
    {
        public PartnerService()
        {
            ServiceBookings = new HashSet<ServiceBooking>();
            ServiceDetails = new HashSet<ServiceDetail>();
            ServiceCategories = new HashSet<ServiceCategory>();
        }

        public int ServiceId { get; set; }
        [JsonIgnore]
        public int? PartnerId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int? Duration { get; set; }
        public int? Status { get; set; }
        public string? Description { get; set; }
        public int? Price { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual Partner? Partner { get; set; }
        [JsonIgnore]
        public virtual ICollection<ServiceBooking> ServiceBookings { get; set; }
        [JsonIgnore]

        public virtual ICollection<ServiceDetail> ServiceDetails { get; set; }

        public virtual ICollection<ServiceCategory> ServiceCategories { get; set; }
    }
}
