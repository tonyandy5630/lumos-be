﻿using BussinessObject;
using DataTransferObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.InterfaceService
{
    public interface IBookingLogService
    {
        Task<bool> UpdateBookingLogStatusForPartnerAsync(int bookingLogId, int newStatus);
        Task<bool> UpdateBookingLogStatusForCustomerAsync(int bookingLogId, int newStatus);
        Task<BookingLog> GetLatestBookingLogAsync(int bookingId);
        Task<bool> CreateBookingLogAsync(BookingLog bookingLog);
        Task<List<PendingBookingDTO>> GetPendingBookingsByEmailAsync(string email);
        Task<List<PendingBookingDTO>> GetPendingBookingsByCustomerIdAsync(int customerId);
        Task<List<PendingBookingDTO>> GetBookingsByCustomerIdAsync(string email);
        Task<List<PendingBookingDTO>> GetBookingsHaveStatus1ByEmailAsync(string email);
    }
}
