using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class Partner
    {
        public Partner()
        {
            FavoritePartners = new HashSet<FavoritePartner>();
            PartnerServices = new HashSet<PartnerServiceObject>();
            Schedules = new HashSet<Schedule>();
        }

        public int PartnerId { get; set; }
        public int? TypeId { get; set; }
        public string? Code { get; set; }
        public string? Email { get; set; }
        public string? PartnerName { get; set; }
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? ImgUrl { get; set; }
        public string? RefreshToken { get; set; }
        public string? BusinessLicenseNumber { get; set; }

        public virtual PartnerType? Type { get; set; }
        public virtual ICollection<FavoritePartner> FavoritePartners { get; set; }
        public virtual ICollection<PartnerServiceObject> PartnerServices { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
    }
}
