using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class BillMedicalDTO
    {
        public string MedicalName { get; set; }

        public List<BillServiceDTO> Services { get; set; }
    }
}
