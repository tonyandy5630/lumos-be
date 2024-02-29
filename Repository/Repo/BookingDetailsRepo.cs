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
    public class BookingDetailsRepo : IBookingDetailsRepo
    {
        public BookingDetailsRepo(LumosDBContext context) { }

        public Task<List<int>> GetDistinctBookingDetailsIdByPartnerId(int partnerId) => BookingDetailsDAO.Instance.GetDistinctDetailByPartnerId(partnerId);
    }
}
