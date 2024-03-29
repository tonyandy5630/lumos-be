﻿using BussinessObject;
using DataTransferObject.DTO;
using Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RequestEntity;
using Service.ErrorObject;
using Service.InterfaceService;
using Service.Service;
using System.Collections.Generic;
using System.Security.Claims;
using Utils;
using static Google.Apis.Requests.BatchRequest;
using static Utils.MessagesResponse;

namespace LumosSolution.Controllers
{
    [Route("api/partner")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService _partnerService;
        private readonly IBookingService _bookingService;
        public PartnerController(IPartnerService partnerService, IBookingService bookingService)
        {
            _partnerService = partnerService;
            _bookingService = bookingService;
        }

        [HttpGet("bookings/status/{stat}")]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<ApiResponse<List<BookingDTO>>>> GetPendingBookings(int stat)
        {
            ApiResponse<object> response = new ApiResponse<object>();
            try
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                List<BookingDTO> pendingBookingDTOs = await _bookingService.GetPartnerBookingsByStatusAsync(userEmail, stat);

                if (pendingBookingDTOs == null || !pendingBookingDTOs.Any())
                {
                    response.message = "No bookings found.";
                    response.StatusCode = ApiStatusCode.OK;
                    return Ok(response);
                }

                response.data = pendingBookingDTOs;
                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;

                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    response.message = ex.Message;
                    response.StatusCode = ApiStatusCode.NotFound;
                    return NotFound(response);
                }

                if (ex is NotSupportedException)
                {
                    response.message = ex.Message;
                    response.StatusCode = 422;
                    return UnprocessableEntity(response);
                }
                Console.WriteLine(ex.ToString());
                response.message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpGet("bookings/{page}")]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<ApiResponse<List<BookingDTO>>>> GetPartnerBookings(int page)
        {
            ApiResponse<List<BookingDTO>> response = new ApiResponse<List<BookingDTO>>();
            try
            {
                const int PageSize = 7;
                string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                if (email == null)
                {
                    response.message = MessagesResponse.Error.Unauthorized;
                    response.StatusCode = ApiStatusCode.Unauthorized;
                    return Unauthorized(response);
                }

                var bookings = await _partnerService.GetPartnerBookingsAsync(email, page, PageSize);


                response.data = bookings;
                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.message = ex.Message;
                response.StatusCode = ApiStatusCode.InternalServerError;
                return StatusCode(500, response);
            }
        }
        [HttpGet("services")]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<ApiResponse<List<PartnerServiceDTO>>>> GetPartnerServices()
        {
            ApiResponse<List<PartnerServiceDTO>> response = new ApiResponse<List<PartnerServiceDTO>>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = 404
            };

            try
            {
                string? email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                List<PartnerServiceDTO> partnerServices = await _partnerService.GetPartnerServicesWithBookingCountAsync(email);


                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;
                response.data = partnerServices;

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                response.message = MessagesResponse.Error.Unauthorized;
                response.StatusCode = ApiStatusCode.Unauthorized;
                return StatusCode(401, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.message = ex.Message;
                response.StatusCode = ApiStatusCode.InternalServerError;
                return StatusCode(500, response);
            }
        }

        [HttpGet("revenue/{month}")]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<ListDataDTO>> GetPartnerRevenueInMonth(int month, int? year = null)
        {
            ApiResponse<ListRevenueDTO> response = new ApiResponse<ListRevenueDTO>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = ApiStatusCode.NotFound
            };

            try
            {
                string? userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (year == null)
                {
                    year = DateTime.Now.Year;
                }

                var revenuePerWeek = await _partnerService.CalculatePartnerRevenueInMonthAsync(userEmail, month, (int)year);

                if (revenuePerWeek == null)
                    return NotFound(response);

                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;
                response.data = revenuePerWeek;

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.message = ex.Message;
                response.StatusCode = ApiStatusCode.InternalServerError;
                return StatusCode(500, response);
            }
        }

        [HttpGet("/api/stat/partner/services")]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<ApiResponse<object>>> GetPartnerServiceStatistics()
        {
            ApiResponse<object> response = new ApiResponse<object>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = ApiStatusCode.NotFound,
                data = "Không tìm thấy kết quả"
            };

            try
            {

                string? email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                StatPartnerServiceDTO res = await _partnerService.GetStatPartnerServiceAsync(email);


                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;
                response.data = res;

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                response.message = MessagesResponse.Error.Unauthorized;
                response.StatusCode = ApiStatusCode.Unauthorized;
                return StatusCode(401, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.message = ex.Message;
                response.StatusCode = ApiStatusCode.InternalServerError;
                return StatusCode(500, response);
            }
        }

        [HttpGet("service/detail/{id:int}")]
        [Authorize(Roles = "Admin,Customer,Partner")]
        public async Task<ActionResult<ApiResponse<PartnerServiceDTO?>>> GetaPartnerServiceDetailById(int id)
        {
            ApiResponse<PartnerServiceDTO?> response = new ApiResponse<PartnerServiceDTO?>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = 404
            };
            try
            {
                PartnerServiceDTO? partnerService = await _partnerService.GetPartnerServiceDetailAsync(id);

                if (partnerService == null)
                    return NotFound(response);

                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;
                response.data = partnerService;

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.message = ex.Message;
                response.StatusCode = ApiStatusCode.InternalServerError;
                return StatusCode(500, response);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<SearchPartnerDTO>>> GetPartnerByPartnerOrServiceName([FromQuery] string? keyword = "")
        {
            ApiResponse<IEnumerable<SearchPartnerDTO>> res = new ApiResponse<IEnumerable<SearchPartnerDTO>>
            {
                message = MessagesResponse.Error.OperationFailed,
                StatusCode = ApiStatusCode.InternalServerError
            };
            try
            {
                IEnumerable<SearchPartnerDTO> searchPartnerDTOs = await _partnerService.SearchPartnerByPartnerOrServiceNameAsync(keyword);
                res.message = MessagesResponse.Success.Completed;
                res.StatusCode = ApiStatusCode.OK;
                res.data = searchPartnerDTOs;
                return Ok(res);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.message = ex.Message;
                res.StatusCode = ApiStatusCode.InternalServerError;
                return StatusCode(500, res);
            }
        }
        [HttpGet("/api/partner/category/{categoryId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<SearchPartnerDTO>>> GetPartnerByCategory(int categoryId)
        {
            ApiResponse<IEnumerable<SearchPartnerDTO>> res = new ApiResponse<IEnumerable<SearchPartnerDTO>>
            {
                message = MessagesResponse.Error.OperationFailed,
                StatusCode = ApiStatusCode.InternalServerError
            };
            try
            {
                IEnumerable<SearchPartnerDTO> searchPartnerDTOs = await _partnerService.GetPartnerByCategoryAsync(categoryId);
                res.message = MessagesResponse.Success.Completed;
                res.StatusCode = ApiStatusCode.OK;
                res.data = searchPartnerDTOs;
                return Ok(res);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.message = ex.Message;
                res.StatusCode = ApiStatusCode.InternalServerError;
                return StatusCode(500, res);
            }
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer,Partner")]
        public async Task<ActionResult<ApiResponse<SearchPartnerDTO?>>> GetPartnerById(int id)
        {
            ApiResponse<SearchPartnerDTO?> res = new ApiResponse<SearchPartnerDTO?>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = ApiStatusCode.NotFound
            };
            try
            {
                SearchPartnerDTO? partner = await _partnerService.GetPartnerByIDAsync(id);

                if (partner == null)
                    return res;

                res.message = MessagesResponse.Success.Completed;
                res.StatusCode = ApiStatusCode.OK;
                res.data = partner;

                return Ok(res);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                res.message = ex.Message;
                res.StatusCode = ApiStatusCode.InternalServerError;
                return StatusCode(500, res);
            }
        }


        [HttpPost("service")]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<object>> AddPartnerService([FromBody] AddPartnerServiceResquest service)
        {
            ApiResponse<object> response = new ApiResponse<object>
            {
                StatusCode = 500
            };
            try
            {
                if (!ModelState.IsValid)
                {
                    response.message = MessagesResponse.Error.InvalidInput;
                    response.StatusCode = ApiStatusCode.UnprocessableEntity;
                    return UnprocessableEntity(response);
                }

                string? email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                (PartnerService? newService, PartnerServiceError? error) = await _partnerService.AddPartnerServiceAsync(service, email);
                if (error != null)
                {
                    response.message = MessagesResponse.Error.DuplicateResource;
                    response.StatusCode = 422;
                    response.data = error;
                    return UnprocessableEntity(response);
                }

                if (newService == null)
                    throw new Exception("Added sevice failed");

                response.message = MessagesResponse.Success.Created;
                response.StatusCode = ApiStatusCode.OK;
                response.data = newService;
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.message = ex.Message;
                response.StatusCode = ApiStatusCode.InternalServerError;
                return BadRequest(response);
            }
        }
        [HttpPut("service")]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> UpdatePartnerService([FromBody] UpdatePartnerServiceRequest request)
        {
            try
            {
                var (isSuccess, message) = await _partnerService.UpdatePartnerServiceAsync(request, request.ServiceId);

                if (!isSuccess)
                {
                    return BadRequest(new { Message = message, StatusCode = 400 });
                }

                // Nếu không có lỗi, trả về dịch vụ đối tác đã được cập nhật thành công
                return Ok(new { Message = "Dịch vụ đối tác đã được cập nhật thành công.", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message });
            }
        }

        [HttpDelete("service/{id}")]
        [Authorize(Roles = "Partner")]
        public async Task<IActionResult> DeletePartnerService(int id)
        {
            try
            {
                bool isDeleted = await _partnerService.DeletePartnerServiceAsync(id);

                if (isDeleted)
                {
                    return Ok(new { StatusCode = 200, Message = "Dịch vụ đã được xóa thành công." });
                }
                else
                {
                    return BadRequest(new { StatusCode = 400, Message = "Xóa dịch vụ thất bại." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = ex.Message });
            }
        }

        [HttpGet("{id}/schedule")]
        [Authorize(Roles = "Partner,Customer")]
        public async Task<ActionResult<List<Schedule>>> GetScheduleByPartnerId(int id)
        {
            ApiResponse<List<Schedule>> res = new ApiResponse<List<Schedule>>();
            try
            {
                res.data = await _partnerService.GetScheduleByPartnerIdAsyn(id);
                if (res.data == null || res.data.Count == 0)
                {
                    res.message = MessagesResponse.Error.NotFound;
                    res.StatusCode = ApiStatusCode.NotFound;
                }
                else
                {
                    {
                        res.message = MessagesResponse.Success.Completed;
                        res.StatusCode = ApiStatusCode.OK;
                    }
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetScheduleByPartnerId: {ex.Message}", ex);
                res.message = MessagesResponse.Error.OperationFailed;
                res.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(res);
            }
        }

        [HttpGet("type")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<PartnerType>>>> GetPartnerTypesAsync([FromQuery] string? keyword)
        {
            ApiResponse<List<PartnerType>> res = new ApiResponse<List<PartnerType>>();
            try
            {
                List<PartnerType> partnerTypes = await _partnerService.GetPartnerTypesAsync(keyword);
                if (partnerTypes.Count == 0)
                {
                    res.message = MessagesResponse.Error.NotFound;
                    res.StatusCode = ApiStatusCode.NotFound;
                    return Ok(res);
                }

                else
                {
                    res.message = MessagesResponse.Success.Completed;
                    res.StatusCode = ApiStatusCode.OK;
                    res.data = partnerTypes;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPartnerTypesAsync: {ex.Message}", ex);
                res.message = MessagesResponse.Error.OperationFailed;
                res.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(res);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> AddPartnerAsync([FromBody] AddPartnerRequest partner)
        {
            ApiResponse<object> response = new ApiResponse<object>
            {
                message = MessagesResponse.Error.OperationFailed,
                StatusCode = ApiStatusCode.BadRequest,
            };

            try
            {
                if (!ModelState.IsValid)
                {
                    return UnprocessableEntity(response);
                }

                (Partner? data, PartnerError? error) = await _partnerService.AddPartnerAsync(partner);

                if (data == null)
                {
                    response.StatusCode = ApiStatusCode.Conflict;
                    response.data = error;
                    return Conflict(response);
                }

                response.StatusCode = ApiStatusCode.OK;
                response.message = MessagesResponse.Success.Completed;
                response.data = data;
                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException || ex is NotSupportedException)
                {
                    response.message = ex.Message;
                    response.StatusCode = ApiStatusCode.UnprocessableEntity;
                    return UnprocessableEntity(response);
                }
                
                return BadRequest(response);
            }
        }

        [HttpGet("top-five-booked-services")]
        [Authorize(Roles = "Admin,Customer,Partner")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PartnerServiceDTO>>>> GetTopFiveBookedServices()
        {
            ApiResponse<IEnumerable<PartnerServiceDTO>> response = new ApiResponse<IEnumerable<PartnerServiceDTO>>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = 404
            };
            try
            {
                IEnumerable<PartnerServiceDTO> topFiveServices = await _partnerService.GetTopFiveBookedServicesAsync();

                if (topFiveServices == null || !topFiveServices.Any())
                    return response;

                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;
                response.data = topFiveServices;

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(response);
            }
        }



    }
}
