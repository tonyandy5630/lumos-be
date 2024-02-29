using BussinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class BookingDetailsDAO
    {
        private static BookingDetailsDAO instance = null;
        private readonly LumosDBContext _context;

        public BookingDetailsDAO()
        {
            _context = new LumosDBContext();
        }

        public static BookingDetailsDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BookingDetailsDAO();
                }
                return instance;
            }
        }

        public async Task<List<int>> GetDistinctDetailByPartnerId(int partnerId)
        {
            try
            {

                 List<int> distinctDetailIds = await (from sb in _context.ServiceBookings
                                         join ps in _context.PartnerServices on sb.ServiceId equals ps.ServiceId
                                         join p in _context.Partners on ps.PartnerId equals p.PartnerId
                                         where p.PartnerId == partnerId
                                         select sb.DetailId).Distinct().ToListAsync();
                return distinctDetailIds;
            }
            catch(Exception ex)
            {
                throw new Exception();
            }
        }
    }
}
