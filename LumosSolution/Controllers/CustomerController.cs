using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.InterfaceService;
using Service.Service;
using Utils;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace LumosSolution.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Customer>>> GetCustomerAsync([FromQuery] string? keyword)
        {
            ApiResponse<List<Customer>> response = new ApiResponse<List<Customer>>();
            try
            {
                response.data = await _customerService.GetCustomersAsync(keyword);
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
            catch
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = ApiStatusCode.BadRequest;

                return BadRequest(response);
            }
        }

        [HttpPost, Route("medical-report")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ApiResponse<MedicalReportDTO?>>> AddMedicalReport([FromBody] MedicalReport medicalReport)
        {
            ApiResponse<MedicalReportDTO?> response = new ApiResponse<MedicalReportDTO?>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = 404
            };
            try
            {
                MedicalReportDTO? med = await _customerService.AddMedicalReportAsyn(medicalReport);

                if (med == null)
                    return response;

                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = 200;
                response.data = med;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.message = MessagesResponse.Error.OperationFailed;
                response.StatusCode = ApiStatusCode.BadRequest;
                return BadRequest(response);
            }
        }
        
        [HttpPost("address")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ApiResponse<Address>>> AddCustomerAddressAsync([FromBody] Address address)
        {
            ApiResponse<Address> response = new ApiResponse<Address>();
            try
            {
                response.data = await _customerService.AddCustomerAddressAsync(address);
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

        [HttpGet("medical-report/{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<MedicalReport>> GetMedicalReportById(int id)
        {
            ApiResponse<MedicalReport> response = new ApiResponse<MedicalReport>();
            try
            {
                response.data = await _customerService.GetMedicalReportByIdAsync(id);
                if (response.data == null)
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
