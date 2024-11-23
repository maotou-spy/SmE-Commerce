using System.Transactions;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.User;
using SmE_CommerceModels.ResponseDtos.User;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;
using SmE_CommerceUtilities;

namespace SmE_CommerceServices;

public class UserService(IUserRepository userRepository, IHelperService helperService) : IUserService
{
    public async Task<Return<IEnumerable<User>>> GetAllUsersAsync(string? status, int? pageSize, int? pageNumber, string? phone, string? email, string? name)
    {
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(nameof(RoleEnum.Manager));
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
                Message = SuccessMessage.Created,
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
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(nameof(RoleEnum.Manager));
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
                Message = SuccessMessage.Created,
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
                Point = currentUser.Data.Point,
                Dob = currentUser.Data.DateOfBirth,
                Gender = currentUser.Data.Gender,
                Avatar = currentUser.Data.Avatar,
            };

            return new Return<GetUserProfileResDto>
            {
                IsSuccess = true,
                Data = user,
                Message = SuccessMessage.Successfully
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

    public async Task<Return<GetUserProfileResDto>> GetUserProfileByManagerAsync(Guid id)
    {
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<GetUserProfileResDto>
                {
                    IsSuccess = false,
                    Message = currentUser.Message,
                    InternalErrorMessage = currentUser.InternalErrorMessage
                };
            }

            var user = await userRepository.GetUserByIdAsync(id);
            if (!user.IsSuccess)
            {
                return new Return<GetUserProfileResDto>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.NotFoundUser,
                };
            }

            var userProfileDto = new GetUserProfileResDto
            {
                Email = user.Data?.Email,
                FullName = user.Data?.FullName,
                Phone = user.Data?.Phone,
                Point = user.Data?.Point,
                Dob = user.Data?.DateOfBirth,
                Gender = user.Data?.Gender,
                Avatar = user.Data?.Avatar,
            };

            return new Return<GetUserProfileResDto>
            {
                IsSuccess = true,
                Data = userProfileDto,
                Message = SuccessMessage.Successfully,
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
                    Message = ErrorMessage.NotFoundUser,
                    InternalErrorMessage = user.InternalErrorMessage
                };
            }

            var existedPhone = await userRepository.GetUserByPhoneAsync(req.Phone);
            if (existedPhone is { IsSuccess: true, Data: not null })
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.UserAlreadyExists,
                };
            }

            if(req.Dob > DateOnly.FromDateTime(DateTime.Now))
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InvalidInput,
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
            user.Data.DateOfBirth = req.Dob;
            user.Data.Gender = req.Gender;
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
                Message = SuccessMessage.Updated,
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

    public async Task<Return<bool>> DeleteUserAsync(Guid id)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = currentUser.Message,
                    InternalErrorMessage = currentUser.InternalErrorMessage
                };
            }

            var user = await userRepository.GetUserByIdAsync(id);
            if (!user.IsSuccess || user.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.NotFoundUser,
                    InternalErrorMessage = user.InternalErrorMessage
                };
            }

            if (user.Data.Role.Equals(RoleEnum.Manager))
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.ManagerCannotBeBanned,
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
                    InternalErrorMessage = updateResult.InternalErrorMessage
                };
            }

            scope.Complete();

            return new Return<bool>
            {
                IsSuccess = true,
                Message = SuccessMessage.Updated,
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<bool>> ChangeUserStatusAsync(Guid id)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = currentUser.Message,
                    InternalErrorMessage = currentUser.InternalErrorMessage
                };
            }

            var user = await userRepository.GetUserByIdAsync(id);
            if (!user.IsSuccess || user.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.NotFoundUser,
                    InternalErrorMessage = user.InternalErrorMessage
                };
            }

            if (user.Data.Role.Equals(RoleEnum.Manager))
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.ManagerCannotBeBanned,
                    InternalErrorMessage = user.InternalErrorMessage
                };
            }

            user.Data.Status = user.Data.Status == UserStatus.Active ? UserStatus.Inactive : UserStatus.Active;
            user.Data.ModifiedAt = DateTime.Now;
            user.Data.ModifiedById = currentUser.Data.UserId;

            var updateResult = await userRepository.UpdateUser(user.Data);
            if (!updateResult.IsSuccess || updateResult.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = updateResult.InternalErrorMessage
                };
            }

            scope.Complete();

            return new Return<bool>
            {
                IsSuccess = true,
                Message = SuccessMessage.Updated,
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }
}
