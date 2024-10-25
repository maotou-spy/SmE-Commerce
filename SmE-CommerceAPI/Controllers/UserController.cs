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

[Route("api/users")]
[Authorize(AuthenticationSchemes = "Defaut")]
public class UserController(IUserService userService, IAddressService addressService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserReqDto req)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400, Helper.GetValidationErrors(ModelState));
            }
            var result = await userService.CreateUser(req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
            {
                logger.LogError("Error at create manager user: {ex}", result.InternalErrorMessage);
            }
            return Helper.GetErrorResponse(result.Message);

        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at create manager user: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.ServerError });
        }
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetUserProfile()
    {
        try
        {
            var result = await userService.GetUserProfileAsync();

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
            {
                logger.LogError("Error at get user profile: {ex}", result.InternalErrorMessage);
            }
            return Helper.GetErrorResponse(result.Message);

        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get user profile: {e}", ex);
            return StatusCode(500, new Return<IEnumerable<User>> { Message = ErrorMessage.ServerError });
        }
    }

    [HttpGet("{id:guid}/profile")]
    [Authorize]
    public async Task<IActionResult> GetUserProfileByManager([FromRoute] Guid id)
    {
        try
        {
            var result = await userService.GetUserProfileByManagerAsync(id);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
            {
                logger.LogError("Error at get user profile by manager: {ex}", result.InternalErrorMessage);
            }
            return Helper.GetErrorResponse(result.Message);

        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get user profile by manager: {e}", ex);
            return StatusCode(500, new Return<dynamic> { Message = ErrorMessage.ServerError });
        }
    }

    [HttpGet("addresses")]
    [Authorize]
    public async Task<IActionResult> GetUserAddresses([FromQuery] int pageSize, [FromQuery] int pageNumber)
    {
        try
        {
            var result = await addressService.GetUserAddressesAsync(pageSize, pageNumber);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
            {
                logger.LogError("Error at get user addresses: {ex}", result.InternalErrorMessage);
            }
            return Helper.GetErrorResponse(result.Message);

        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get user addresses: {e}", ex);
            return StatusCode(500, new Return<dynamic> { Message = ErrorMessage.ServerError });
        }
    }

    [HttpPost("addresses")]
    [Authorize]
    public async Task<IActionResult> AddAddress([FromBody] AddressReqDto req)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400, Helper.GetValidationErrors(ModelState));
            }

            var result = await addressService.AddAddressAsync(req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
            {
                logger.LogError("Error at add address: {ex}", result.InternalErrorMessage);
            }
            return Helper.GetErrorResponse(result.Message);

        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at add address: {e}", ex);
            return StatusCode(500, new Return<dynamic> { Message = ErrorMessage.ServerError });
        }
    }

    [HttpPut("addresses/{addressId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateAddress([FromRoute] Guid addressId, [FromBody] AddressReqDto req)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400, Helper.GetValidationErrors(ModelState));
            }

            var result = await addressService.UpdateAddressAsync(addressId, req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
            {
                logger.LogError("Error at update address: {ex}", result.InternalErrorMessage);
            }
            return Helper.GetErrorResponse(result.Message);

        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update address: {e}", ex);
            return StatusCode(500, new Return<dynamic> { Message = ErrorMessage.ServerError });
        }
    }

    [HttpDelete("addresses/{addressId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteAddress([FromRoute] Guid addressId)
    {
        try
        {
            var result = await addressService.DeleteAddressAsync(addressId);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
            {
                logger.LogError("Error at delete address: {ex}", result.InternalErrorMessage);
            }
            return Helper.GetErrorResponse(result.Message);

        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at delete address: {e}", ex);
            return StatusCode(500, new Return<dynamic> { Message = ErrorMessage.ServerError });
        }
    }

    [HttpPut("addresses/{addressId:guid}/default")]
    [Authorize]
    public async Task<IActionResult> SetDefaultAddress([FromRoute] Guid addressId)
    {
        try
        {
            var result = await addressService.SetDefaultAddressAsync(addressId);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
            {
                logger.LogError("Error at set default address: {ex}", result.InternalErrorMessage);
            }
            return Helper.GetErrorResponse(result.Message);

        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at set default address: {e}", ex);
            return StatusCode(500, new Return<dynamic> { Message = ErrorMessage.ServerError });
        }
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileReqDto req)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400, Helper.GetValidationErrors(ModelState));
            }
            var result = await userService.UpdateProfile(req);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
            {
                logger.LogError("Error at update user profile: {ex}", result.InternalErrorMessage);
            }
            return Helper.GetErrorResponse(result.Message);

        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at update user profile: {e}", ex);
            return StatusCode(500, new Return<bool> { Message = ErrorMessage.ServerError });
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllUsers([FromQuery] string? status, [FromQuery] int? pageSize, [FromQuery] int? pageNumber)
    {
        try
        {
            var result = await userService.GetAllUsersAsync(status, pageSize, pageNumber);

            if (result.IsSuccess) return StatusCode(200, result);
            if (result.InternalErrorMessage is not null)
            {
                logger.LogError("Error at get all users: {ex}", result.InternalErrorMessage);
            }
            return Helper.GetErrorResponse(result.Message);

        }
        catch (Exception ex)
        {
            logger.LogInformation("Error at get all users: {e}", ex);
            return StatusCode(500, new Return<IEnumerable<User>> { Message = ErrorMessage.ServerError });
        }
    }
}
