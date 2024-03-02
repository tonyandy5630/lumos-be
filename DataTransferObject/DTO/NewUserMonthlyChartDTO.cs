using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class NewUserMonthlyChartDTO
    {
        public List<int?> newCustomerMonthly { get; set; }
        public List<int?> newPartnerMonthly { get; set; }
    }
}
