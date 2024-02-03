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
