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
using System.Globalization;
using System.Security.Claims;
using RequestEntity;
using DataAccessLayer;
using System.Net;

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
        [HttpGet("pending")]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<ApiResponse<object>>> GetPendingBookings()
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var pendingBookingDTOs = await _bookingLogService.GetBookingsHaveStatus1ByEmailAsync(userEmail);

                if (pendingBookingDTOs == null || !pendingBookingDTOs.Any())
                {
                    response.message = "No pending bookings found.";
                    response.StatusCode = ApiStatusCode.NotFound;
                    return NotFound(response);
                }

                response.data = pendingBookingDTOs;
                response.message = "Success";
                response.StatusCode = ApiStatusCode.OK;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.message = "Internal Server Error";
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ApiResponse<object>>> GetAllBookingsByCustomerID()
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var pendingBookingDTOs = await _bookingLogService.GetBookingsByCustomerIdAsync(userEmail);

                if (pendingBookingDTOs == null || !pendingBookingDTOs.Any())
                {
                    response.message = "No bookings found.";
                    response.StatusCode = ApiStatusCode.NotFound;
                    return NotFound(response);
                }

                response.data = pendingBookingDTOs;
                response.message = "Success";
                response.StatusCode = ApiStatusCode.OK;

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                response.message = "Unauthorized";
                response.StatusCode = ApiStatusCode.Unauthorized;
                return Unauthorized(response);
            }
            catch (Exception ex)
            {
                response.message = "Internal Server Error";
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }
        [HttpGet("customer/{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ApiResponse<object>>> GetIcomeBookingsByCustomerID(int id)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var pendingBookingDTOs = await _bookingLogService.GetPendingBookingsByCustomerIdAsync(id);

                if (pendingBookingDTOs == null || !pendingBookingDTOs.Any())
                {
                    response.message = "No bookings found.";
                    response.StatusCode = ApiStatusCode.NotFound;
                    return NotFound(response);
                }

                response.data = pendingBookingDTOs;
                response.message = "Success";
                response.StatusCode = ApiStatusCode.OK;

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                response.message = "Unauthorized";
                response.StatusCode = ApiStatusCode.Unauthorized;
                return Unauthorized(response);
            }
            catch (Exception ex)
            {
                response.message = "Internal Server Error";
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }

        [HttpGet("comming")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ApiResponse<object>>> GetIncomingBookings()
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                var pendingBookingDTOs = await _bookingLogService.GetPendingBookingsByEmailAsync(userEmail);

                if (pendingBookingDTOs == null || !pendingBookingDTOs.Any())
                {
                    response.message = "No pending bookings found.";
                    response.StatusCode = ApiStatusCode.NotFound;
                    return NotFound(response);
                }

                response.data = pendingBookingDTOs;
                response.message = "Success";
                response.StatusCode = ApiStatusCode.OK;

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                response.message = "Unauthorized";
                response.StatusCode = ApiStatusCode.Unauthorized;
                return Unauthorized(response);
            }
            catch (Exception ex)
            {
                response.message = "Internal Server Error";
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }
        [HttpPost("/api/booking-logs")]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateBookingStatusAndCreateLog([FromBody] BookingLogRequest updateBookingStatusDTO)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var latestBookingLog = await _bookingLogService.GetLatestBookingLogAsync(updateBookingStatusDTO.BookingId);

                if (latestBookingLog.Status < 0 || latestBookingLog.Status > 4)
                {
                    response.message = "The status of the latest booking log is invalid or not allowed.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }

                if (latestBookingLog.Status == 4)
                {
                    response.message = "The status of the latest booking log is already Completed. Cannot create a new one.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }

                BookingLog newBookingLog = new BookingLog
                {
                    BookingId = updateBookingStatusDTO.BookingId,
                    Note = latestBookingLog.Note,
                    Status = latestBookingLog.Status + 1,
                    CreatedDate = DateTime.Now,
                    CreatedBy = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
            };

                bool result = await _bookingLogService.CreateBookingLogAsync(newBookingLog);
                if (!result)
                {
                    response.message = "Failed to create booking log with new status.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }

                response.message = "Booking log with new status created successfully.";
                response.StatusCode = ApiStatusCode.OK;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.message = "Internal Server Error";
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }

        [HttpGet("/api/stat/sub/partner")]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<ApiResponse<object>>> GetTopBookedServicesInAllTime()
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (userEmail == null)
                {
                    response.message = "không tìm thấy email";
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }

                // Get top booked services
                var topServices = await _bookingService.GetAllBookedServicesByPartnerEmailAsync(userEmail);

                if (topServices == null)
                {
                    response.message = "No data found.";
                    response.StatusCode = ApiStatusCode.NotFound;
                    return NotFound(response);
                }

                // Prepare output data
                var responseData = new
                {
                    TotalBookings = topServices.TotalBookings,
                    ReturnPatients = topServices.ReturnPatients,
                    Operations = topServices.Operations,
                    Earning = topServices.Earning
                };

                response.data = responseData;
                response.message = "Success";
                response.StatusCode = ApiStatusCode.OK;

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                response.message = "Unauthorized";
                response.StatusCode = ApiStatusCode.Unauthorized;
                return Unauthorized(response);
            }
            catch (Exception ex)
            {
                response.message = "Internal Server Error";
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }
        [HttpGet("stats/bookings/monthly/{year}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> GetMonthlyBookingStats(int year)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                if (year < 0)
                {
                    response.message = "Invalid year parameter.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }

                List<int?> stats = await _bookingService.GetAllBookingsForYearAsync(year);


                // Prepare output data
                response.data = stats;
                response.message = "Success";
                response.StatusCode = ApiStatusCode.OK;

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                response.message = "Unauthorized";
                response.StatusCode = ApiStatusCode.Unauthorized;
                return Unauthorized(response);
            }
            catch (Exception ex)
            {
                response.message = "Internal Server Error";
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }

        [HttpGet("{top}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> GetTopBookedServices(int top)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                if (top <= 0)
                {
                    response.message = "Invalid top parameter.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }

                var topServices = await _bookingService.GetTopBookedServicesAsync(top);

                if (topServices == null || !topServices.Any())
                {
                    response.message = "No data found.";
                    response.StatusCode = ApiStatusCode.NotFound;
                    return NotFound(response);
                }

                response.data = topServices;
                response.message = "Success";
                response.StatusCode = ApiStatusCode.OK;

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                response.message = "Unauthorized";
                response.StatusCode = ApiStatusCode.Unauthorized;
                return Unauthorized(response);
            }
            catch (Exception ex)
            {
                response.message = "Internal Server Error";
                response.StatusCode = 500;
                return BadRequest(response);
            }
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

        [HttpPost("customer")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ApiResponse<object>>> CreateBookingAsync([FromBody] CreateBookingDTO createBookingDTO)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                var booking = _mapper.Map<Booking>(createBookingDTO);

                bool result = await _bookingService.CreateBookingAsync(booking, createBookingDTO, userEmail);

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
        [HttpGet("incomplete")]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<ActionResult<ApiResponse<object>>> GetIncompleteBookings(int? customerId = null, int? reportId = null)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                List<Booking> incompleteBookings;

                if (customerId.HasValue)
                {
                    incompleteBookings = await _bookingService.GetIncompleteBookingsByCustomerIdAsync(customerId.Value);
                }
                else if (reportId.HasValue)
                {
                    incompleteBookings = await _bookingService.GetIncompleteBookingsByReportIdAsync(reportId.Value);
                }
                else
                {
                    incompleteBookings = await _bookingService.GetAllIncompleteBookingsAsync();
                }

                response.data = incompleteBookings;
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
    }
}
