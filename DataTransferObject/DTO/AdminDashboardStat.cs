using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class AdminDashboardStat
    {
        public int TotalBookings { get; set; }
        public int TotalMembers { get; set; }
        public int OnGoingBookings { get; set; }
        public int Earning { get; set; }
    }
}
