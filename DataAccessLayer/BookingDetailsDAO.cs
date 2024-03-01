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
    }
}
