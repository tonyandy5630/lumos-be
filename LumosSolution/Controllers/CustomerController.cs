using BussinessObject;
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

        [HttpGet("{id}/medical-report")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<List<MedicalReport>>> GetMedicalReportByCustomerIdAsync(int id)
        {
            ApiResponse<List<MedicalReport>> res = new ApiResponse<List<MedicalReport>>();
            try
            {
                res.data = await _customerService.GetMedicalReportByCustomerIdAsync(id);
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
            catch
            {
                res.message = MessagesResponse.Error.OperationFailed;
                res.StatusCode = ApiStatusCode.BadRequest;

                return BadRequest(res);
            }
        }
    }
}
