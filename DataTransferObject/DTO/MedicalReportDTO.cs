using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.DTO
{
    public class MedicalReportDTO
    {
        public int ReportId { get; set; }
        public int? CustomerId { get; set; }
        public string? Code { get; set; }
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public DateTime? Dob { get; set; }
        public bool? Gender { get; set; } //true = nam
        public int? Pronounce { get; set; }
        public int? BloodType { get; set; }
        public string? Note { get; set; }
        public int? Status { get; set; }
    }
}
