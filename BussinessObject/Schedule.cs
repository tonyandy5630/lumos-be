using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BussinessObject
{
    public partial class Schedule
    {
        public int ScheduleId { get; set; }
        public int? PartnerId { get; set; }
        public string? Code { get; set; }

        [Required]
        public DateTime? From { get; set; }

        [Required]
        // to > from
        //[Compare(nameof(From), ErrorMessage = "To must be greater than From")]
        public DateTime? To { get; set; }

        public string? Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }

        [Required]
        [Range(1, 7, ErrorMessage = "Day of week must be between 1 and 7 (Sunday - Friday)")]
        public int? DayOfWeek { get; set; }

        [JsonIgnore]
        public virtual Partner? Partner { get; set; }
    }
}
