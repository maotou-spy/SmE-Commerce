using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/admin")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class AdminController(IOrderService orderService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpGet]
    [OpenApiOperation("Manager get orders", "manager get orders")]
    [Authorize]
    public async Task<IActionResult> ManagerGetOrdersAsync([FromQuery] string statusFilter)
    {
        try
        {
            var result = await orderService.ManagerGetOrdersAsync(statusFilter);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if(result.InternalErrorMessage is not null)
                logger.LogError("Error at get orders: {ex}", result.InternalErrorMessage);
            
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get orders: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}