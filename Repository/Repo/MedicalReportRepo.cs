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

        public Task<bool> AddMedicalReportAsyn(MedicalReport medicalReport)
        {
            throw new NotImplementedException();
        }

        public Task<List<MedicalReport>> GetMedicalReportByCustomerIdAsync(int id) => MedicalReportDAO.Instance.GetMedicalReportByCustomerIdAsync(id);
    }
}
