using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.RequestDtos.Discount;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceServices.Interface;
using ErrorCode = SmE_CommerceModels.Enums.ErrorCode;

namespace SmE_CommerceAPI.Controllers;

[Route("api/discounts")]
[Authorize(AuthenticationSchemes = "Defaut")]
public class DiscountController(ILogger<AuthController> logger, IDiscountService discountService) : ControllerBase
{
    #region Discount

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddDiscountAsync([FromBody] AddDiscountReqDto req)
    {
        try
        {
            if (!ModelState.IsValid) return StatusCode(400, Helper.GetValidationErrors(ModelState));
            var result = await discountService.AddDiscountAsync(req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at create discount: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create discount: {e}", ex);
            return StatusCode(500, new Return<bool>
            {
                StatusCode = ErrorCode.InternalServerError
            });
        }
    }
    
    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateDiscountAsync([FromRoute] Guid id, [FromBody] AddDiscountReqDto req)
    {
        try
        {
            if (!ModelState.IsValid) return StatusCode(400, Helper.GetValidationErrors(ModelState));
            var result = await discountService.UpdateDiscountAsync(id, req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update discount: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update discount: {e}", ex);
            return StatusCode(500, new Return<bool>
            {
                StatusCode = ErrorCode.InternalServerError
            });
        }
    }

    #endregion

    #region DiscountCode

    [HttpPost("{id:guid}/codes")]
    [Authorize]
    public async Task<IActionResult> AddDiscountCodeAsync([FromRoute] Guid id, [FromBody] AddDiscountCodeReqDto req)
    {
        try
        {
            if (!ModelState.IsValid) return StatusCode(400, Helper.GetValidationErrors(ModelState));
            var result = await discountService.AddDiscountCodeAsync(id, req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at create discount code: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create discount code: {e}", ex);
            return StatusCode(500, new Return<bool>
            {
                StatusCode = ErrorCode.InternalServerError
            });
        }
    }

    [HttpGet("codes/{code}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDiscountCodeByCodeAsync([FromRoute] string code)
    {
        try
        {
            var result = await discountService.GetDiscounCodeByCodeAsync(code);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get discount code: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get discount code: {e}", ex);
            return StatusCode(500, new Return<bool>
            {
                StatusCode = ErrorCode.InternalServerError
            });
        }
    }

    #endregion
}
