using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class HistoryLog
    {
        public int LogId { get; set; }
        public DateTime? Date { get; set; }
        public string? Message { get; set; }
    }
}
