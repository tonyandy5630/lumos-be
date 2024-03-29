﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BussinessObject
{
    public partial class PartnerType
    {
        public PartnerType()
        {
            Partners = new HashSet<Partner>();
        }
        [JsonIgnore]
        public int TypeId { get; set; }
        [JsonIgnore]
        public string? Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? Status { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }

        public virtual ICollection<Partner> Partners { get; set; }
    }
}
