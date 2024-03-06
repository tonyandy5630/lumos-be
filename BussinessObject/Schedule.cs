using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace BussinessObject
{
    public partial class Schedule
    {
        public int ScheduleId { get; set; }
        [JsonIgnore]
        public int PartnerId { get; set; }
        public string? Code { get; set; }
        [JsonIgnore]
        public TimeSpan From { get; set; }
        [JsonIgnore]
        public TimeSpan To { get; set; }
        public string? Note { get; set; }
        [JsonIgnore]
        public DateTime? CreatedDate { get; set; }
        [JsonIgnore]
        public string? CreatedBy { get; set; }
        [JsonIgnore]
        public DateTime? LastUpdate { get; set; }
        [JsonIgnore]
        public string? UpdatedBy { get; set; }
        public int DayOfWeek { get; set; }
        public int WorkShift { get;set; }
        [JsonIgnore]
        public virtual Partner? Partner { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Schedule schedule = (Schedule)obj;
            return DayOfWeek == schedule.DayOfWeek && WorkShift == schedule.WorkShift;
        }
        public override int GetHashCode()
        {
            return (DayOfWeek, WorkShift).GetHashCode();
        }
    }
}
