using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class PartnerType
    {
        public int PartnertypeId { get; set; }
        public int? PartnerId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual Partner Partner { get; set; }
    }
}
