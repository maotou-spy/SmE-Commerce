using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}")]
public class HomepageController(IHomepageService homepageService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpGet("homepage")]
    [OpenApiOperation("Get data for Home", "Get data for Home")]
    public async Task<IActionResult> GetHomepageData()
    {
        try
        {
            var result = await homepageService.GetHomepageDataAsync();

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get homepage data: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at get homepage data: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}