using AutoMapper;
using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestEntity;
using Service.InterfaceService;
using Service.Service;
using System.Security.Claims;
using Utils;
using static Google.Apis.Requests.BatchRequest;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace LumosSolution.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICustomerService _customerService;

        public CustomerController(IMapper mapper, ICustomerService customerService)
        {
            _mapper = mapper;
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
                    return NotFound(res);
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
                Console.WriteLine(ex.Message);
                res.message = ex.Message;
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
                Console.WriteLine(ex.Message);
                response.message = ex.Message;
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost, Route("medical-report")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ApiResponse<MedicalReport>>> AddMedicalReport([FromBody] MedicalReportDTO medicalReport)
        {
            ApiResponse<MedicalReport> response = new ApiResponse<MedicalReport>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = 404
            };

            try
            {
                bool existingReport = await _customerService.CheckExistingMedicalReportAsync(medicalReport.Fullname);
                if (existingReport)
                {
                    response.message = MessagesResponse.Error.MedicalReportExists;
                    return BadRequest(response);
                }
                string? userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(userEmail))
                {
                    response.message = MessagesResponse.Error.UserEmailNotFound;
                    return BadRequest(response);
                }

                MedicalReport med = await _customerService.AddMedicalReportAsync(medicalReport, userEmail);

                if (med == null)
                    return response;

                response.message = MessagesResponse.Success.Created;
                response.StatusCode = ApiStatusCode.OK;
                response.data = med;

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("address")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ApiResponse<Address>>> AddCustomerAddressAsync([FromBody] AddAddressRequest addressrequest)
        {
            ApiResponse<Address> response = new ApiResponse<Address>();
            try
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                bool existingAddress = await _customerService.CheckExistingAddressAsync(addressrequest.address1);
                if (existingAddress)
                {
                    response.message = MessagesResponse.Error.AddressExists;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
                }
                var address = _mapper.Map<Address>(addressrequest);
                response.data = await _customerService.AddCustomerAddressAsync(address, userEmail);
                if (response.data == null)
                {
                    response.message = MessagesResponse.Error.OperationFailed;
                    response.StatusCode = ApiStatusCode.BadRequest;
                    return BadRequest(response);
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
                Console.WriteLine(ex.Message);
                response.message = ex.Message;
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
                Console.WriteLine(ex.Message);
                response.message = ex.Message;
                return BadRequest(response);
            }
        }
    }
}
