using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.InterfaceService;
using BussinessObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Utils;

namespace LumosSolution.Controllers
{
    [Route("api/admin/")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("bookingdetail/{id}/booking")]
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

        [HttpGet("medical-report/{reportId}/booking")]
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
    }
}
