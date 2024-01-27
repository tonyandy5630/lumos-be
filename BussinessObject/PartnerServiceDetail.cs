using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BussinessObject
{
    public class PartnerServiceDetail
    {
        public int ServiceId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int? Duration { get; set; }
        public int? Status { get; set; }
        public string? Description { get; set; }
        public int? Price { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }

    }
}
