using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.InterfaceService;
using Service.Service;
using System.Collections.Generic;
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

        [HttpGet, Route("{keyword?}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<IEnumerable<SearchPartnerDTO>>> GetPartnerByPartnerOrServiceName(string? keyword = "")
        {
            ApiResponse<IEnumerable<SearchPartnerDTO>> res = new ApiResponse<IEnumerable<SearchPartnerDTO>>
            {
                message = MessagesResponse.Error.OperationFailed,
                StatusCode = 500
            };
            try
            {
                IEnumerable<SearchPartnerDTO> searchPartnerDTOs = await _partnerService.SearchPartnerByPartnerOrServiceName(keyword);
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
    }
}
