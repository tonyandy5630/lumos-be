using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Validation;

namespace RequestEntity
{
    public class ScheduleRequest
    {

        [Required(ErrorMessage = "WorkShift is required")]
        [Range(1, 3, ErrorMessage = "WorkShift must be between 1 and 3")]
        public int WorkShift { get; set; }

        [Required(ErrorMessage = "DayOfWeek is required")]
        [Range(2, 8, ErrorMessage = "DayOfWeek must be between 2 and 8")]
        public int DayOfWeek { get; set; }
        [BadWord(ErrorMessage = "Note contains a forbidden word.")]
        public string? Note { get; set; }
    }
}
