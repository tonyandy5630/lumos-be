﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RequestEntity
{
    public class PaymentRequest
    {
        public string Name { get; set; }
        [JsonIgnore]
        public int Quantity { get; set; }
        public int Amount { get; set; }
        public int OrderId { get; set; }
        public int TotalAmount { get; set; }
        public string Description { get; set; }
    }
}
