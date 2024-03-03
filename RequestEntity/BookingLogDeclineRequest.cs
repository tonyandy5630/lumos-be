using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestEntity
{
    public class BookingLogDeclineRequest
    {
        public int BookingId { get; set; }
        public string CancellationReason { get; set; }
    }
}
