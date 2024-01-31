using Microsoft.AspNetCore.Mvc;
using Service.InterfaceService;

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
    }
}
