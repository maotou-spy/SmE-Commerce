using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.User;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[Route("api/users")]
[Authorize(AuthenticationSchemes = "Defaut")]
public class UserController : ControllerBase
{
    private readonly IUserService userService;
    private readonly ILogger<AuthController> _logger;

    public UserController(IUserService userService, ILogger<AuthController> logger)
    {
        this.userService = userService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserReqDto req)
    {
        try
        {
            var result = await userService.CreateUser(req);

            if (!result.IsSuccess)
            {
                if (result.InternalErrorMessage is not null)
                {
                    _logger.LogError("Error at create manager user: {ex}", result.InternalErrorMessage);
                }
                return Helper.GetErrorResponse(result.Message);
            }
            return StatusCode(200, result);

        }
        catch (Exception ex)
        {
            _logger.LogInformation("Error at create manager user: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.ServerError });
        }
    }
}
