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
    public class BookingLogRepo : IBookingLogRepo
    {
        public BookingLogRepo(LumosDBContext context ) { }


        public Task<bool> UpdateBookingLogStatusForCustomerAsync(int bookingLogId, int newStatus) => BookingLogDAO.Instance.UpdateBookingLogStatusForCustomerAsync(bookingLogId, newStatus);

        public Task<bool> UpdateBookingLogStatusForPartnerAsync(int bookingLogId, int newStatus) => BookingLogDAO.Instance.UpdateBookingLogStatusForPartnerAsync(bookingLogId, newStatus);
    }
}
