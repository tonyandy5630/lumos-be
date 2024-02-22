using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class ServiceCategoryDTO
    {
        public int CategoryId { get; set; }
        public string? Category { get; set; }
        public string? Code { get; set; }
    }
}
