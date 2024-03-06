using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestEntity
{
    public class AddPartnerRequest
    {
        public PartnerRequest Partner { get;set; }
        public List<ScheduleRequest> Schedules { get; set; }
    }
}
