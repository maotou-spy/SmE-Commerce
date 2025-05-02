using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Setting;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class SettingController(ISettingService settingService, ILogger<AuthController> logger)
    : ControllerBase
{
    [HttpGet("settings")]
    [OpenApiOperation("Get Settings", "Get all system's settings")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSettings()
    {
        try
        {
            var result = await settingService.GetSettingsAsync();

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get settings: {e}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get settings: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpGet("settings/{key}")]
    [OpenApiOperation("Get Setting By Key", "Get a system's setting by key")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSettingByKey(string key)
    {
        try
        {
            var result = await settingService.GetSettingByKeyAsync(key);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get setting by key: {e}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get setting by key: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("admin/settings")]
    [OpenApiOperation("Update Settings", "Update system's settings")]
    [Authorize]
    public async Task<IActionResult> UpdateSettings(List<SettingReqDto> settings)
    {
        try
        {
            var result = await settingService.UpdateSettingAsync(settings);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update settings: {e}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update settings: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}
