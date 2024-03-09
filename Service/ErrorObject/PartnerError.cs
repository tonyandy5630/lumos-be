using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ErrorObject
{
    public class PartnerError
    {
        public string? PartnerName { get; set; }
        public string? DisplayName { get; set; }
        public int? TypeId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? BusinessLicenseNumber { get; set; }
        public string? Schedule { get; set; }
    }
}
