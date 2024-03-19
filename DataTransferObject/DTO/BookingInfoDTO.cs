using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class BookingInfoDTO
    {
        public string PaymentMethod { get; set; }
        public string Note { get; set; }
        public string PartnerName { get; set; }
        public Booking Booking { get; set; }
        public Customer Customer { get; set; }
        public bool isPaid { get; set; }
        public bool isRefund { get; set; }
    }
}
