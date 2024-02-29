using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using RequestEntity;
using Service.InterfaceService;
using static RequestEntity.Constraint.Constraint;

namespace LumosSolution.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreatePaymentLink([FromBody] PaymentRequest request)
        {
            try
            {
                string checkoutUrl = await _paymentService.CreatePaymentLink(request);
                return Ok(checkoutUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating payment link: {ex.Message}");
            }
        }
    }
}
