﻿using BussinessObject;
using DataTransferObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IBookingRepo
    {
        Task<BookingDetail> GetBookingDetailByBookingIdAsync(int id);
        Task<List<Booking>> GetBookingsByMedicalReportIdAsync(int medicalReportId);
        Task<bool> CreateBookingAsync(Booking booking, CreateBookingDTO createBookingDTO);
    }
}
