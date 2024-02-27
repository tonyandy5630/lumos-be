using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class MedicalServiceDTO
    {
        public string MedicalName { get; set; }
        public List<PartnerServiceDTO> Services { get; set; }
    }
}
