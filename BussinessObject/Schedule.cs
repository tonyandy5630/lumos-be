using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class Schedule
    {
        public int ScheduleId { get; set; }
        public int? PartnerId { get; set; }
        public string Code { get; set; }
        public int? DayOfWeek { get; set; }
        public TimeSpan? From { get; set; }
        public TimeSpan? To { get; set; }
        public string Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Partner Partner { get; set; }
    }
}
