﻿using BussinessObject;
using Newtonsoft.Json;

namespace DataTransferObject.DTO
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public int Status { get; set; }
        public string? Partner { get; set; }
        public int? TotalPrice { get; set; }
        public int? AdditionalFee { get; set; } = 0;
        public DateTime BookingDate { get; set; }
        public int bookingTime { get; set; } // workshift
        public string Address { get; set; }
        public string? PaymentMethod { get; set; }
        public Customer? Customer { get; set; }
        public string? Note { get; set; }
        public decimal? Rating { get; set;}
        public string? PaymentLinkId { get; set; }
        public List<MedicalServiceDTO?>? MedicalServices { get; set; }
    }
}
