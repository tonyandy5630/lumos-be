using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class PartnerServiceDTO
    {
        [JsonIgnore]
        public int ServiceId { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string? Code { get; set; }
        public int? Duration { get; set; }
        public int? Status { get; set; }
        public string? Description { get; set; }
        public int? Price { get; set; }
        [JsonIgnore]
        public DateTime? CreatedDate { get; set; }
        [JsonIgnore]
        public DateTime? LastUpdate { get; set; }
        [JsonIgnore]
        public string? UpdatedBy { get; set; }
        public int BookedQuantity { get; set; }
        public decimal? Rating { get; set; }
        public IEnumerable<ServiceCategoryDTO>? Categories { get; set; }

    }
}
