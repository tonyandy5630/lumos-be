using BussinessObject;
using DataAccessLayer;
using DataTransferObject.DTO;
using Enum;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Repository.Repo
{
    public class BookingLogRepo : IBookingLogRepo
    {
        public BookingLogRepo(LumosDBContext context ) { }

        public Task<bool> CreateBookingLogAsync(BookingLog bookingLog) => BookingLogDAO.Instance.CreateBookingLogAsync(bookingLog);

        public Task<BookingLog> GetLatestBookingLogAsync(int bookingId) => BookingLogDAO.Instance.GetLatestBookingLogAsync(bookingId);

        public Task<bool> UpdateBookingLogStatusForCustomerAsync(int bookingLogId, int newStatus) => BookingLogDAO.Instance.UpdateBookingLogStatusForCustomerAsync(bookingLogId, newStatus);

        public Task<bool> UpdateBookingLogStatusForPartnerAsync(int bookingLogId, int newStatus) => BookingLogDAO.Instance.UpdateBookingLogStatusForPartnerAsync(bookingLogId, newStatus);

        public List<IGrouping<int, BookingLog>> GroupBookings(List<BookingLog> allBookingLogs) => BookingLogDAO.Instance.GroupBookings(allBookingLogs);

        public Task<List<BookingLog>> GetALLBookingBillsAsync() => BookingLogDAO.Instance.GetALLBookingBillsAsync();


        public async Task<BillDetailDTO> FilterAndMapAllBillBookingsByBookingIdAsync(List<IGrouping<int, BookingLog>> BillDetails, int bookingId)
        {
            var result = new BillDetailDTO();

            foreach (var group in BillDetails)
            {
                if (group.Key == bookingId)
                {
                    var statuses = group.Select(bl => bl.Status).Distinct().ToList();
                    var bookingInfo = await GetBookingDetailsBillsByIdAsync(bookingId);
                    if (bookingInfo == null)
                    {
                        return new BillDetailDTO();
                    }
                    foreach (var status in statuses)
                    {

                        var isPay = await GetBookingStatusListAndCheckIsPayAsync(bookingId);
                        if (await CheckStatusForGetAllBooking(bookingId, bookingInfo.Customer))
                        {
                            var medicalServiceDTOs = await GetMedicalServiceBillDTOsAsync(bookingId);
                            result = new BillDetailDTO
                            {
                                BookingId = bookingId,
                                BookingCode = bookingInfo.Booking.Code,
                                Status = status,
                                Address = bookingInfo.Booking.Address,
                                Partner = bookingInfo.PartnerName,
                                PaymentMethod = bookingInfo.PaymentMethod,
                                TotalPrice = bookingInfo.Booking.TotalPrice,
                                BookingDate = bookingInfo.Booking.BookingDate,
                                CreateDate = (DateTime)bookingInfo.Booking.CreatedDate,
                                bookingTime = bookingInfo.Booking.bookingTime,
                                Note = bookingInfo.Note,
                                IsPay = isPay,
                                MedicalServices = medicalServiceDTOs,
                            };
                        }
                    }
                }
            }

            return result;
        }

        public Task<string> GetBookingStatusListAndCheckIsPayAsync(int bookingId) => BookingLogDAO.Instance.GetBookingStatusListAndCheckIsPayAsync(bookingId);


        public Task<BookingInfoDTO> GetBookingDetailsByIdAsync(int bookingId) => BookingLogDAO.Instance.GetBookingDetailsByIdAsync(bookingId);

        public Task<bool> CheckStatusForGetAllBooking(int bookingId, Customer customer) => BookingLogDAO.Instance.CheckStatusForGetAllBooking(bookingId, customer);

        public Task<List<BillMedicalDTO>> GetMedicalServiceBillDTOsAsync(int bookingId) => BookingLogDAO.Instance.GetMedicalServiceBillDTOsAsync(bookingId);

        public async Task<List<BookingDTO>> FilterAndMapIncomingBookingsAsync(List<IGrouping<int, BookingLog>> incoming, Customer customer)
        {
            var result = new List<BookingDTO>();

            foreach (var group in incoming)
            {
                var bookingId = group.Key;
                var statuses = group.Select(bl => bl.Status).Distinct().ToList();
                var bookingInfo = await GetBookingDetailsByIdAsync(bookingId);
                if (bookingInfo == null)
                {
                    return new List<BookingDTO>();
                }
                foreach (var status in statuses)
                {
                    if (await IsBookingStatusValidAsync(bookingId, customer))
                    {
                        var medicalServiceDTOs = await GetMedicalServiceDTOsAsync(bookingId);
                        if (bookingInfo.Booking.BookingDate.Date >= DateConverter.GetUTCTime().Date)
                        {
                            result.Add(new BookingDTO
                            {
                                BookingId = bookingId,
                                BookingCode = bookingInfo.Booking.Code,
                                Status = status,
                                PaymentLinkId = bookingInfo.Booking.PaymentLinkId,
                                Partner = bookingInfo.PartnerName,
                                TotalPrice = bookingInfo.Booking.TotalPrice,
                                BookingDate = bookingInfo.Booking.BookingDate,
                                bookingTime = bookingInfo.Booking.bookingTime,
                                Address = bookingInfo.Booking.Address,
                                PaymentMethod = bookingInfo.PaymentMethod,
                                Note = bookingInfo.Note,
                                Customer = customer,
                                MedicalServices = medicalServiceDTOs
                            });
                        }
                    }
                }
            }

            return result;
        }

        public Task<List<MedicalServiceDTO>> GetMedicalServiceDTOsAsync(int bookingId) => BookingLogDAO.Instance.GetMedicalServiceDTOsAsync(bookingId);

        public Task<bool> IsBookingStatusValidAsync(int bookingId, Customer customer) => BookingLogDAO.Instance.IsBookingStatusValidAsync(bookingId,customer);

        public async Task<List<BillDTO>> FilterAndMapAllBillBookingsAsync(List<IGrouping<int, BookingLog>> pendingBookings, Customer customer)
        {
            var result = new List<BillDTO>();

            foreach (var group in pendingBookings)
            {
                var bookingId = group.Key;
                var statuses = group.Select(bl => bl.Status).Distinct().ToList();
                var bookingInfo = await GetBookingDetailsBillsByIdAsync(bookingId);
                if (bookingInfo == null)
                {
                    return new List<BillDTO>();
                }
                foreach (var status in statuses)
                {
                    if (await CheckStatusForGetAllBooking(bookingId, customer))
                    {
                        result.Add(new BillDTO
                        {
                            BookingId = bookingId,
                            BookingCode = bookingInfo.Booking.Code,
                            Status = status,
                            PartnerName = bookingInfo.PartnerName,
                            TotalPrice = bookingInfo.Booking.TotalPrice,
                            BookingDate = bookingInfo.Booking.BookingDate,
                            CreateDate = (DateTime)bookingInfo.Booking.CreatedDate,
                            bookingTime = bookingInfo.Booking.bookingTime,
                            Note = bookingInfo.Note,
                        });
                    }
                }
            }
            var waitingForPaymentBookings = result.Where(b => b.Status == (int)BookingStatusEnum.WaitingForPayment).ToList();
            if (waitingForPaymentBookings.Any())
            {
                result = waitingForPaymentBookings.Concat(result.Except(waitingForPaymentBookings)).ToList();
            }

            return result;
        }

        public Task<List<BookingLog>> GetAllPendingBookingLogsAsync()=>BookingLogDAO.Instance.GetAllPendingBookingLogsAsync();

        public async Task<List<BookingDTO>> FilterAndMapAllBookingsAsync(List<IGrouping<int, BookingLog>> bookings, Customer customer)
        {
            var result = new List<BookingDTO>();
            foreach (var group in bookings)
            {
                var bookingId = group.Key;
                var statuses = group.Select(bl => bl.Status).Distinct().ToList();
                var bookingInfo = await GetBookingDetailsByIdAsync(bookingId);
                if (bookingInfo == null)
                {
                    return new List<BookingDTO>();
                }
                foreach (var status in statuses)
                {
                    if (await CheckStatusForGetAllBooking(bookingId, customer))
                    {
                        var medicalServiceDTOs = await GetMedicalServiceDTOsAsync(bookingId);
                        result.Add(new BookingDTO
                        {
                            BookingId = bookingId,
                            BookingCode = bookingInfo.Booking.Code,
                            Status = status,
                            Partner = bookingInfo.PartnerName,
                            PaymentLinkId = bookingInfo.Booking.PaymentLinkId,
                            TotalPrice = bookingInfo.Booking.TotalPrice,
                            BookingDate = bookingInfo.Booking.BookingDate,
                            bookingTime = bookingInfo.Booking.bookingTime,
                            Address = bookingInfo.Booking.Address,
                            PaymentMethod = bookingInfo.PaymentMethod,
                            Note = bookingInfo.Note,
                            Customer = customer,
                            MedicalServices = medicalServiceDTOs
                        });
                    }
                }
            }
            var waitingForPaymentBookings = result.Where(b => b.Status == (int)BookingStatusEnum.WaitingForPayment).ToList();
            if (waitingForPaymentBookings.Any())
            {
                result = waitingForPaymentBookings.Concat(result.Except(waitingForPaymentBookings)).ToList();
            }

            return result;
        }

        public Task<List<BookingLog>> GetAllBookingLogsAsync() =>BookingLogDAO.Instance.GetAllBookingLogsAsync();

        public Task<BookingInfoDTO> GetBookingDetailsBillsByIdAsync(int bookingId) => BookingLogDAO.Instance.GetBookingDetailsBillsByIdAsync(bookingId);
        public async Task<List<BookingDTO>> FilterAndMapBookingsAsync(List<IGrouping<int, BookingLog>> bookings, Partner partner)
        {
            var result = new List<BookingDTO>();

            foreach (var group in bookings)
            {
                var bookingId = group.Key;
                var statuses = group.Select(bl => bl.Status).Distinct().ToList();
                var bookingInfo = await GetBookingDetailsBillsByIdAsync(bookingId);
                if (bookingInfo == null)
                {
                    return new List<BookingDTO>();
                }
                foreach (var status in statuses)
                {
                    if (await BookingLogDAO.Instance.CheckStatusForGetAllBookingWithPartner(bookingId, partner))
                    {
                        var medicalServiceDTOs = await GetMedicalServiceDTOsAsync(bookingId);

                        result.Add(new BookingDTO
                        {
                            BookingId = bookingId,
                            BookingCode = bookingInfo.Booking.Code,
                            TotalPrice = bookingInfo.Booking.TotalPrice,
                            Status = status,
                            Partner = partner.DisplayName,
                            Note = bookingInfo.Note,
                            Rating = bookingInfo.Booking.Rating,
                            PaymentLinkId = bookingInfo.Booking.PaymentLinkId,
                            BookingDate = bookingInfo.Booking.BookingDate,
                            bookingTime = bookingInfo.Booking.bookingTime,
                            Address = bookingInfo.Booking.Address,
                            PaymentMethod = bookingInfo.PaymentMethod,
                            MedicalServices = medicalServiceDTOs
                        });
                    }
                }
            }

            return result;
        }

        public Task<List<BookingLog>> GetAllLogAsync() => BookingLogDAO.Instance.GetAllLogAsync();

        public Task<List<BookingDTO>> GetBookingDetailsByCustomerIdAsync(string email) => BookingLogDAO.Instance.GetBookingDetailsByCustomerIdAsync(email);

        public Task<List<BillDTO>> GetBookingBillsByCustomerIdAsync(string email) => BookingLogDAO.Instance.GetBookingBillsByCustomerIdAsync(email);

        public Task<List<BookingDTO>> GetAllBookingDetailsByCustomerIdAsync(string email) => BookingLogDAO.Instance.GetAllBookingDetailsByCustomerIdAsync(email);

        public Task<List<BookingDTO>> GetAllBookingDetailsByCustomerIdForPartnertAsync(string email) => BookingLogDAO.Instance.GetAllBookingDetailsByCustomerIdForPartnertAsync(email);
    }
}
