using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Category;
using SmE_CommerceServices.Interface;
using ErrorCode = SmE_CommerceModels.Enums.ErrorCode;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class CategoryController(ICategoryService categoryService, ILogger<AuthController> logger)
    : ControllerBase
{
    [HttpPost("admin/categories")]
    [OpenApiOperation("Add Category", "Add Category")]
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
            logger.LogError("Error at create category user: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("categories")]
    [OpenApiOperation("Get Active Categories", "Get Active Categories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoriesForCustomerAsync()
    {
        try
        {
            var result = await categoryService.GetCategoriesForCustomerAsync();
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
            logger.LogError("Error at get categories for customer: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("admin/categories")]
    [OpenApiOperation("Get Categories", "Get All Categories")]
    [Authorize]
    public async Task<IActionResult> GetCategoriesForManagerAsync(
        string? name,
        string? status,
        int pageNumber = PagingEnum.PageNumber,
        int pageSize = PagingEnum.PageSize
    )
    {
        try
        {
            var result = await categoryService.GetCategoriesForManagerAsync(
                name,
                status,
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
            logger.LogError("Error at get categories for manager: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("admin/categories/{id:guid}")]
    [OpenApiOperation("Update Category", "Update Category")]
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
            logger.LogError("Error at update category detail: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("admin/categories/{id:guid}")]
    [OpenApiOperation("Delete Category", "Delete Category")]
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
            logger.LogError("Error at delete category: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("admin/categories/{id:guid}/status")]
    [OpenApiOperation("Update Category Status", "Update Category Status")]
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
            logger.LogError("Error at update category status: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}
