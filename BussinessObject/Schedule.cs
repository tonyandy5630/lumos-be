using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BussinessObject
{
    public partial class Schedule
    {
        public int ScheduleId { get; set; }
        public int? PartnerId { get; set; }
        public string? Code { get; set; }
        [JsonIgnore]
        public TimeSpan From { get; set; }
        [JsonIgnore]
        public TimeSpan To { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }
        public int DayOfWeek { get; set; }
        public int WorkShift { get;set; }

        public virtual Partner? Partner { get; set; }
    }
}
