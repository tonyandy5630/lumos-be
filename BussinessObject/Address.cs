using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BussinessObject
{
    public partial class Address
    {
        public int AddressId { get; set; }
        public string? Code { get; set; }
        public string? DisplayName { get; set; }
        public string? Address1 { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }
        public int? CustomerId { get; set; }
        [JsonIgnore]

        public virtual Customer? Customer { get; set; }
    }
}
