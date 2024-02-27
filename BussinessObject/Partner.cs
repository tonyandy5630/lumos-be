using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BussinessObject
{
    public partial class Partner
    {
        public Partner()
        {
            FavoritePartners = new HashSet<FavoritePartner>();
            PartnerServices = new HashSet<PartnerService>();
            Schedules = new HashSet<Schedule>();
        }

        public int PartnerId { get; set; }
        public int TypeId { get; set; }
        public string? Code { get; set; }
        [Required]
        public string Email { get; set; }
        public string PartnerName { get; set; }
        public string DisplayName { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        [JsonIgnore]
        public int? Status { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? ImgUrl { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public string BusinessLicenseNumber { get; set; }
        public int? Role { get; set; }

        public virtual PartnerType? Type { get; set; }
        [JsonIgnore]
        public virtual ICollection<FavoritePartner> FavoritePartners { get; set; }
        public virtual ICollection<PartnerService> PartnerServices { get; set; }
        [JsonIgnore]
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
