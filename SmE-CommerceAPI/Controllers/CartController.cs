using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Cart;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[Route("api/carts")]
public class CartController(ICartService cartService, ILogger<CartController> logger)
    : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> CustomerGetCartAsync()
    {
        try
        {
            var result = await cartService.CustomerGetCartAsync();

            if (result.IsSuccess)
                return StatusCode(200, result);

            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get cart: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in CustomerGetCartAsync");
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddToCartAsync([FromBody] CartItemReqDto request)
    {
        try
        {
            if (!ModelState.IsValid)
                return StatusCode(400, Helper.GetValidationErrors(ModelState));

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

    [HttpPut("{cartId}")]
    [Authorize]
    public async Task<IActionResult> UpdateCartItemAsync(Guid cartId, [FromBody] int updatedQuantity)
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

    [HttpDelete("{cartId}")]
    [Authorize]
    public async Task<IActionResult> RemoveCartItemByIdAsync(Guid cartId)
    {
        try
        {
            var result = await cartService.CustomerRemoveCartItemByIdAsync(cartId);

            if (result.IsSuccess)
                return StatusCode(200, result);

            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at remove cart item: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in CustomerRemoveCartItemByIdAsync");
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> ClearCartByUserIdAsync()
    {
        try
        {
            var result = await cartService.CustomerClearCartAsync();

            if (result.IsSuccess)
                return StatusCode(200, result);

            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at clear cart: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in CustomerClearCartAsync");
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}
