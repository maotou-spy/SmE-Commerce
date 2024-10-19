using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.User;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[ApiController]
[Route("api/users")]
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
    [AllowAnonymous]
    public async Task<IActionResult> CreateManagerUser([FromBody] CreateManagerReqDto req)
    {
        try
        {
            var result = await userService.CreateManagerUser(req);

            if (!result.IsSuccess)
            {
                if (result.InternalErrorMessage is not null)
                {
                    _logger.LogError("Error at create manager user: {ex}", result.InternalErrorMessage);
                }
                return Helper.GetErrorResponse(result.Message);
            }
            return StatusCode(200, result);
        } catch (Exception ex)
        {
            _logger.LogInformation("Error at login with google: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.SERVER_ERROR });
        }
    }
}
