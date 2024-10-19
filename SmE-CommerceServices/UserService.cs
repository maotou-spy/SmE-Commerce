using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.User;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;
using System.Security.Cryptography;

namespace SmE_CommerceServices;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Return<IEnumerable<User>>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    public async Task<Return<bool>> CreateManagerUser(CreateManagerReqDto req)
    {
        try
        {
            // Check email is already existed
            var existedManager = await _userRepository.GetUserByEmailAsync(req.Email);
            if (!existedManager.Message.Equals(ErrorMessage.NotFound))
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.USER_ALREADY_EXISTS,
                    InternalErrorMessage = existedManager.InternalErrorMessage,
                };
            }

            // Create password hash
            string passwordHash = CreatePasswordHash(req.Password);

            User newManager = new()
            {
                Email = req.Email,
                PasswordHash = passwordHash,
                FullName = req.FullName,
                Role = RoleEnum.Manager,
                Status = UserStatus.Active,
                //CreateById = Guid.Empty,
                //CreateAt = DateTime.UtcNow,
            };

            var roleManager = await _userRepository.CreateNewUser(newManager);
            return new Return<bool>
            {
                IsSuccess = true,
                Message = SuccessfulMessage.Created,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    #region Private Methods
    // Verify Password Hash
    private static bool VerifyPassword(string password, string hashedPassword)
    {
        byte[] hashBytes = Convert.FromBase64String(hashedPassword);
        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
        byte[] hash = pbkdf2.GetBytes(20);
        for (int i = 0; i < 20; i++)
        {
            if (hashBytes[i + 16] != hash[i])
                return false;
        }
        return true;
    }

    // Create Password Hash using PBKDF2
    private static string CreatePasswordHash(string password)
    {
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
        byte[] hash = pbkdf2.GetBytes(20);
        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);
        return Convert.ToBase64String(hashBytes);
    }
    #endregion
}
