using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
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
            var result = await authService.LoginWithAccount(reqDto);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
            {
                logger.LogError("Error at login: {ex}", result.InternalErrorMessage);
            }
            return Helper.GetErrorResponse(result.Message);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterWithAccountReqDto reqDto)
        {
            var result = await authService.RegisterWithAccount(reqDto);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
            {
                logger.LogError("Error at register: {ex}", result.InternalErrorMessage);
            }
            return Helper.GetErrorResponse(result.Message);
        }
    }
}
