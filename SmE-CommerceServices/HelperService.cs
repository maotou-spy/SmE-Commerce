using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class HelperService(
    IConfiguration configuration,
    IHttpContextAccessor httpContextAccessor,
    IUserRepository userRepository) : IHelperService
{
    public async Task<Return<User>> GetCurrentUser()
    {
        var token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token) || !VerifyToken(token))
        {
            return new Return<User>
            {
                IsSuccess = false,
                Message = ErrorMessage.NotAuthority
            };
        }

        var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value;
        if (userId == null)
        {
            return new Return<User>
            {
                IsSuccess = false,
                Message = ErrorMessage.NotAuthority
            };
        }

        var user = await userRepository.GetUserByIdAsync(Guid.Parse(userId));
        if (user.Data == null)
        {
            return new Return<User>
            {
                IsSuccess = false,
                Message = ErrorMessage.NotAuthority
            };
        }

        if (user.Data.Status == UserStatus.Inactive)
        {
            return new Return<User>
            {
                IsSuccess = false,
                Message = ErrorMessage.AccountIsInactive
            };
        }

        return new Return<User>
        {
            IsSuccess = true,
            Data = user.Data,
            Message = SuccessMessage.Successfully
        };
    }

    public async Task<Return<User>> GetCurrentUserWithRole(string role)
    {
        var user = await GetCurrentUser();

        // Improved role check
        var isAuthorized = role switch
        {
            nameof(RoleEnum.Customer) => user.Data.Role == RoleEnum.Customer,
            nameof(RoleEnum.Staff) => user.Data.Role is RoleEnum.Staff or RoleEnum.Manager,
            nameof(RoleEnum.Manager) => user.Data.Role == RoleEnum.Manager,
            _ => false
        };

        if (!isAuthorized)
        {
            return new Return<User>
            {
                IsSuccess = false,
                Message = ErrorMessage.NotAuthority
            };
        }

        return new Return<User>
        {
            IsSuccess = true,
            Data = user.Data,
            Message = SuccessMessage.Successfully
        };
    }

    private bool VerifyToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration.GetSection("AppSettings:Token").Value
                                          ?? throw new Exception("SERVER_ERROR: Token key is missing"));

        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        }, out var validatedToken);

        // Check if token is expired
        return validatedToken.ValidTo >= DateTime.Now;
    }

    // private string GetRoleFromToken(string token)
    // {
    //     var tokenHandler = new JwtSecurityTokenHandler();
    //     var jwtToken = tokenHandler.ReadJwtToken(token);
    //     var encryptedRole = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
    //
    //     if (encryptedRole == null)
    //     {
    //         throw new Exception("SERVER_ERROR: Role not found in token");
    //     }
    //
    //     return  Decrypt(encryptedRole);
    // }
}
