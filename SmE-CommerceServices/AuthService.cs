using System;
using System.Security.Cryptography;
using System.Text;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Auth;
using SmE_CommerceModels.ResponseDtos;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceUtilities;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly BearerTokenUtil _bearerTokenUtil;
    private const int Iterations = 10000;

    public AuthService(IUserRepository userRepository, BearerTokenUtil bearerTokenUtil)
    {
        _userRepository = userRepository;
        _bearerTokenUtil = bearerTokenUtil;
    }

    public async Task<Return<LoginResDto>> LoginWithAccount(LoginWithAccountReqDto reqDto)
    {
        var userResult = await _userRepository.GetUserByEmailOrPhoneAsync(reqDto.EmailOrPhone);

        if (!userResult.IsSuccess || userResult.Data == null)
        {
            return new Return<LoginResDto>
            {
                IsSuccess = false,
                Message = ErrorMessage.NotFound
            };
        }

        var user = userResult.Data;

        if (user.Status == UserStatus.Inactive)
        {
            return new Return<LoginResDto>
            {
                IsSuccess = false,
                Message = ErrorMessage.ACCOUNT_IS_INACTIVE
            };
        }

        if (!VerifyPassword(reqDto.Password, user.PasswordHash))
        {
            return new Return<LoginResDto>
            {
                IsSuccess = false,
                Message = ErrorMessage.INVALID_CREDENTIALS
            };
        }

        var token = _bearerTokenUtil.GenerateBearerToken(user.UserId, user.Role);

        return new Return<LoginResDto>
        {
            Data = new LoginResDto
            {
                BearerToken = token,
                Email = user.Email,
                Phone = user.Phone,
                Name = user.FullName
            },
            IsSuccess = true,
            Message = SuccessfulMessage.Successfully
        };
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        byte[] hashBytes = Convert.FromBase64String(hashedPassword);

        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations))
        {
            byte[] hash = deriveBytes.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }
            return true;
        }
    }

    public string HashPassword(string password)
    {
        byte[] salt;
        byte[] hash;

        using (var deriveBytes = new Rfc2898DeriveBytes(password, 16, Iterations))
        {
            salt = deriveBytes.Salt;
            hash = deriveBytes.GetBytes(20);  // 20 bytes for SHA1
        }

        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        return Convert.ToBase64String(hashBytes);
    }
}