using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Order;
using SmE_CommerceModels.RequestDtos.Payment;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/orders")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class OrderController(IOrderService orderService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost]
    [OpenApiOperation("Create Order", "Create Order")]
    [Authorize]
    public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderReqDto req)
    {
        try
        {
            var result = await orderService.CustomerCreateOrderAsync(req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at create order: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create order: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}