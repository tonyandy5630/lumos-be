using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class Admin
    {
        public Admin()
        {
            AdminConfigurations = new HashSet<AdminConfiguration>();
        }

        public int AdminId { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? Role { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string UpdatedBy { get; set; }
        public string ImgUrl { get; set; }

        public virtual ICollection<AdminConfiguration> AdminConfigurations { get; set; }
    }
}
