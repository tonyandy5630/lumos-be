﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class MonthlyRevenueDTO : ChartStatDTO
    {
        public List<RevenuePerWeekDTO> Details { get; set; }
    }
}
