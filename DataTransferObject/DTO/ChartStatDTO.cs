using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class ChartStatDTO
    {
        public int StatUnit { get; set; } // can be month, year, week
        public int StatValue { get; set; }
    }
}
