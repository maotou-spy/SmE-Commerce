using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.User;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;
using System.Security.Cryptography;
using SmE_CommerceUtilities;

namespace SmE_CommerceServices;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<Return<IEnumerable<User>>> GetAllUsersAsync()
    {
        return await userRepository.GetAllUsersAsync();
    }

    public async Task<Return<bool>> CreateManagerUser(CreateManagerReqDto req)
    {
        try
        {
            // Check email is already existed
            var existedManager = await userRepository.GetUserByEmailAsync(req.Email);
            if (!existedManager.Message.Equals(ErrorMessage.NotFound))
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.USER_ALREADY_EXISTS,
                    InternalErrorMessage = existedManager.InternalErrorMessage,
                };
            }

            User newManager = new()
            {
                Email = req.Email,
                PasswordHash = HashUtil.Hash(req.Password),
                FullName = req.FullName,
                Role = RoleEnum.Manager,
                Status = UserStatus.Active,
                //CreateById = Guid.Empty,
                //CreateAt = DateTime.UtcNow,
            };

            var roleManager = await userRepository.CreateNewUser(newManager);
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
}
