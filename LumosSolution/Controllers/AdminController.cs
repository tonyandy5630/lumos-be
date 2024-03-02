using DataTransferObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.InterfaceService;
using Service.Service;
using Utils;

namespace LumosSolution.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("dashboard/stat")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<ApiResponse<AdminDashboardStat>>> GetAdminDashboardStat()
        {
            ApiResponse<AdminDashboardStat> response = new ApiResponse<AdminDashboardStat>
            {
                message = MessagesResponse.Error.OperationFailed,
                StatusCode = ApiStatusCode.BadRequest
            };
            try
            {
                AdminDashboardStat stats = await _adminService.GetAdminDashboardStatAsync();
                response.message = MessagesResponse.Success.Completed;
                return Ok(stats);

            }catch (Exception ex)
            {
                return BadRequest(response);
            }
        }
        [HttpGet("user/{year}/monthly")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<NewUserMonthlyChartDTO?>>> GetMonthlyAppNewUser(int year)
        {
            ApiResponse<NewUserMonthlyChartDTO?> res = new ApiResponse<NewUserMonthlyChartDTO?>
            {
                message = MessagesResponse.Error.OperationFailed,
                StatusCode = 400
            };
            try
            {
                NewUserMonthlyChartDTO monthlyUser = await _adminService.GetAppNewUserMonthlyAsync(year);
                res.message = MessagesResponse.Success.Completed;
                res.StatusCode = 200;
                res.data = monthlyUser;
                return Ok(res);
            }
            catch
            {
                return BadRequest(res);
            }
        }

    }
}
