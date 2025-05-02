using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/admin/users")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class UserController(
    IUserService userService,
    IAddressService addressService,
    ILogger<AuthController> logger
) : ControllerBase
{
    [HttpGet("{id:guid}/profile")]
    [OpenApiOperation("Get User Profile By Manager", "Get user profile by manager")]
    [Authorize]
    public async Task<IActionResult> GetUserProfileByManager([FromRoute] Guid id)
    {
        try
        {
            var result = await userService.GetUserProfileByManagerAsync(id);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError(
                    "Error at get user profile by manager: {ex}",
                    result.InternalErrorMessage
                );

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get user profile by manager: {ex}", ex);
            return StatusCode(
                500,
                new Return<dynamic> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpGet]
    [OpenApiOperation("Get All Users", "Get all users")]
    [Authorize]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] string? status,
        [FromQuery] int? pageSize,
        [FromQuery] int? pageNumber,
        [FromQuery] string? phone,
        [FromQuery] string? email,
        [FromQuery] string name
    )
    {
        try
        {
            var result = await userService.GetAllUsersAsync(
                status,
                pageSize,
                pageNumber,
                phone,
                email,
                name
            );

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get all users: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get all users: {ex}", ex);
            return StatusCode(
                500,
                new Return<IEnumerable<User>> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpPut("{id:guid}/status")]
    [OpenApiOperation("Change User Status", "Change user status")]
    [Authorize]
    public async Task<IActionResult> ChangeUserStatus([FromRoute] Guid id)
    {
        try
        {
            var result = await userService.ChangeUserStatusAsync(id);
            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at change user status: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at change user status: {ex}", ex);
            return StatusCode(
                500,
                new Return<IEnumerable<User>> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }
}
