using BussinessObject;
using DataTransferObject.DTO;
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
using static Utils.MessagesResponse;

namespace LumosSolution.Controllers
{
    [Route("api/partner")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService _partnerService;
        public PartnerController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
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

                if (partnerServices == null || partnerServices.Count == 0)
                    return NotFound(response);

                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = 200;
                response.data = partnerServices;

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                response.message = MessagesResponse.Error.Unauthorized;
                response.StatusCode = 401;
                return StatusCode(401, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = 406;
                return StatusCode(406, response);
            }
        }

        [HttpGet("/api/stat/revenue/monthly/{year}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<List<MonthlyRevenueDTO>>>> GetMonthlyRevenue(int year)
        {
            ApiResponse<List<MonthlyRevenueDTO>> response = new ApiResponse<List<MonthlyRevenueDTO>>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = 404
            };

            try
            {
                List<MonthlyRevenueDTO> monthlyRevenue = await _partnerService.CalculateMonthlyRevenueAsync(year);

                if (monthlyRevenue == null || monthlyRevenue.Count == 0)
                    return NotFound(response);

                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = 200;
                response.data = monthlyRevenue;

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                response.message = "Không có quyền";
                response.StatusCode = 401;
                return StatusCode(401, response);
            }
            catch (Exception)
            {
                response.message = "Lỗi máy chủ nội bộ";
                response.StatusCode = 500;
                return StatusCode(500, response);
            }
        }

        [HttpGet("revenue/{month}")]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<ApiResponse<List<RevenuePerWeekDTO>>>> GetPartnerRevenueInMonth(int month, int? year = null)
        {
            ApiResponse<List<RevenuePerWeekDTO>> response = new ApiResponse<List<RevenuePerWeekDTO>>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = 404
            };

            try
            {
                if (year == null)
                {
                    year = DateTime.Now.Year;
                }

                var revenuePerWeek = await _partnerService.CalculatePartnerRevenueInMonthAsync(month, (int)year);

                if (revenuePerWeek == null || revenuePerWeek.Count() == 0)
                    return NotFound(response);

                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = 200;
                response.data = revenuePerWeek;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.message = $"Internal server error";
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
                StatusCode = 404,
                data = "Không tìm thấy kết quả"
            };

            try
            {

                string? email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                StatPartnerServiceDTO res = await _partnerService.GetStatPartnerServiceAsync(email);


                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = 200;
                response.data = res;

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                response.message = "Không có quyền";
                response.StatusCode = 401;
                return StatusCode(401, response);
            }
            catch (Exception)
            {
                response.message = "Lỗi máy chủ nội bộ";
                response.StatusCode = 500;
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
                    return response;

                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = 200;
                response.data = partnerService;

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(response);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<SearchPartnerDTO>>> GetPartnerByPartnerOrServiceName([FromQuery] string? keyword = "")
        {
            ApiResponse<IEnumerable<SearchPartnerDTO>> res = new ApiResponse<IEnumerable<SearchPartnerDTO>>
            {
                message = MessagesResponse.Error.OperationFailed,
                StatusCode = 500
            };
            try
            {
                IEnumerable<SearchPartnerDTO> searchPartnerDTOs = await _partnerService.SearchPartnerByPartnerOrServiceNameAsync(keyword);
                res.message = MessagesResponse.Success.Completed;
                res.StatusCode = 200;
                res.data = searchPartnerDTOs;
                return Ok(res);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(res);
            }
        }
        [HttpGet("/api/partner/category/{categoryId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<SearchPartnerDTO>>> GetPartnerByCategory(int categoryId)
        {
            ApiResponse<IEnumerable<SearchPartnerDTO>> res = new ApiResponse<IEnumerable<SearchPartnerDTO>>
            {
                message = MessagesResponse.Error.OperationFailed,
                StatusCode = 500
            };
            try
            {
                IEnumerable<SearchPartnerDTO> searchPartnerDTOs = await _partnerService.GetPartnerByCategoryAsync(categoryId);
                res.message = MessagesResponse.Success.Completed;
                res.StatusCode = 200;
                res.data = searchPartnerDTOs;
                return Ok(res);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(res);
            }
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer,Partner")]
        public async Task<ActionResult<ApiResponse<SearchPartnerDTO?>>> GetPartnerById(int id)
        {
            ApiResponse<SearchPartnerDTO?> res = new ApiResponse<SearchPartnerDTO?>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = 404
            };
            try
            {
                SearchPartnerDTO? partner = await _partnerService.GetPartnerByIDAsync(id);

                if (partner == null)
                    return res;

                res.message = MessagesResponse.Success.Completed;
                res.StatusCode = 200;
                res.data = partner;

                return Ok(res);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(res);
            }
        }


        [HttpPost("service")]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<PartnerService>> AddPartnerService([FromBody] AddPartnerServiceResquest service)
        {
            ApiResponse<PartnerService> response = new ApiResponse<PartnerService>
            {
                StatusCode = 500
            };
            try
            {
                if (!ModelState.IsValid)
                {
                    response.message = MessagesResponse.Error.InvalidInput;
                    response.StatusCode = 422;
                    return UnprocessableEntity(response);
                }

                string? email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                PartnerService newService = await _partnerService.AddPartnerServiceAsync(service, email);

                if (newService == null)
                    throw new Exception("Added sevice failed");

                response.message = MessagesResponse.Success.Created;
                response.StatusCode = 200;
                response.data = newService;
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.message = ex.Message;
                return BadRequest(response);
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
                (Partner? data, PartnerError? error) =  await _partnerService.AddPartnereAsync(partner);

                if (data == null)
                {
                    response.StatusCode = 409;
                    response.data = error;
                    return Conflict(response);
                }

                response.StatusCode = ApiStatusCode.OK;
                response.message = MessagesResponse.Success.Completed;
                response.data = data;
                return Ok(response);
            }
            catch(Exception ex)
            {
                if(ex is NullReferenceException)
                {
                    response.message = ex.Message;
                    response.StatusCode = 422;
                    return UnprocessableEntity(response);
                }
                return BadRequest(response);
            }
        }

        [HttpPost("schedule")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<Schedule>>> AddPartnerSchedule([FromBody] Schedule schedule)
        {
            ApiResponse<Schedule> res = new ApiResponse<Schedule>();
            try
            {
                Schedule addedSchedule = await _partnerService.AddPartnerScheduleAsync(schedule);
                if (addedSchedule == null)
                {
                    throw new Exception("Something wrong, Schedule not added");
                }
                else
                {
                    res.message = MessagesResponse.Success.Completed;
                    res.StatusCode = ApiStatusCode.OK;
                    res.data = addedSchedule;
                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddPartnerSchedule: {ex.Message}", ex);
                res.message = MessagesResponse.Error.OperationFailed;
                res.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(res);
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
                response.StatusCode = 200;
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
