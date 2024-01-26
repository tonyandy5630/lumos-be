using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class AdminConfiguration
    {
        public int AdminConfigId { get; set; }
        public int? ConfigId { get; set; }
        public int? AdminId { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdateBy { get; set; }

        public virtual Admin? Admin { get; set; }
        public virtual SystemConfiguration? Config { get; set; }
    }
}
