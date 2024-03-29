﻿using BussinessObject;
using DataTransferObject.DTO;
using RequestEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.InterfaceService
{
    public interface IBookingLogService
    {
        Task<List<RefundListDTO>> GetRefundListAsync();
        Task<bool> UpdateBookingLogStatusForPartnerAsync(int bookingLogId, int newStatus);
        Task<bool> UpdateBookingLogStatusForCustomerAsync(int bookingLogId, int newStatus);
        Task<BookingLog> GetLatestBookingLogAsync(int bookingId);
        Task<bool> AcceptBooking(int bookingid, string email);
        Task<bool> DeclineBooking(BookingLogDeclineRequest updateBookingStatus, string email);
        Task<List<BookingDTO>> GetIncomingBookingsByEmailAsync(string email);
        Task<List<BookingDTO>> GetIncomingBookingsByCustomerIdAsync(int customerId);
        Task<List<BookingDTO>> GetBookingsByCustomerIdAsync(string email);
        Task<bool> ChangeStatusToPending(BookingLogAcceptRequest updateBookingStatus, string email);
        Task<List<BillDTO>> GetBookingsBillsByCustomerIdAsync(string email);
        Task<BillDetailDTO> GetBookingsBillsByBookingidAsync(int bookingId);
        Task<bool> ChangeStatusToComplete(ChangToCompleteRequest complete, string email);
    }
}
