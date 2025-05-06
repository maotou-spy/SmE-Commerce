using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Cart;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/carts")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class CartController(ICartService cartService, ILogger<CartController> logger)
    : ControllerBase
{
    [HttpGet]
    [OpenApiOperation("Get Cart", "Get cart by user id")]
    [Authorize]
    public async Task<IActionResult> CustomerGetCartAsync()
    {
        try
        {
            var result = await cartService.GetCartAsync();

            if (result.IsSuccess)
                return StatusCode(200, result);

            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get cart: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetCartAsync");
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPost]
    [OpenApiOperation("Add To Cart", "Add item to cart")]
    [Authorize]
    public async Task<IActionResult> AddToCartAsync([FromBody] CartItemReqDto request)
    {
        try
        {
            var result = await cartService.AddToCartAsync(request);

            if (result.IsSuccess)
                return StatusCode(200, result);

            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at add to cart: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in AddToCartAsync");
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("{cartId:guid}")]
    [OpenApiOperation("Update Cart Item", "Update cart item quantity")]
    [Authorize]
    public async Task<IActionResult> UpdateCartItemAsync(
        Guid cartId,
        [FromBody] int updatedQuantity
    )
    {
        try
        {
            var result = await cartService.UpdateCartItemAsync(cartId, updatedQuantity);

            if (result.IsSuccess)
                return StatusCode(200, result);

            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update cart item: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in UpdateCartItemAsync");
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("{cartId:guid}")]
    [OpenApiOperation("Remove Cart Item", "Remove cart item by id")]
    [Authorize]
    public async Task<IActionResult> RemoveCartItemByIdAsync(Guid cartId)
    {
        try
        {
            var result = await cartService.RemoveCartItemByIdAsync(cartId);

            if (result.IsSuccess)
                return StatusCode(200, result);

            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at remove cart item: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in RemoveCartItemByIdAsync");
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("remove-multiple")]
    [OpenApiOperation("Remove Cart Items", "Remove cart items by ids")]
    [Authorize]
    public async Task<IActionResult> RemoveCartItemByIdsAsync([FromBody] List<Guid> cartIds)
    {
        try
        {
            var result = await cartService.RemoveCartItemByIdsAsync(cartIds);

            if (result.IsSuccess)
                return StatusCode(200, result);

            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at remove cart items: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in RemoveCartItemByIdsAsync");
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete]
    [OpenApiOperation("Clear Cart", "Clear cart by user id")]
    [Authorize]
    public async Task<IActionResult> ClearCartByUserIdAsync()
    {
        try
        {
            var result = await cartService.ClearCartAsync();

            if (result.IsSuccess)
                return StatusCode(200, result);

            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at clear cart: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in ClearCartAsync");
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}
