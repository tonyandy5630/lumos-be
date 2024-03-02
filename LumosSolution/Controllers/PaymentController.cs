using BussinessObject;
using DataTransferObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using RequestEntity;
using Service.InterfaceService;
using Utils;
using static Google.Apis.Requests.BatchRequest;
using static RequestEntity.Constraint.Constraint;

namespace LumosSolution.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentMethodService  _paymentMethodService;
        public PaymentController(IPaymentService paymentService, IPaymentMethodService paymentMethodService)
        {
            _paymentService = paymentService;
            _paymentMethodService = paymentMethodService;
        }

        [HttpPost]
        public async Task<ActionResult<PaymentResponse>> CreatePaymentLink([FromBody] PaymentRequest request)
        {
            try
            {
                var  res = await _paymentService.CreatePaymentLink(request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error CreatePaymentLink: {ex.Message}");
            }
        }
        [HttpGet("method")]
        [Authorize(Roles = "Admin,Customer,Partner")]
        public async Task<ActionResult<List<PaymentMethod>>> GetAllPaymentMethod()
        {
            ApiResponse<List<PaymentMethod>> response = new ApiResponse<List<PaymentMethod>>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = ApiStatusCode.NotFound
            };
            try
            {
                response.data = await _paymentMethodService.GetAllPaymentMethodAsync();
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
            catch (UnauthorizedAccessException)
            {
                response.message = MessagesResponse.Error.Unauthorized;
                response.StatusCode = ApiStatusCode.Unauthorized;
                return StatusCode(401, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.message = ex.Message;
                return StatusCode(500, response);
            }
        }
    }
}
