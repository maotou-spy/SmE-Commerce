using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Auth;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers
{
    [Route("api/auths")]
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : Controller
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginWithAccountReqDto reqDto)
        {
            try
            {
                var result = await authService.LoginWithAccount(reqDto);

                if (result.IsSuccess) return StatusCode(200, result);
                if (result.InternalErrorMessage is not null)
                {
                    logger.LogError("Error at login: {ex}", result.InternalErrorMessage);
                }

                return Helper.GetErrorResponse(result.StatusCode);
            } catch (Exception ex)
            {
                logger.LogError("Error at login: {ex}", ex);
                return Helper.GetErrorResponse(ErrorCode.InternalServerError);
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterWithAccountReqDto reqDto)
        {
            try
            {
                var result = await authService.RegisterWithAccount(reqDto);

                if (result.IsSuccess) return StatusCode(200, result);
                if (result.InternalErrorMessage is not null)
                {
                    logger.LogError("Error at register: {ex}", result.InternalErrorMessage);
                }

                return Helper.GetErrorResponse(result.StatusCode);
            }
            catch (Exception e)
            {
                logger.LogError("Error at register: {ex}", e);
                return Helper.GetErrorResponse(ErrorCode.InternalServerError);
            }
        }
    }
}
