using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RequestEntity
{
    public class PaymentRequest
    {
        public string buyerName { get; set; }
        public string buyerEmail { get; set; }
        public string buyerPhone { get; set; }
        public string buyerAddress { get; set; }
        public int OrderId { get; set; }
        public List<ItemRequest> Items { get; set; }
        public int TotalAmount { get; set; }
        public string Description { get; set; }
        public int expiredAt { get; set; }
    }
}
