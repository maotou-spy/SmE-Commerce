using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.VariantName;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[Route("api/variants")]
[Authorize(AuthenticationSchemes = "Defaut")]
public class VariantController(
    IVariantNameService variantNameService,
    ILogger<AuthController> logger
) : ControllerBase
{
    [HttpGet]
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
            logger.LogInformation("Error at get variant attributes: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPost]
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
            logger.LogInformation("Error at bulk create variant attribute: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("{variantId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateVariantNameAsync(
        [FromBody] VariantNameReqDto req,
        Guid variantId
    )
    {
        try
        {
            if (!ModelState.IsValid)
                return StatusCode(400, Helper.GetValidationErrors(ModelState));

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
            logger.LogInformation("Error at update variant attribute: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("{variantId:guid}")]
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
            logger.LogInformation("Error at delete variant attribute: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}
