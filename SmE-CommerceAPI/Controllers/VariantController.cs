using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.VariantName;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class VariantController(
    IVariantNameService variantNameService,
    ILogger<AuthController> logger
) : ControllerBase
{
    [HttpGet("variants")]
    [OpenApiOperation("GetVariantNames", "Get all variant names")]
    [Authorize]
    public async Task<IActionResult> GetVariantNamesAsync()
    {
        try
        {
            var result = await variantNameService.GetVariantNamesAsync();
            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at get variant attributes: {ex}",
                    result.InternalErrorMessage
                );
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at get variant attributes: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPost("admin/variants")]
    [OpenApiOperation("CreateVariantName", "Create a new variant name")]
    [Authorize]
    public async Task<IActionResult> BulkCreateVariantNameAsync([FromBody] List<string> req)
    {
        try
        {
            var result = await variantNameService.BulkVariantNameAsync(req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at bulk create variant attribute: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at bulk create variant attribute: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("admin/variants/{variantId:guid}")]
    [OpenApiOperation("UpdateVariantName", "Update a variant name")]
    [Authorize]
    public async Task<IActionResult> UpdateVariantNameAsync(
        [FromBody] VariantNameReqDto req,
        Guid variantId
    )
    {
        try
        {
            var result = await variantNameService.UpdateVariantNameAsync(variantId, req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at update variant attribute: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at update variant attribute: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("admmin/variants/{variantId:guid}")]
    [OpenApiOperation("DeleteVariantName", "Delete a variant name")]
    [Authorize]
    public async Task<IActionResult> DeleteVariantNameAsync(Guid variantId)
    {
        try
        {
            var result = await variantNameService.DeleteVariantNameAsync(variantId);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at delete variant attribute: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at delete variant attribute: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}
