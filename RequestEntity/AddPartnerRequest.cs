using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestEntity
{
    public class AddPartnerRequest
    {
        [Required]
        public string PartnerName { get; set; }
        public string? DisplayName { get; set; }
        [Required]
        public int TypeId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string Address { get; set; }
        public string? Description { get; set; }
        public string? BusinessLicenseNumber { get; set; }
    }
}
