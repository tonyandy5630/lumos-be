using System;
using System.Collections.Generic;

namespace BussinessObject
{
    public partial class FavoritePartner
    {
        public int FavoriteId { get; set; }
        public int? PartnerId { get; set; }
        public int? CustomerId { get; set; }
        public string DisplayName { get; set; }
        public bool? Status { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Partner Partner { get; set; }
    }
}
