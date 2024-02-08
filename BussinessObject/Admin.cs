using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BussinessObject
{
    public partial class Admin
    {
        public Admin()
        {
            AdminConfigurations = new HashSet<AdminConfiguration>();
        }

        public int AdminId { get; set; }
        public string? Code { get; set; }
        [Required]
        public string Email { get; set; }
        [JsonIgnore]
        public string? Password { get; set; }
        public int? Role { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? ImgUrl { get; set; }
        public string? RefreshToken { get; set; }

        public virtual ICollection<AdminConfiguration> AdminConfigurations { get; set; }
    }
}
