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
        public static class PaymentsConstraint
        {
            public const string clientId = "2b13d4e0-5fac-42c7-b09b-5c3580d17a7f";
            public const string apiKey = "2c2991da-1a34-47d9-8902-dbe38b24146d";
            public const string checksumKey = "14fdde4c9fac6ad7f75e25137b3593bec0120691fc2470704b3445a11a75d4bb";
        }
    }
}
