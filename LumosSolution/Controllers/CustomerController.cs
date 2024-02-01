using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.InterfaceService;
using Utils;

namespace LumosSolution.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{id}/address")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<List<Address>>> GetCustomerAddressByCustomerIdAsync(int id)
        {
            ApiResponse<List<Address>> response = new ApiResponse<List<Address>>();
            try
            {
                response.data = await _customerService.GetCustomerAddressByCustomerIdAsync(id);
                if (response.data == null || response.data.Count == 0)
                {
                    response.message = MessagesResponse.Error.NotFound;
                    response.StatusCode = ApiStatusCode.NotFound;
                }
                else
                {
                    response.StatusCode = ApiStatusCode.OK;
                    response.message = MessagesResponse.Success.Completed;
                }

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
