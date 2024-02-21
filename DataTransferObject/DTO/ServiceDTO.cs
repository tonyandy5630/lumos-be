using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class ServiceDTO
    {
        public int ServiceId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
