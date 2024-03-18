using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utils.Validation;

namespace DataTransferObject.DTO
{
    public class CreateBookingDTO
    {
        public int PartnerId { get; set; }
        public int PaymentId { get; set; }
        [JsonIgnore]
        public int TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        /*        public DateTime From { get; set; }*/
        [Required(ErrorMessage = "DayOfWeek is required")]
        [Range(2, 8, ErrorMessage = "DayOfWeek must be between 2 and 8")]
        public int DayOfWeek { get; set; }
        [Required(ErrorMessage = "WorkShift is required")]
        [Range(1, 3, ErrorMessage = "WorkShift must be between 1 and 3")]
        public int bookingTime { get; set; }
        public string Address { get; set; }
        [BadWord(ErrorMessage = "Note contains a forbidden word.")]
        public string Note { get; set; }
        [JsonIgnore]
        public string? PaymentLinkId { get; set; }
        public List<CartModelDTO> CartModel { get; set; }
    }

}
