using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RequestEntity.Constraint.Constraint;
using Utils.Validation;

namespace RequestEntity
{
    public class UpdatePartnerServiceRequest
    {

        public int ServiceId { get; set; }
        [BadWord(ErrorMessage = "Name contains a forbidden word.")]
        public string? Name { get; set; }
        public int? Duration { get; set; }
        [BadWord(ErrorMessage = "Description contains a forbidden word.")]
        public string? Description { get; set; }
        [Range(PriceConstraint.FLOOR, PriceConstraint.CEIL, ErrorMessage = PriceConstraint.MESSAGE)]
        public int? Price { get; set; }
    }
}
