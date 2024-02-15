using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.InterfaceService;
using BussinessObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Utils;
using DataTransferObject.DTO;
using AutoMapper;

namespace LumosSolution.Controllers
{
    [Route("api/booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IBookingLogService _bookingLogService;
        private readonly IMapper _mapper;
        public BookingController(IBookingService bookingService, IMapper mapper, IBookingLogService bookingLogService)
        {
            _bookingService = bookingService;
            _mapper = mapper;
            _bookingLogService = bookingLogService;
        }

        [HttpGet("admin/bookingdetail/{id}/booking")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> GetBookingDetailById(int id)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                BookingDetail bookingDetail = await _bookingService.GetBookingDetailByBookingIdAsync(id);

                if (bookingDetail == null)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = ApiStatusCode.NotFound;
                    return NotFound(response);
                }

                response.data = bookingDetail;
                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }

        [HttpGet("admin/medical-report/{reportId}/booking")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> GetBookingsByMedicalReportId(int reportId)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                List<Booking> bookings = await _bookingService.GetBookingsByMedicalReportIdAsync(reportId);

                if (bookings == null || bookings.Count == 0)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = ApiStatusCode.NotFound;
                    return NotFound(response);
                }

                response.data = bookings;
                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }

        [HttpPost("customer/booking")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ApiResponse<object>>> CreateBookingAsync([FromBody] CreateBookingDTO createBookingDTO)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var booking = _mapper.Map<Booking>(createBookingDTO);
                bool result = await _bookingService.CreateBookingAsync(booking,createBookingDTO);

                if (result)
                {
                    response.message = MessagesResponse.Success.Created;
                    response.StatusCode = ApiStatusCode.OK;
                    return Ok(response);
                }
                else
                {
                    response.message = MessagesResponse.Error.OperationFailed;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }
        [HttpPut("partner/bookinglog/{bookinglogId}/status")]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateBookingLogStatusForPartner(int bookinglogId, [FromBody] int newStatus)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                bool result = await _bookingLogService.UpdateBookingLogStatusForPartnerAsync(bookinglogId, newStatus);

                if (result)
                {
                    response.message = MessagesResponse.Success.Updated;
                    response.StatusCode = ApiStatusCode.OK;
                    return Ok(response);
                }
                else
                {
                    response.message = MessagesResponse.Error.OperationFailed;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }

        [HttpPut("customer/bookinglog/{bookinglogId}/status")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateBookingLogStatusForCustomer(int bookinglogId, [FromBody] int newStatus)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                bool result = await _bookingLogService.UpdateBookingLogStatusForCustomerAsync(bookinglogId, newStatus);

                if (result)
                {
                    response.message = MessagesResponse.Success.Updated;
                    response.StatusCode = ApiStatusCode.OK;
                    return Ok(response);
                }
                else
                {
                    response.message = MessagesResponse.Error.OperationFailed;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }
    }
}
