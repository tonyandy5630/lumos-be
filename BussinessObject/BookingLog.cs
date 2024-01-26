using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class BookingLog
    {
        public int BookingLogId { get; set; }
        public int? BookingId { get; set; }
        public int? Status { get; set; }
        public string? Note { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public virtual Booking? Booking { get; set; }
    }
}
