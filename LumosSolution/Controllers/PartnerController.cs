using BussinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.InterfaceService;
using Service.Service;
using Utils;

namespace LumosSolution.Controllers
{
    [Route("api/partner")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService _partnerService;
        public PartnerController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }

        [HttpGet("service/{id}")]
        [Authorize(Roles = "Admin,Customer,Partner")]
        public async Task<ActionResult<PartnerService?>> GetaPartnerServiceDetailById(int id){
            ApiResponse<PartnerService?> res = await _partnerService.GetPartnerServiceDetailAsync(id);
            return Ok(res);
        }

    }
}
