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
        [HttpGet("/api/stat/sub")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> GetTopBookedServicesInAllTime([FromQuery] int top =5)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                if(top <= 0)
                {
                    response.message = "Invalid top parameter.";
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }

                // Get top booked services
                var topServices = await _bookingService.GetTopBookedServicesAsync(top);

                if (topServices == null || !topServices.Any())
                {
                    response.message = "No data found.";
                    response.StatusCode = ApiStatusCode.NotFound;
                    return NotFound(response);
                }

                // Calculate total bookings, return patients, operations, and earnings
                int totalBookings = topServices.Sum(s => s.NumberOfBooking);
                int returnPatients = topServices.Count(s => s.NumberOfBooking > 2);
                int operations = topServices.Sum(s => s.NumberOfBooking); // Assuming operation count is the same as total bookings
                int earning = totalBookings * 1000; // Placeholder earning calculation

                // Prepare output data
                var responseData = new
                {
                    TotalBookings = totalBookings,
                    ReturnPatients = returnPatients,
                    Operations = operations,
                    Earning = earning
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

                // Retrieve booking data for the specified year
                List<Booking> bookings = await _bookingService.GetAllBookingsForYearAsync(year);

                // Group bookings by month and count the number of bookings in each month
                Dictionary<string, int> monthlyStats = new Dictionary<string, int>();
                for (int month = 1; month <= 12; month++)
                {
                    string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                    int bookingsCount = bookings.Count(b => b.BookingDate.Value.Year == year && b.BookingDate.Value.Month == month);
                    monthlyStats.Add(monthName, bookingsCount);
                }

                // Prepare output data
                response.data = monthlyStats;
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
