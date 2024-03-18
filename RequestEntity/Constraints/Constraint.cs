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
            public const string clientId = "e2cd40e5-c2b0-45d2-b0ea-26a096707b48";
            public const string apiKey = "292a589f-ac7d-41e2-9049-3afb37f37841";
            public const string checksumKey = "6cc624c4a98dabb1e0fe83b24010e3ab4b0df6b2e9c60259c6377a7f7b8c669f";
        }
    }
}
