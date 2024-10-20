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

    public async Task<Return<bool>> CreateManagerUser(CreateManagerReqDto req)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUser(nameof(RoleEnum.Manager));
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = currentUser.Message,
                };
            }

            // Check email is already existed
            var existedManager = await userRepository.GetUserByEmailAsync(req.Email);
            if (!existedManager.Message.Equals(ErrorMessage.NotFound))
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.UserAlreadyExists,
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
                CreateById = currentUser.Data.UserId,
                CreatedAt = DateTime.Now
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
