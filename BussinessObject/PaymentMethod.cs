using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BussinessObject
{
    public partial class PaymentMethod
    {
        public PaymentMethod()
        {
            Bookings = new HashSet<Booking>();
        }

        public int PaymentId { get; set; }
        [JsonIgnore]
        public string? Code { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }
        [JsonIgnore]
        public DateTime? CreateDate { get; set; }
        [JsonIgnore]
        public string? CreatedBy { get; set; }
        [JsonIgnore]
        public DateTime? LastUpdate { get; set; }
        [JsonIgnore]
        public string? UpdatedBy { get; set; }
        [JsonIgnore]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
