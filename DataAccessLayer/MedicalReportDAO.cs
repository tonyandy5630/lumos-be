using BussinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

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

        public async Task<MedicalReport> AddMedicalReportAsyn(MedicalReport medicalReport)
        {
            try
            {

                medicalReport.Status = 1;
                medicalReport.Code = GenerateCode.GenerateTableCode("medicalreport");
                medicalReport.CreatedDate = DateTime.Now;
                medicalReport.LastUpdate = DateTime.Now;
                dbContext.MedicalReports.Add(medicalReport);
                await dbContext.SaveChangesAsync();
                Console.WriteLine("Add medical report successfully!");
                return await dbContext.MedicalReports.FirstOrDefaultAsync(x => x.Code.Equals(medicalReport.Code));
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

        public async Task<MedicalReport> GetMedicalReportByIdAsync(int id)
        {
            try
            {
                return await dbContext.MedicalReports.FirstOrDefaultAsync(u => u.ReportId == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMedicalReportByIdAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
