using BussinessObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class PartnerDTO
    {
        [JsonPropertyName("id")]
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
        public bool IsDelete { get; set; } = false; 
        [JsonIgnore]
        public DateTime? LastLogin { get; set; }
        [JsonIgnore]
        public string? CreatedBy { get; set; }
        [JsonIgnore]
        public DateTime? LastUpdate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [JsonIgnore]
        public string? UpdatedBy { get; set; }
        public string? ImgUrl { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        [JsonIgnore]
        public string BusinessLicenseNumber { get; set; }
        public int? Role { get; set; }
        public decimal Rating { get; set; } = 0.0m;
        [JsonIgnore]
        public virtual PartnerType? Type { get; set; }
    }
}
