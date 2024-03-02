﻿using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class PartnerServiceDAO
    {
        private static PartnerServiceDAO instance = null;
        private readonly LumosDBContext _context = null;

        public PartnerServiceDAO()
        {
            if (_context == null)
                _context = new LumosDBContext();
        }

        public static PartnerServiceDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PartnerServiceDAO();
                }
                return instance;
            }
        }

        public async Task<List<PartnerServiceDTO>> GetServiceBookedByMedicalReportIdAndBookingId(int reportId, int bookingId)
        {
            try
            {
                List<PartnerServiceDTO> bookedServices = await (from ps in _context.PartnerServices
                                                                  join sb in _context.ServiceBookings on ps.ServiceId equals sb.ServiceId
                                                                  join bd in _context.BookingDetails on sb.DetailId equals bd.DetailId
                                                                  join mr in _context.MedicalReports on bd.ReportId equals mr.ReportId
                                                                  join b in _context.Bookings on bd.BookingId equals b.BookingId
                                                                  where mr.ReportId == reportId && b.BookingId == bookingId
                                                                  select new PartnerServiceDTO
                                                                  {
                                                                      Code = ps.Code,
                                                                      Name = ps.Name,
                                                                      Price = sb.Price,
                                                                      Duration = ps.Duration,
                                                                      ServiceId = ps.ServiceId,
                                                                  }).ToListAsync();
                return bookedServices;
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<PartnerService?> GetServiceOfPartnerByServiceNameAsync(string serviceName, int partnerId)
        {
            return await _context.PartnerServices.FirstOrDefaultAsync(s => s.PartnerId == partnerId && s.Name.ToLower().Contains(serviceName.ToLower()));
        }
        public async Task<IEnumerable<PartnerServiceDTO>> GetTopFiveBookedServicesAsync()
        {
            var partnerDAO = PartnerDAO.Instance;
            var result = from sb in _context.ServiceBookings
                         group sb by sb.ServiceId into grouped
                         orderby grouped.Count() descending
                         select new PartnerServiceDTO
                         {
                             ServiceId = (int)grouped.Key,
                             BookedQuantity = grouped.Count(),
                         };

            var topFiveServices = await result.Take(5).ToListAsync();

            var serviceDetails = new List<PartnerServiceDTO>();
            foreach (var service in topFiveServices)
            {
                var serviceDetail = await partnerDAO.GetPartnerServiceByIdAsync(service.ServiceId);
                if (serviceDetail != null)
                {
                    serviceDetail.BookedQuantity = service.BookedQuantity;
                    serviceDetails.Add(serviceDetail);
                }
            }

            return serviceDetails;
        }

    }
}
