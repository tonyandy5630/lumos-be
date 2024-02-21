using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public  class CartModelDTO
    {
        public int ReportId { get; set; }
        public List<ServiceDTO> Services { get; set; }
    }
}
