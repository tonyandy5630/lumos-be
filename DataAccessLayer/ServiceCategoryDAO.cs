using BussinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ServiceCategoryDAO
    {
        private static ServiceCategoryDAO instance = null;
        private LumosDBContext _context = null;
        public static ServiceCategoryDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServiceCategoryDAO();
                    instance._context = new LumosDBContext();
                }
                return instance;
            }
        }

    }
}
