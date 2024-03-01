﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BussinessObject
{
    public partial class Customer
    {
        public Customer()
        {
            Addresses = new HashSet<Address>();
            FavoritePartners = new HashSet<FavoritePartner>();
            MedicalReports = new HashSet<MedicalReport>();
        }

        public int CustomerId { get; set; }
        public string? Code { get; set; }
        [Required]
        public string Email { get; set; }
        public string? Fullname { get; set; }
        [Required]
        [JsonIgnore]
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public int? Pronounce { get; set; }
        public int? Status { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastUpdate { get; set; }
        [JsonIgnore]
        public string? UpdateBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ImgUrl { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        [JsonIgnore]
        public int? Role { get; set; }
        [JsonIgnore]
        public virtual ICollection<Address> Addresses { get; set; }
        [JsonIgnore]

        public virtual ICollection<FavoritePartner> FavoritePartners { get; set; }
        [JsonIgnore]
        public virtual ICollection<MedicalReport> MedicalReports { get; set; }
    }
}
