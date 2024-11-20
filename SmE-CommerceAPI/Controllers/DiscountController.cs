using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Discount;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceServices.Interface;

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
            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create discount: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
        }
    }

    #endregion

    #region DiscountCode

    [HttpPost("codes")]
    [Authorize]
    public async Task<IActionResult> AddDiscountCodeAsync([FromBody] AddDiscountCodeReqDto req)
    {
        try
        {
            if (!ModelState.IsValid) return StatusCode(400, Helper.GetValidationErrors(ModelState));
            var result = await discountService.AddDiscountCodeAsync(req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at create discount code: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.Message);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create discount code: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
        }
    }
    
    #endregion
}