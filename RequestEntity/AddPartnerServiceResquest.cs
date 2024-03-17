using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Validation;
using static RequestEntity.Constraint.Constraint;

namespace RequestEntity
{
    public class AddPartnerServiceResquest
    {
        [Required]
        [BadWord(ErrorMessage = "Name contains a forbidden word.")]
        public string Name { get; set; }
        public int? Duration { get; set; }
        [BadWord(ErrorMessage = "Description contains a forbidden word.")]
        public string? Description { get; set; }
        [Required]
        [Range(PriceConstraint.FLOOR,PriceConstraint.CEIL, ErrorMessage = PriceConstraint.MESSAGE)]
        public int Price { get; set; }
        public IEnumerable<int> Categories { get; set; }
    }
}
