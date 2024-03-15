using BussinessObject;
using DataTransferObject.DTO;
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

        public async Task<bool> AddPartnerScheduleAsync(List<Schedule> schedules)
        {
            try
            {
                await _context.Schedules.AddRangeAsync(schedules);
                int success = await _context.SaveChangesAsync();
                if (success != schedules.Count)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddPartnerScheduleAsync: {ex.Message}", ex);
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<ScheduleDTO>> GetSchedulesByPartnerIdAsync(int partnerId)
        {
            List<ScheduleDTO> schedules = new List<ScheduleDTO>();
            try
            {
                // Truy vấn cơ sở dữ liệu để lấy thông tin lịch làm việc của đối tác
                List<Schedule> schedulesFromDb = await _context.Schedules.Where(s => s.PartnerId == partnerId).ToListAsync();

                // Chuyển đổi danh sách lịch làm việc sang định dạng DTO
                schedules = schedulesFromDb.Select(s => new ScheduleDTO
                {
                    ScheduleId = s.ScheduleId,
                    WorkShift = s.WorkShift,
                    DayOfWeek = s.DayOfWeek,
                    From = s.From,
                    To = s.To,
                }).OrderBy(s => s.DayOfWeek)
                  .ThenBy(s => s.WorkShift)
                  .ToList();

                return schedules;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetSchedulesByPartnerIdAsync: {ex.Message}", ex);
                throw;
            }
        }
    }
}
