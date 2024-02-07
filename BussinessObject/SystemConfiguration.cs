using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BussinessObject
{
    public partial class SystemConfiguration
    {
        public SystemConfiguration()
        {
            AdminConfigurations = new HashSet<AdminConfiguration>();
        }

        public int ConfigId { get; set; }
        public string Config { get; set; }
        public string Field { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? UpdatedBy { get; set; }
        [JsonIgnore]
        public virtual ICollection<AdminConfiguration> AdminConfigurations { get; set; }
    }
}
