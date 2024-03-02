using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class PaymentResponse
    {
        public string Bin { get; set; }
        public string AccountNumber { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public long OrderCode { get; set; }
        public string Currency { get; set; }
        public string PaymentLinkId { get; set; }
        public string Status { get; set; }
        public string CheckoutUrl { get; set; }
        public string QrCode { get; set; }
    }
}
