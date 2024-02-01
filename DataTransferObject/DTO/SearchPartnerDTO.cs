using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class SearchPartnerDTO
    {
        public int PartnerId { get; set; }
        public string? Code { get; set; }
        public string? Email { get; set; }
        public string? PartnerName { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? ImgUrl { get; set; }
        public  PartnerType? Type { get; set; }
        public IEnumerable<PartnerServiceDTO>? PartnerServices { get; set; }
    }
}
