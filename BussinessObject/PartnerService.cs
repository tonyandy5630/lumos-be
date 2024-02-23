using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using RequestEntity.Constraint;
using static RequestEntity.Constraint.Constraint;

namespace BussinessObject
{
    public partial class PartnerService
    {

        public PartnerService()
        {
            ServiceBookings = new HashSet<ServiceBooking>();
            ServiceDetails = new HashSet<ServiceDetail>();
        }

        public int ServiceId { get; set; }
        [JsonIgnore]
        public int? PartnerId { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Code { get; set; }
        public int? Duration { get; set; }
        public int? Status { get; set; }
        public string? Description { get; set; }

        [Required]
        [Range(PriceConstraint.FLOOR, PriceConstraint.CEIL, ErrorMessage = PriceConstraint.MESSAGE)]
        public int Price { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }
        [JsonIgnore]
        public virtual Partner? Partner { get; set; }

        [JsonIgnore]
        public virtual ICollection<ServiceBooking> ServiceBookings { get; set; }

        [JsonIgnore]
        public virtual ICollection<ServiceDetail> ServiceDetails { get; set; }
        public decimal Rating { get; set; }

    }
}
