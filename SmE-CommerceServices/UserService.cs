using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.User;
using SmE_CommerceModels.ResponseDtos.User;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;
using SmE_CommerceUtilities;
using System.Transactions;
using static System.Formats.Asn1.AsnWriter;

namespace SmE_CommerceServices;

public class UserService(IUserRepository userRepository, IHelperService helperService) : IUserService
{
    public async Task<Return<IEnumerable<User>>> GetAllUsersAsync(string? status, int? pageSize, int? pageNumber, string? phone, string? email, string? name)
    {
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUserWithRole(nameof(RoleEnum.Manager));
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<IEnumerable<User>>
                {
                    IsSuccess = false,
                    Message = currentUser.Message,
                };
            }

            var users = await userRepository.GetAllUsersAsync(status, pageSize, pageNumber, phone, email, name);
            if (!users.IsSuccess || users.Data == null)
            {
                return new Return<IEnumerable<User>>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = users.InternalErrorMessage,
                };
            }

            return new Return<IEnumerable<User>>
            {
                IsSuccess = true,
                Message = SuccessfulMessage.Created,
                Data = users.Data,
                TotalRecord = users.TotalRecord
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<User>>
            {
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<bool>> CreateUser(CreateUserReqDto req)
    {
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUserWithRole(nameof(RoleEnum.Manager));
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = currentUser.Message,
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
                Email = req.Email.ToLower(),
                PasswordHash = HashUtil.Hash(req.Password),
                FullName = req.FullName,
                Role = req.Role,
                Status = UserStatus.Active,
                CreateById = currentUser.Data.UserId,
                CreatedAt = DateTime.Now
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

    public async Task<Return<GetUserProfileResDto>> GetUserProfileAsync()
    {
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUser();
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<GetUserProfileResDto>
                {
                    IsSuccess = false,
                    Message = currentUser.Message,
                    InternalErrorMessage = currentUser.InternalErrorMessage
                };
            }

            var user = new GetUserProfileResDto
            {
                Email = currentUser.Data.Email,
                FullName = currentUser.Data.FullName,
                Phone = currentUser.Data.Phone,
                Point = currentUser.Data.Point
            };

            return new Return<GetUserProfileResDto>
            {
                IsSuccess = true,
                Data = user,
                Message = SuccessfulMessage.Successfully
            };
        }
        catch (Exception ex)
        {
            return new Return<GetUserProfileResDto>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<GetUserProfileResDto>> GetUserProfileByManagerAsync(Guid Id)
    {
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUserWithRole(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<GetUserProfileResDto>
                {
                    IsSuccess = false,
                    Message = currentUser.Message,
                    InternalErrorMessage = currentUser.InternalErrorMessage
                };
            }

            var user = await userRepository.GetUserByIdAsync(Id);
            if (!user.IsSuccess || user == null) 
            {
                return new Return<GetUserProfileResDto>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.NotFound,
                };
            }

            var userProfileDto = new GetUserProfileResDto
            {
                Email = user.Data.Email,
                FullName = user.Data.FullName,
                Phone = user.Data.Phone,
                Point = user.Data.Point
            };

            return new Return<GetUserProfileResDto>
            {
                IsSuccess = true,
                Data = userProfileDto,
                Message = SuccessfulMessage.Successfully,
                TotalRecord = user.TotalRecord
            };
        }
        catch (Exception ex)
        {
            return new Return<GetUserProfileResDto>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<bool>> UpdateProfileAsync(UpdateUserProfileReqDto req)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUser();
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = currentUser.Message,
                    InternalErrorMessage = currentUser.InternalErrorMessage
                };
            }

            var user = await userRepository.GetUserByIdAsync(currentUser.Data.UserId);
            if (!user.IsSuccess || user.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.NotFound,
                    InternalErrorMessage = user.InternalErrorMessage
                };
            }

            var existedPhone = await userRepository.GetUserByEmailOrPhoneAsync(req.Phone);
            if (existedPhone.IsSuccess && existedPhone.Data != null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.UserAlreadyExists,
                };
            }

            if(req.Phone.Equals(user.Data.Phone) && req.FullName.Equals(user.Data.FullName))
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.NoChanges,
                };
            }

            user.Data.FullName = req.FullName;
            user.Data.Phone = req.Phone;
            user.Data.CreatedAt = DateTime.Now;
            user.Data.CreateById = currentUser.Data.UserId;

            var updateResult = await userRepository.UpdateUser(user.Data);
            if (!updateResult.IsSuccess || updateResult.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = updateResult.InternalErrorMessage,
                };
            }

            scope.Complete();

            return new Return<bool>
            {
                IsSuccess = true,
                Message = SuccessfulMessage.Updated,
                Data = true
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

    public async Task<Return<bool>> DeleteUserAsync(Guid Id)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUserWithRole(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = currentUser.Message,
                    InternalErrorMessage = currentUser.InternalErrorMessage
                };
            }

            var user = await userRepository.GetUserByIdAsync(Id);
            if (!user.IsSuccess || user.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.NotFound,
                    InternalErrorMessage = user.InternalErrorMessage
                };
            }

            if (user.Data.Role.Equals(RoleEnum.Manager))
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.MANAGERCANNOTDELETED,
                    InternalErrorMessage = user.InternalErrorMessage
                };
            }

            user.Data.Status = UserStatus.Deleted;
            user.Data.ModifiedAt = DateTime.Now;
            user.Data.ModifiedById = currentUser.Data.UserId;

            var updateResult = await userRepository.UpdateUser(user.Data);
            if (!updateResult.IsSuccess || updateResult.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = updateResult.InternalErrorMessage,
                };
            }

            scope.Complete();

            return new Return<bool>
            {
                IsSuccess = true,
                Message = SuccessfulMessage.Deleted,
                Data = true
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
 