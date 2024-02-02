using BussinessObject;
using DataAccessLayer;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class MedicalReportRepo:IMedicalReportRepo
    {
        public MedicalReportRepo(LumosDBContext context) { }

        public Task<MedicalReport> AddMedicalReportAsyn(MedicalReport medicalReport) => MedicalReportDAO.Instance.AddMedicalReportAsyn(medicalReport);

        public Task<List<MedicalReport>> GetMedicalReportByCustomerIdAsync(int id) => MedicalReportDAO.Instance.GetMedicalReportByCustomerIdAsync(id);
    }
}
