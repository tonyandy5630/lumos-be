using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestEntity
{
    public class AddPartnerServiceResquest
    {
        public string Name { get; set; }
        public string? Code { get; set; }
        public int? Duration { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public IEnumerable<int> Categories { get; set; }
    }
}
