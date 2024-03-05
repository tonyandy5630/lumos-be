using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RequestEntity
{
    public class ChangToCompleteRequest
    {
        [JsonIgnore]
        public int BookingId { get; set; }

        public string reason { get; set; }

        public FeedbackRequest feedback { get; set; }
    }
}
