using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RequestEntity
{
    public class PaymentRequest
    {
        public string BuyerName { get; set; }

        public string BuyerEmail { get; set; }

        public string BuyerPhone { get; set; }

        public string BuyerAddress { get; set; }
/*
        public string? Signature { get; set; }*/

        public int BookingId { get; set; }

/*        public List<ItemRequest> Items { get; set; }*/

/*        public int TotalAmount { get; set; }*/

        public string Description { get; set; }

/*        public string? CancelUrl { get; set; }*/

/*        public string? ReturnUrl { get; set; }*/
/*        public int ExpiredAt { get; set; }*/
    }
}
