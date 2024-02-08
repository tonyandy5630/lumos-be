using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestEntity
{
    public class AddPartnerScheduleRequest
    {
        [Required]
        [Range(1, 7, ErrorMessage = "Day of week must be between 1 and 7 (Sunday - Friday)")]
        public int DayOfWeek { get; set; }
        [Required]
        public DateTime From { get; set; }
        [Required]
        //[Compare(nameof(From), ErrorMessage = "To must be greater than From")]
        public DateTime To { get; set; }
        public string Note { get; set; }
    }
}