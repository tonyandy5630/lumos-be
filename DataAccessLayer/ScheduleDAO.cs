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
    public class ScheduleDAO
    {
        private static ScheduleDAO instance = null;
        private LumosDBContext _context = null;

        public ScheduleDAO()
        {
            if (_context == null)
                _context = new LumosDBContext();
        }

        public static ScheduleDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScheduleDAO();
                }
                return instance;
            }
        }

        public async Task<List<Schedule>> GetScheduleByPartnerIdAsyn(int id)
        {
            try
            {
                var schedule = await _context.Schedules.Where(u => u.PartnerId == id).ToListAsync();
                return schedule;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetScheduleByPartnerIdAsyn: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Schedule> AddPartnerScheduleAsync(Schedule schedule)
        {
            try
            {
                schedule.Code = GenerateCode.GenerateTableCode("Schedule");
                schedule.CreatedDate = DateTime.Now;
                schedule.CreatedDate = DateTime.Now;
                //schedule.CreatedBy = "admin";
                //schedule.UpdatedBy = "admin";

                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();
                Console.WriteLine("Schedule added successfully");
                return await _context.Schedules.SingleOrDefaultAsync(x => x.Code.Equals(schedule.Code));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddPartnerScheduleAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
