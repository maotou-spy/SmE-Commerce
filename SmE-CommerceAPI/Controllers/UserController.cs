using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmE_CommerceAPI.HelperClass;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Address;
using SmE_CommerceModels.RequestDtos.User;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/users")]
[Authorize(AuthenticationSchemes = "JwtScheme")]
public class UserController(
    IUserService userService,
    IAddressService addressService,
    ILogger<AuthController> logger
) : ControllerBase
{
    // [HttpPost]
    // [Authorize]
    // public async Task<IActionResult> CreateUser([FromBody] CreateUserReqDto req)
    // {
    //     try
    //     {
    //         if (!ModelState.IsValid)
    //         {
    //             return ErrorCode(400, Helper.GetValidationErrors(ModelState));
    //         }
    //         var result = await userService.CreateUser(req);
    //
    //         if (result.IsSuccess) return ErrorCode(200, result);
    //         if (result.InternalErrorMessage is not null)
    //         {
    //             logger.LogError("Error at create manager user: {ex}", result.InternalErrorMessage);
    //         }
    //         return Helper.GetErrorResponse(result.ErrorCode);
    //
    //     }
    //     catch (Exception ex)
    //     {
    //         logger.LogInformation("Error at create manager user: {e}", ex);
    //         return ErrorCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
    //     }
    // }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetUserProfile()
    {
        try
        {
            var result = await userService.GetUserProfileAsync();

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get user profile: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get user profile: {e}", ex);
            return StatusCode(
                500,
                new Return<IEnumerable<User>> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpGet("{id:guid}/profile")]
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
            logger.LogInformation("Error at get user profile by manager: {e}", ex);
            return StatusCode(
                500,
                new Return<dynamic> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpGet("addresses")]
    [Authorize]
    public async Task<IActionResult> GetUserAddresses(
        [FromQuery] int pageSize,
        [FromQuery] int pageNumber
    )
    {
        try
        {
            var result = await addressService.GetUserAddressesAsync(pageSize, pageNumber);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get user addresses: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get user addresses: {e}", ex);
            return StatusCode(
                500,
                new Return<dynamic> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpPost("addresses")]
    [Authorize]
    public async Task<IActionResult> AddAddress([FromBody] AddressReqDto req)
    {
        try
        {
            if (!ModelState.IsValid)
                return StatusCode(400, Helper.GetValidationErrors(ModelState));

            var result = await addressService.AddAddressAsync(req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at add address: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at add address: {e}", ex);
            return StatusCode(
                500,
                new Return<dynamic> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpPut("addresses/{addressId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateAddress(
        [FromRoute] Guid addressId,
        [FromBody] AddressReqDto req
    )
    {
        try
        {
            if (!ModelState.IsValid)
                return StatusCode(400, Helper.GetValidationErrors(ModelState));

            var result = await addressService.UpdateAddressAsync(addressId, req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update address: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update address: {e}", ex);
            return StatusCode(
                500,
                new Return<dynamic> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpDelete("addresses/{addressId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteAddress([FromRoute] Guid addressId)
    {
        try
        {
            var result = await addressService.DeleteAddressAsync(addressId);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at delete address: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at delete address: {e}", ex);
            return StatusCode(
                500,
                new Return<dynamic> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpPut("addresses/{addressId:guid}/default")]
    [Authorize]
    public async Task<IActionResult> SetDefaultAddress([FromRoute] Guid addressId)
    {
        try
        {
            var result = await addressService.SetDefaultAddressAsync(addressId);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at set default address: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at set default address: {e}", ex);
            return StatusCode(
                500,
                new Return<dynamic> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileReqDto req)
    {
        try
        {
            if (!ModelState.IsValid)
                return StatusCode(400, Helper.GetValidationErrors(ModelState));

            var result = await userService.UpdateProfileAsync(req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update user profile: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update user profile: {e}", ex);
            return StatusCode(500, new Return<bool> { StatusCode = ErrorCode.InternalServerError });
        }
    }

    [HttpGet]
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
            logger.LogInformation("Error at get all users: {e}", ex);
            return StatusCode(
                500,
                new Return<IEnumerable<User>> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
    {
        try
        {
            var result = await userService.DeleteUserAsync(id);
            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at delete user: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at delete user: {e}", ex);
            return StatusCode(
                500,
                new Return<IEnumerable<User>> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpPut("{id}")]
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
            logger.LogInformation("Error at change user status: {e}", ex);
            return StatusCode(
                500,
                new Return<IEnumerable<User>> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpGet("codes")]
    [Authorize]
    public async Task<IActionResult> GetDiscountCodesForCustomerAsync()
    {
        try
        {
            if (!ModelState.IsValid)
                return StatusCode(400, Helper.GetValidationErrors(ModelState));
            var result = await userService.UserGetTheirDiscountsAsync();

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get discount code: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get discount code: {e}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}
