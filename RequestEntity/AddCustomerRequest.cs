using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestEntity
{
    public class AddCustomerRequest
    {
        public string? Email { get; set; }
        public string? Fullname { get; set; }
        public string? Password { get; set; }
        public string? ImgUrl { get; set; }
    }
}
