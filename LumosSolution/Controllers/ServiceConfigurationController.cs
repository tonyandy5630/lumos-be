using BussinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.InterfaceService;
using Utils;

namespace LumosSolution.Controllers
{
    [Route("api/configuration")]
    [ApiController]
    public class SystemConfigurationController : ControllerBase
    {
        private readonly ISystemConfigurationService _systemService;

        public SystemConfigurationController(ISystemConfigurationService systemService)
        {
            _systemService = systemService;
        }

        [HttpGet("detail/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<SystemConfiguration>>> GetSystemConfigurationDetail(int id)
        {
            ApiResponse<SystemConfiguration> response = new ApiResponse<SystemConfiguration>
            {
                message = MessagesResponse.Error.OperationFailed,
                StatusCode = 500
            };
            try
            {
                SystemConfiguration? config = await _systemService.GetSystemConfigurationDetailById(id);
                if(config == null)
                {
                    response.message = MessagesResponse.Error.NotFound;
                        response.StatusCode = 404;
                        return NotFound(response);
                };
                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = 200;
                response.data = config;
                return Ok(response);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                response.message = ex.Message;
                return BadRequest(response);
            }
        }


        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<IEnumerable<SystemConfiguration>>> SearchSystemConfigByName(string? keyword)
        {
            ApiResponse<IEnumerable<SystemConfiguration>> response = new ApiResponse<IEnumerable<SystemConfiguration>>
            {
                message = MessagesResponse.Error.OperationFailed,
                StatusCode = 500
            };
            try
            {
                IEnumerable<SystemConfiguration> systemConfigurations = await _systemService.SearchSystemConfigByNameAsync(keyword);
                response.message = MessagesResponse.Success.Completed;
                response.data = systemConfigurations;
                response.StatusCode = 200;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                BadRequest(response);
            }
            return Ok(response);
        }

    }
}
