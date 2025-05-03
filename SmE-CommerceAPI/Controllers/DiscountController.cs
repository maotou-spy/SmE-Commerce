using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.RequestDtos.Discount;
using SmE_CommerceModels.RequestDtos.Discount.DiscountCode;
using SmE_CommerceServices.Interface;
using ErrorCode = SmE_CommerceModels.Enums.ErrorCode;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class DiscountController(ILogger<AuthController> logger, IDiscountService discountService)
    : ControllerBase
{
    #region Discount

    [HttpGet("admin/discounts/{id:guid}/codes")]
    [OpenApiOperation("Get Discount Codes By DiscountId", "Get Discount Codes By DiscountId")]
    [Authorize]
    public async Task<IActionResult> GetDiscountCodesByDiscountIdAsync(
        [FromRoute] Guid id,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize
    )
    {
        try
        {
            var result = await discountService.GetDiscountCodeByDiscountIdAsync(
                id,
                pageNumber,
                pageSize
            );

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get discount codes: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at get discount codes: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPost("admin/discounts")]
    [Authorize]
    public async Task<IActionResult> AddDiscountAsync([FromBody] AddDiscountReqDto req)
    {
        try
        {
            var result = await discountService.AddDiscountAsync(req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at create discount: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at create discount: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("admin/discounts/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateDiscountAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateDiscountReqDto updateDiscountReqDto
    )
    {
        try
        {
            var result = await discountService.UpdateDiscountAsync(id, updateDiscountReqDto);

            if (result.IsSuccess)
                return StatusCode(200, result);

            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update discount: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at update discount: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("admin/discounts")]
    public async Task<IActionResult> GetDiscountsForManagerAsync(
        [FromQuery] string? name,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize
    )
    {
        try
        {
            var result = await discountService.GetDiscountsForManagerAsync(
                name,
                pageNumber,
                pageSize
            );

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get discounts: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at get discounts: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("admin/discounts/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteDiscountAsync([FromRoute] Guid id)
    {
        try
        {
            var result = await discountService.DeleteDiscountAsync(id);

            if (result.IsSuccess)
                return StatusCode(200, result);

            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at delete discount: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at delete discount: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    #endregion

    #region DiscountCode

    [HttpPost("admin/discounts/{id:guid}/codes")]
    [OpenApiOperation("Add Discount Code", "Add Discount Code")]
    [Authorize]
    public async Task<IActionResult> AddDiscountCodeAsync(
        [FromRoute] Guid id,
        [FromBody] AddDiscountCodeReqDto req
    )
    {
        try
        {
            var result = await discountService.AddDiscountCodeAsync(id, req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at create discount code: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at create discount code: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("discounts/codes/{code}")]
    [OpenApiOperation("Get Discount Code By Code", "Get Discount Code By Code")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDiscountCodeByCodeAsync([FromRoute] string code)
    {
        try
        {
            var result = await discountService.GetDiscounCodeByCodeAsync(code);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get discount code: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at get discount code: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("admin/discounts/codes/{codeId:guid}")]
    [OpenApiOperation("Update Discount Code", "Update Discount Code")]
    [Authorize]
    public async Task<IActionResult> UpdateDiscountCodeAsync(
        [FromRoute] Guid codeId,
        [FromBody] UpdateDiscountCodeReqDto req
    )
    {
        try
        {
            var result = await discountService.UpdateDiscountCodeAsync(codeId, req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update discount code: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at update discount code: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("discounts/codes/{id:guid}")]
    [OpenApiOperation("Get Discount Code By CodeId", "Get Discount Code By CodeId")]
    [Authorize]
    public async Task<IActionResult> GetDiscountCodeByIdAsync([FromRoute] Guid id)
    {
        try
        {
            var result = await discountService.GetDiscountCodeByIdAsync(id);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get discount codes: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at get discount codes: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("admin/discounts/codes/{id:guid}")]
    [OpenApiOperation("Delete Discount Code", "Delete Discount Code")]
    [Authorize]
    public async Task<IActionResult> DeleteDiscountCodeAsync([FromRoute] Guid id)
    {
        try
        {
            var result = await discountService.DeleteDiscountCodeAsync(id);

            if (result.IsSuccess)
                return StatusCode(200, result);

            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at delete discount code: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at delete discount code: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    #endregion
}
