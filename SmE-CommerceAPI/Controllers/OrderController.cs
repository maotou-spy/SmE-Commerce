using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Order;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class OrderController(IOrderService orderService, ILogger<AuthController> logger)
    : ControllerBase
{
    [HttpPost("orders")]
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
            logger.LogError("Error at create order: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("orders/{orderId:guid}")]
    [OpenApiOperation("Get Order Detail", "Get Order Detail")]
    [Authorize(AuthenticationSchemes = "JwtScheme")]
    public async Task<IActionResult> GetOrderByIdAsync([FromRoute] Guid orderId)
    {
        try
        {
            var result = await orderService.GetOrderByIdAsync(orderId);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get order detail: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at get order detail: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("orders")]
    [OpenApiOperation("Get orders", "get orders")]
    [Authorize]
    public async Task<IActionResult> GetOrdersAsync([FromQuery] OrderFilterReqDto filter)
    {
        try
        {
            var result = await orderService.GetOrdersAsync(filter);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get orders: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at get orders: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
    
    [HttpPut("admin/orders/status")]
    [OpenApiOperation("Manager Update Order Status", "Manager Update Order Status")]
    [Authorize]
    public async Task<IActionResult> ManagerUpdateOrderStatusAsync([FromBody] ManagerUpdateOrderStatusReqDto req)
    {
        try
        {
            var result = await orderService.ManagerUpdateOrderStatusAsync(req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update order status: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at update order status: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
    
    [HttpPut("orders/status")]
    [OpenApiOperation("Customer Update Order Status", "Customer Update Order Status")]
    [Authorize]
    public async Task<IActionResult> CustomerUpdateOrderStatusAsync([FromBody] CustomerUpdateOrderStatusReqDto req)
    {
        try
        {
            var result = await orderService.CustomerUpdateOrderStatusAsync(req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update order status: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at update order status: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}
