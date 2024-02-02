using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class CustomerServiceDTO
    {
        public string? Code { get; set; }
        public string? Email { get; set; }
        public string? Fullname { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public int? Pronounce { get; set; }
        public int? Status { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ImgUrl { get; set; }
        public string? RefreshToken { get; set; }
    }
}
