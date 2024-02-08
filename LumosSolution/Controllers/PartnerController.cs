using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RequestEntity;
using Service.InterfaceService;
using Service.Service;
using System.Collections.Generic;
using System.Security.Claims;
using Utils;

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

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer,Partner")]
        public async Task<ActionResult<ApiResponse<Partner?>>> GetPartnerById(int id)
        {
            ApiResponse<Partner?> res = new ApiResponse<Partner?>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = 404
            };
            try
            {
                Partner? partner = await _partnerService.GetPartnerByIDAsync(id);

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
        public async Task<ActionResult<ApiResponse<Partner>>> AddPartnerAsync([FromBody] Partner partner)
        {
            ApiResponse<Partner> response = new ApiResponse<Partner>();
            try
            {
                response.data = await _partnerService.AddPartnereAsync(partner);
                if (response.data == null)
                {
                    response.message = MessagesResponse.Error.OperationFailed;
                    response.StatusCode = ApiStatusCode.BadRequest;
                }
                else
                {
                    response.StatusCode = ApiStatusCode.OK;
                    response.message = MessagesResponse.Success.Completed;
                }

                return Ok(response);
            }
            catch
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = ApiStatusCode.BadRequest;

                return BadRequest(response);
            }
        }

        [HttpPost("schedule")]
        [Authorize(Roles = "Partner")]
        public async Task<ActionResult<ApiResponse<List<Schedule>>>> AddPartnerSchedule([FromBody] List<AddPartnerScheduleRequest> scheduleList)
        {
            ApiResponse<List<Schedule>> response = new ApiResponse<List<Schedule>>
            {
                message = MessagesResponse.Error.OperationFailed,
                StatusCode = ApiStatusCode.BadRequest,
            };

            try
            {
                string? email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                List<Schedule> addedSchedules = await _partnerService.AddPartnerScheduleAsync(scheduleList, email);

                response.data = addedSchedules;
                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddPartnerSchedule: {ex.Message}", ex);
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = ApiStatusCode.BadRequest;

                return StatusCode(response.StatusCode, response);
            }
        }

    }
}
