using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.RequestDtos.Discount;
using SmE_CommerceModels.RequestDtos.Discount.DiscountCode;
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
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateDiscountAsync([FromRoute] Guid id,
        [FromBody] UpdateDiscountReqDto updateDiscountReqDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return StatusCode(400, Helper.GetValidationErrors(ModelState));

            var result = await discountService.UpdateDiscountAsync(id, updateDiscountReqDto);

            if (result.IsSuccess)
                return StatusCode(200, result);

            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update discount: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update discount: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetDiscountsForManagerAsync([FromQuery] string? name,[FromQuery] int? pageNumber,[FromQuery] int? pageSize)
    {
        try
        {
            var result = await discountService.GetDiscountsForManagerAsync(name, pageNumber, pageSize);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get discounts: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get discounts: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
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
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
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
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("codes/{codeId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateDiscountCodeAsync([FromRoute] Guid codeId,
        [FromBody] UpdateDiscountCodeReqDto req)
    {
        try
        {
            if (!ModelState.IsValid) return StatusCode(400, Helper.GetValidationErrors(ModelState));
            var result = await discountService.UpdateDiscountCodeAsync(codeId, req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update discount code: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update discount code: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
    
    [HttpGet("codes/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetDiscountCodeByIdAsync([FromRoute] Guid id)
    {
        try
        {
            var result = await discountService.GetDiscountCodeByIdAsync(id);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get discount codes: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get discount codes: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
    
    [HttpDelete("codes/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteDiscountCodeAsync([FromRoute] Guid id)
    {
        try
        {
            if (!ModelState.IsValid)
                return StatusCode(400, Helper.GetValidationErrors(ModelState));

            var result = await discountService.DeleteDiscountCodeAsync(id);

            if (result.IsSuccess)
                return StatusCode(200, result);

            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at delete discount code: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at delete discount code: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
    #endregion
}