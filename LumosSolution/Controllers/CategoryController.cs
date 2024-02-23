using BussinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.InterfaceService;
using Utils;

namespace LumosSolution.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController : ControllerBase
    {
        private readonly IServiceCategorySer _serviceCategorySer;
        public CategoryController(IServiceCategorySer serviceCategorySer)
        {
            _serviceCategorySer = serviceCategorySer;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer,Partner")]
        public async Task<ActionResult<List<ServiceCategory>>> GetCategorysAsync(string? keyword)
        {
            ApiResponse<List<ServiceCategory>> response = new ApiResponse<List<ServiceCategory>>();
            try
            {
                response.data = await _serviceCategorySer.GetCategorysAsync(keyword);
                if (response.data == null || response.data.Count == 0)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = ApiStatusCode.NotFound;
                    return NotFound(response);
                }
                else
                {
                    response.message = MessagesResponse.Success.Completed;
                    response.StatusCode = ApiStatusCode.OK;
                    return Ok(response);
                }
            }
            catch
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }
    } 
}
