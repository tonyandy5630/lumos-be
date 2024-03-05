﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RequestEntity.Constraint.Constraint;

namespace RequestEntity
{
    public class AddPartnerServiceResquest
    {
        [Required]
        public string Name { get; set; }
        public int? Duration { get; set; }
        public string? Description { get; set; }
        [Required]
        [Range(PriceConstraint.FLOOR,PriceConstraint.CEIL, ErrorMessage = PriceConstraint.MESSAGE)]
        public int Price { get; set; }
        public IEnumerable<int> Categories { get; set; }
    }
}
