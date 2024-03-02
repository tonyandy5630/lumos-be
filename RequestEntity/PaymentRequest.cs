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

        public int OrderId { get; set; }
        public List<ItemRequest> Items { get; set; }
        public int TotalAmount { get; set; }
        public string Description { get; set; }
    }
}
