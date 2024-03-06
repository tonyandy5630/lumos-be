using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestEntity
{
    public class FeedbackRequest
    {
        [Required(ErrorMessage = "Cần có rating")]
        public double rating { get; set; }
        public string? feedbackPartner { get; set; }
        public string? feedbackLumos { get; set; }
        public string? feedbackImage { get; set; }
    }
}
