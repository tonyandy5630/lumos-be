using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestEntity
{
    public class ItemRequest
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int Amount { get; set; }
    }
}
