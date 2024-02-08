using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class Schedule
    {
        public int ScheduleId { get; set; }
        public int? PartnerId { get; set; }
        public string? Code { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }
        public int? DayOfWeek { get; set; }

        public virtual Partner? Partner { get; set; }
    }
}
