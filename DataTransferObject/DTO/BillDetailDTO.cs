﻿using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class BillDetailDTO
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; }
        public int Status { get; set; }
        public string? Partner { get; set; }
        public int? TotalPrice { get; set; }
        public int? AdditionalFee { get; set; } = 0;
        public DateTime BookingDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int bookingTime { get; set; } // workshift
        public string Address { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Note { get; set; }
        public decimal? Rating { get; set; }
        public bool? IsPay { get; set; }
        public bool? IsRefund { get; set; }
        public List<BillMedicalDTO?>? MedicalServices { get; set; }
    }
}
