using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class ScheduleDTO
    {
        public int ScheduleId { get; set; }
        public int WorkShift { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
    }
}
