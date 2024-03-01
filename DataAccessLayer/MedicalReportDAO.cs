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
        private readonly LumosDBContext _context;

        public MedicalReportDAO(LumosDBContext _context)
        {
            this._context = _context;
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

        public async Task<IEnumerable<MedicalReport>> GetMedicalReportsByBookingIdAsync(int bookingId)
        {
            try
            {
                List<MedicalReport> reports = await (from b in _context.Bookings
                                 join bd in _context.BookingDetails on b.BookingId equals bd.BookingId
                                 join mr in _context.MedicalReports on bd.ReportId equals mr.ReportId
                                 join c in _context.Customers on mr.CustomerId equals c.CustomerId
                                 where b.BookingId == bookingId
                                 select mr).ToListAsync();
                return reports;
            }
            catch
            {
                throw new Exception();
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
                medicalReport.UpdatedBy = medicalReport.Fullname;
                _context.MedicalReports.Add(medicalReport);
                await _context.SaveChangesAsync();
                Console.WriteLine("Add medical report successfully!");
                return await _context.MedicalReports.FirstOrDefaultAsync(x => x.Code.Equals(medicalReport.Code));
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
                var customer = await _context.MedicalReports.Where(u => u.CustomerId == id).ToListAsync();
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
                return await _context.MedicalReports.FirstOrDefaultAsync(u => u.ReportId == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMedicalReportByIdAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
