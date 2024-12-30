using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Category;
using SmE_CommerceServices.Interface;
using ErrorCode = SmE_CommerceModels.Enums.ErrorCode;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/categories")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class CategoryController(ICategoryService categoryService, ILogger<AuthController> logger)
    : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddCategoryAsync([FromBody] AddCategoryReqDto req)
    {
        try
        {
            var result = await categoryService.AddCategoryAsync(req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at create category user: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create category user: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("active")]
    [Authorize]
    public async Task<IActionResult> GetCategoriesForCustomerAsync(
        string? name,
        int pageNumber = PagingEnum.PageNumber,
        int pageSize = PagingEnum.PageSize
    )
    {
        try
        {
            var result = await categoryService.GetCategoriesForCustomerAsync(
                name,
                pageNumber,
                pageSize
            );
            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at get categories for customer: {ex}",
                    result.InternalErrorMessage
                );
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get categories for customer: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCategoriesForManagerAsync(
        string? name,
        int pageNumber = PagingEnum.PageNumber,
        int pageSize = PagingEnum.PageSize
    )
    {
        try
        {
            var result = await categoryService.GetCategoriesForManagerAsync(
                name,
                pageNumber,
                pageSize
            );
            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at get categories for manager: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get categories for manager: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateCategoryDetailAsync(
        [FromRoute] Guid id,
        [FromBody] AddCategoryReqDto req
    )
    {
        try
        {
            var result = await categoryService.UpdateCategoryDetailAsync(id, req);
            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at update category detail: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update category detail: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteCategoryAsync([FromRoute] Guid id)
    {
        try
        {
            var result = await categoryService.DeleteCategoryAsync(id);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at delete category: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at delete category: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("{id:guid}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateCategoryStatusAsync([FromRoute] Guid id)
    {
        try
        {
            var result = await categoryService.UpdateCategoryStatusAsync(id);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at update category status: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update category status: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}
