using BussinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class MedicalReportDAO
    {
        private static MedicalReportDAO instance = null;
        private readonly LumosDBContext dbContext;

        public MedicalReportDAO(LumosDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public static MedicalReportDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MedicalReportDAO(new LumosDBContext());
                }
                return instance;
            }
        }

        public async Task<bool> AddMedicalReportAsyn(MedicalReport medicalReport)
        {
            try
            {
                bool existMed = dbContext.MedicalReports
                    .Any(m => m.Phone.Equals(medicalReport.Phone));

                if (!existMed)
                {
                    medicalReport.Status = 1;
                    medicalReport.CreatedDate = DateTime.Now;
                    medicalReport.LastUpdate = DateTime.Now;
                    dbContext.MedicalReports.Add(medicalReport);
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            } catch (Exception ex)
            {
                Console.WriteLine($"Error in AddMedicalReportAsyn: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<MedicalReport>> GetMedicalReportByCustomerIdAsync(int id)
        {
            try
            {
                var customer = await dbContext.MedicalReports.Where(u => u.CustomerId == id).ToListAsync();
                return customer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMedicalReportByCustomerIdAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
