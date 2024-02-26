using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestEntity
{
    public  class AddAddressRequest
    {
        public string? displayName { get; set; }
        public string? address1 { get; set; }
        public int? customerId { get; set; }
    }
}
