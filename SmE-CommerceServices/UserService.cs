using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.User;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;
using SmE_CommerceUtilities;

namespace SmE_CommerceServices;

public class UserService(IUserRepository userRepository, IHelperService helperService) : IUserService
{
    public async Task<Return<IEnumerable<User>>> GetAllUsersAsync()
    {
        return await userRepository.GetAllUsersAsync();
    }

    public async Task<Return<bool>> CreateUser(CreateUserReqDto req)
    {
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUser(nameof(RoleEnum.Manager));
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = currentUser.Message ?? ErrorMessage.NotAuthority,
                };
            }

            // Check if email already exists
            var existedManager = await userRepository.GetUserByEmailAsync(req.Email);
            if (existedManager is { IsSuccess: true, Data: not null })
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.UserAlreadyExists,
                };
            }

            User newManager = new()
            {
                Email = req.Email,
                PasswordHash = HashUtil.Hash(req.Password),
                FullName = req.FullName,
                Role = req.Role,
                Status = UserStatus.Active,
                CreateById = currentUser.Data.UserId,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await userRepository.CreateNewUser(newManager);
            if (!createResult.IsSuccess || createResult.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = createResult.InternalErrorMessage,
                };
            }

            return new Return<bool>
            {
                IsSuccess = true,
                Message = SuccessfulMessage.Created,
                Data = true
            };
        }
        catch (Exception ex)
        {
            // Log the exception here
            return new Return<bool>
            {
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }
}
