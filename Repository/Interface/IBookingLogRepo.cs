﻿using BussinessObject;
using DataTransferObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IBookingLogRepo
    {
        Task<List<BookingforAdminDTO>> GetAllBookingDetailsForAdminAsync();
        Task<List<BookingDTO>> GetAllBookingDetailsByCustomerIdForPartnertAsync(string email);
        Task<List<BookingDTO>> GetAllBookingDetailsByCustomerIdAsync(string email);
        Task<List<BillDTO>> GetBookingBillsByCustomerIdAsync(string email);
        Task<List<BookingDTO>> GetBookingDetailsByCustomerIdAsync(string email);
        Task<bool> UpdateBookingLogStatusForPartnerAsync(int bookingLogId, int newStatus);
        Task<bool> UpdateBookingLogStatusForCustomerAsync(int bookingLogId, int newStatus);
        Task<BookingLog> GetLatestBookingLogAsync(int bookingId);
        Task<bool> CreateBookingLogAsync(BookingLog bookingLog);

        List<IGrouping<int, BookingLog>> GroupBookings(List<BookingLog> allBookingLogs);
        Task<List<BookingLog>> GetALLBookingBillsAsync();
        Task<BillDetailDTO> FilterAndMapAllBillBookingsByBookingIdAsync(List<IGrouping<int, BookingLog>> BillDetails, int bookingId);
        Task<string> GetBookingStatusListAndCheckIsPayAsync(int bookingId);
        Task<BookingInfoDTO> GetBookingDetailsByIdAsync(int bookingId);
        Task<bool> CheckStatusForGetAllBooking(int bookingId, Customer customer);
        Task<List<BillMedicalDTO>> GetMedicalServiceBillDTOsAsync(int bookingId);
        Task<List<BookingDTO>> FilterAndMapIncomingBookingsAsync(List<IGrouping<int, BookingLog>> incoming, Customer customer);
        Task<List<MedicalServiceDTO>> GetMedicalServiceDTOsAsync(int bookingId);
        Task<bool> IsBookingStatusValidAsync(int bookingId, Customer customer);
        Task<List<BillDTO>> FilterAndMapAllBillBookingsAsync(List<IGrouping<int, BookingLog>> pendingBookings, Customer customer);
        Task<List<BookingLog>> GetAllPendingBookingLogsAsync();
        Task<List<BookingDTO>> FilterAndMapAllBookingsAsync(List<IGrouping<int, BookingLog>> pendingBookings, Customer customer);
        Task<List<BookingLog>> GetAllBookingLogsAsync();
        Task<BookingInfoDTO> GetBookingDetailsBillsByIdAsync(int bookingId);

        Task<List<BookingDTO>> FilterAndMapBookingsAsync(List<IGrouping<int, BookingLog>> bookings, Partner partner);
        Task<List<BookingLog>> GetAllLogAsync();
        Task<List<RefundListDTO>> GetRefundListAsync();
    }
}
