using BussinessObject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RequestEntity
{
    public class PartnerRequest
    {
        [Required(ErrorMessage = "PartnerName is required")]
        [MaxLength(50, ErrorMessage ="Partner Name is too long")]
        public string PartnerName { get; set; }
        [Required(ErrorMessage = "DisplayName is required")]
        [MaxLength(50, ErrorMessage = "Display Name is too long")]
        public string DisplayName { get; set; }
        [Required(ErrorMessage = "Type is required")]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Not a valid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Not a valid Phone number")]
        public string? Phone { get; set; }

        [Required(ErrorMessage ="Address is required")]
        [MaxLength(100, ErrorMessage = "Address is too long")]
        public string Address { get; set; }
        public string? Description { get; set; }
        public string? BusinessLicenseNumber { get; set; }
    }
}
