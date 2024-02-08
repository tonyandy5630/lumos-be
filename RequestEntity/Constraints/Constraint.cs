using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestEntity.Constraint
{
    public class Constraint
    {
        public static class PriceConstraint
        {
            public const  int FLOOR = 0;
            public const int CEIL = 3000000;
            public const string MESSAGE = "Price is too large";
        }
    }
}
