using BussinessObject;
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
        [HttpGet("{top}/partner")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<List<Partner>>> GetTopPartnerAsync(int top = 5)
        {
            ApiResponse<List<Partner>> response = new ApiResponse<List<Partner>>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = ApiStatusCode.NotFound
            };
            try
            {
                List<Partner> topPartner = await _adminService.GetTopPartnerAsync(top);
                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;
                response.data = topPartner;
                return Ok(response);
            }catch(Exception ex)
            {
                return BadRequest(response);
            }
        }

        [HttpGet("revenue/monthly/{year}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ListDataDTO?>>> GetMonthlyRevenue(int year)
        {
            ApiResponse<ListDataDTO?> response = new ApiResponse<ListDataDTO?>
            {
                message = MessagesResponse.Error.NotFound,
                StatusCode = ApiStatusCode.NotFound
            };

            try
            {
                ListDataDTO monthlyRevenue = await _adminService.GetAppMonthlyRevenueAsync(year);

                if (monthlyRevenue == null)
                    return Ok(response);

                response.message = MessagesResponse.Success.Completed;
                response.StatusCode = ApiStatusCode.OK;
                response.data = monthlyRevenue;

                return Ok(response);
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
