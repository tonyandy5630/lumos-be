﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestEntity
{
    public class AddScheduleRequest
    {
       public int partnerId {  get; set; }
       public List<ScheduleRequest> schedules { get; set; }


    }

    public class ScheduleRequest
    {
        public int WorkShift { get; set; }
        public int DayOfWeek { get; set; }
        public string? Note { get; set; }
    }
}
