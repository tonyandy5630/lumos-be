using BussinessObject;
using DataAccessLayer;
using DataTransferObject.DTO;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo
{
    public class BookingRepo: IBookingRepo
    {
        public BookingRepo(LumosDBContext context) { }

        public Task<bool> CreateBookingAsync(Booking booking, CreateBookingDTO createBookingDTO) => BookingDAO.Instance.CreateBookingAsync(booking, createBookingDTO);  

        public Task<BookingDetail> GetBookingDetailByBookingIdAsync(int id) =>BookingDAO.Instance.GetBookingDetailByBookingIdAsync(id);

        public Task<List<Booking>> GetBookingsByMedicalReportIdAsync(int medicalReportId) => BookingDAO.Instance.GetBookingsByMedicalReportIdAsync(medicalReportId);
    }
}
