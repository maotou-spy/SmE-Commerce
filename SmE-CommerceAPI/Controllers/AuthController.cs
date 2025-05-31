using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Auth;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/auths")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : Controller
{
    [HttpPost("login")]
    [OpenApiOperation("Login with account", "Login with account")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] LoginWithAccountReqDto reqDto)
    {
        try
        {
            var result = await authService.LoginWithAccount(reqDto);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at login: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError("Error at login: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPost("register")]
    [OpenApiOperation("Register with account", "Register with account")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterWithAccountReqDto reqDto)
    {
        try
        {
            if (reqDto.ConfirmPassword.Trim() != reqDto.Password.Trim())
            {
                return Helper.GetErrorResponse(ErrorCode.ConfirmPasswordNotMatch);
            } 
            
            var result = await authService.RegisterWithAccount(reqDto);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at register: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception e)
        {
            logger.LogError("Error at register: {ex}", e);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}
