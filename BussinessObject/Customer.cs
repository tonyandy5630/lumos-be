using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class Customer
    {
        public Customer()
        {
            FavoritePartners = new HashSet<FavoritePartner>();
            MedicalReports = new HashSet<MedicalReport>();
        }

        public int CustomerId { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool? Gender { get; set; }
        public int? Pronounce { get; set; }
        public DateTime? Dob { get; set; }
        public string RefreshToken { get; set; }
        public int? Status { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<FavoritePartner> FavoritePartners { get; set; }
        public virtual ICollection<MedicalReport> MedicalReports { get; set; }
    }
}
