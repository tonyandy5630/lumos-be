using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Service.InterfaceService;
using Service.RequestEntity;
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
        public async Task<ActionResult<IEnumerable<SearchPartnerDTO>>> GetPartnerByPartnerOrServiceName(string keyword = "")
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

        [HttpGet("/{id}")]
        [Authorize(Roles = "Admin,Customer,Partner")]
        public async Task<ActionResult<Partner?>> GetPartnerById(int id)
        {
            ApiResponse<Partner?> res = await _partnerService.GetPartnerByIDAsync(id);
            return Ok(res);
        }

        [HttpPost("service")]
        public async Task<ActionResult<PartnerService>> AddPartnerService([FromBody]AddPartnerServiceResquest service)
        {
            ApiResponse<PartnerService> response = new ApiResponse<PartnerService>
            {
                message = MessagesResponse.Error.OperationFailed,
                StatusCode = 500
            };
            try
            {
                if(ModelState.IsValid)
                {
                    response.message = MessagesResponse.Error.InvalidInput;
                    response.StatusCode = 422;
                    return UnprocessableEntity(response);
                }
                string? accessToken = User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
                PartnerService newService = await _partnerService.AddPartnerServiceAsync(service, accessToken);
                return Ok(response);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message) ;
                return BadRequest(response);
            }
        }

    }
}
