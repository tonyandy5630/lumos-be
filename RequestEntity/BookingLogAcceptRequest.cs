using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RequestEntity
{
    public class BookingLogAcceptRequest
    {
        [JsonIgnore]
        public int BookingId { get; set; }
        public string? PaymentLinkId { get; set; }
    }
}
