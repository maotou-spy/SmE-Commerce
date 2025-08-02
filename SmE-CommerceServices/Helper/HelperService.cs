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
using ErrorCode = SmE_CommerceModels.Enums.ErrorCode;

namespace SmE_CommerceServices.Helper;

public class HelperService(
    IConfiguration configuration,
    IHttpContextAccessor httpContextAccessor,
    IUserRepository userRepository
) : IHelperService
{
    public async Task<Return<User>> GetCurrentUser()
    {
        var token = httpContextAccessor
            .HttpContext.Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token) || !VerifyToken(token))
            return new Return<User> { IsSuccess = false, StatusCode = ErrorCode.NotAuthority };

        var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Sid)?.Value;
        if (userId == null)
            return new Return<User> { IsSuccess = false, StatusCode = ErrorCode.NotAuthority };

        var user = await userRepository.GetUserByIdAsync(Guid.Parse(userId));
        if (user.Data == null)
            return new Return<User> { IsSuccess = false, StatusCode = ErrorCode.NotAuthority };

        if (user.Data.Status == UserStatus.Inactive)
            return new Return<User> { IsSuccess = false, StatusCode = ErrorCode.AccountIsInactive };

        return new Return<User>
        {
            IsSuccess = true,
            Data = user.Data,
            StatusCode = ErrorCode.Ok,
        };
    }

    public async Task<Return<User>> GetCurrentUserWithRoleAsync(string role)
    {
        var user = await GetCurrentUser();

        if (!user.IsSuccess || user.Data == null)
            return user;

        // Improved role check
        var isAuthorized = role switch
        {
            nameof(RoleEnum.Customer) => user.Data.Role == RoleEnum.Customer,
            nameof(RoleEnum.Administrator) => user.Data.Role == RoleEnum.Administrator,
            nameof(RoleEnum.Staff) => user.Data.Role is RoleEnum.Staff or RoleEnum.Manager,
            nameof(RoleEnum.Manager) => user.Data.Role == RoleEnum.Manager,
            _ => false,
        };

        if (!isAuthorized)
            return new Return<User> { IsSuccess = false, StatusCode = ErrorCode.NotAuthority };

        return new Return<User>
        {
            IsSuccess = true,
            Data = user.Data,
            StatusCode = ErrorCode.Ok,
        };
    }

    private bool VerifyToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(
            configuration.GetSection("AppSettings:Token").Value
                ?? throw new Exception("SERVER_ERROR: Token key is missing")
        );

        tokenHandler.ValidateToken(
            token,
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
            },
            out var validatedToken
        );

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
