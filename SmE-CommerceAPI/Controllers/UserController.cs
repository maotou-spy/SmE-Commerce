using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
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
    //             return ErrorCode(400, Helper.ReturnValidationErrors(ModelState));
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
    //         logger.LogInformation("Error at create manager user: {ex}", ex);
    //         return ErrorCode(500, new Return<bool> { Message = ErrorMessage.InternalServerError });
    //     }
    // }

    [HttpGet("profile")]
    [OpenApiOperation("Get User Profile", "Get user profile")]
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
            logger.LogInformation("Error at get user profile: {ex}", ex);
            return StatusCode(
                500,
                new Return<IEnumerable<User>> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpGet("addresses")]
    [OpenApiOperation("Get User Addresses", "Get user addresses")]
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
            logger.LogInformation("Error at get user addresses: {ex}", ex);
            return StatusCode(
                500,
                new Return<dynamic> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpPost("addresses")]
    [OpenApiOperation("Add Address", "Add address")]
    [Authorize]
    public async Task<IActionResult> AddAddress([FromBody] AddressReqDto req)
    {
        try
        {
            var result = await addressService.AddAddressAsync(req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at add address: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at add address: {ex}", ex);
            return StatusCode(
                500,
                new Return<dynamic> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpPut("addresses/{addressId:guid}")]
    [OpenApiOperation("Update Address", "Update address")]
    [Authorize]
    public async Task<IActionResult> UpdateAddress(
        [FromRoute] Guid addressId,
        [FromBody] AddressReqDto req
    )
    {
        try
        {
            var result = await addressService.UpdateAddressAsync(addressId, req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update address: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update address: {ex}", ex);
            return StatusCode(
                500,
                new Return<dynamic> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpDelete("addresses/{addressId:guid}")]
    [OpenApiOperation("Delete Address", "Delete address")]
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
            logger.LogInformation("Error at delete address: {ex}", ex);
            return StatusCode(
                500,
                new Return<dynamic> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpPut("addresses/{addressId:guid}/default")]
    [OpenApiOperation("Set Default Address", "Set default address")]
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
            logger.LogInformation("Error at set default address: {ex}", ex);
            return StatusCode(
                500,
                new Return<dynamic> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpPut("profile")]
    [OpenApiOperation("Update User Profile", "Update user profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileReqDto req)
    {
        try
        {
            var result = await userService.UpdateProfileAsync(req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at update user profile: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update user profile: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpDelete("{id:guid}")]
    [OpenApiOperation("Delete User", "Delete user")]
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
            logger.LogInformation("Error at delete user: {ex}", ex);
            return StatusCode(
                500,
                new Return<IEnumerable<User>> { StatusCode = ErrorCode.InternalServerError }
            );
        }
    }

    [HttpGet("codes")]
    [OpenApiOperation("Get Discount Codes For Customer", "Get discount codes for customer")]
    [Authorize]
    public async Task<IActionResult> GetDiscountCodesForCustomerAsync()
    {
        try
        {
            var result = await userService.UserGetTheirDiscountsAsync();

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at get discount code: {ex}", result.InternalErrorMessage);
            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get discount code: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }

    [HttpPut("password")]
    [OpenApiOperation("Change Password", "Change password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordReqDto req)
    {
        try
        {
            var result = await userService.ChangePassword(req);

            if (result.IsSuccess)
                return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
                logger.LogError("Error at change password: {ex}", result.InternalErrorMessage);

            return Helper.GetErrorResponse(result.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at change password: {ex}", ex);
            return Helper.GetErrorResponse(ErrorCode.InternalServerError);
        }
    }
}
