using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IMedicalReportRepo
    { 
        Task<List<MedicalReport>> GetMedicalReportByCustomerIdAsync(int id);
        Task<MedicalReport> AddMedicalReportAsyn(MedicalReport medicalReport);
        Task<MedicalReport> GetMedicalReportByIdAsync(int id);
    }
}
