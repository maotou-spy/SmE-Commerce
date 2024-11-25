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
    public async Task<Return<IEnumerable<User>>> GetAllUsersAsync(string? status, int? pageSize, int? pageNumber,
        string? phone, string? email, string? name)
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
                    ErrorCode = currentUser.ErrorCode
                };
            }

            var users = await userRepository.GetAllUsersAsync(status, pageSize, pageNumber, phone, email, name);
            if (!users.IsSuccess || users.Data == null)
            {
                return new Return<IEnumerable<User>>
                {
                    IsSuccess = false,
                    Message = users.Message,
                    ErrorCode = users.ErrorCode
                };
            }

            return new Return<IEnumerable<User>>
            {
                IsSuccess = true,
                Message = users.Message,
                Data = users.Data,
                TotalRecord = users.TotalRecord,
                ErrorCode = users.ErrorCode
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<User>>
            {
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                ErrorCode = ErrorCodes.InternalServerError
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
                    ErrorCode = currentUser.ErrorCode
                };
            }

            // Check if email already exists
            var existedManager = await userRepository.GetUserByEmailAsync(req.Email);
            if (existedManager is { IsSuccess: true, Data: not null })
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.EmailAlreadyExists,
                    ErrorCode = ErrorCodes.EmailAlreadyExists
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
                    Message = createResult.Message,
                    InternalErrorMessage = createResult.InternalErrorMessage,
                    ErrorCode = createResult.ErrorCode
                };
            }

            return new Return<bool>
            {
                IsSuccess = true,
                Message = createResult.Message,
                Data = true,
                ErrorCode = createResult.ErrorCode
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
                ErrorCode = ErrorCodes.InternalServerError
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
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                    ErrorCode = currentUser.ErrorCode
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
                Avatar = currentUser.Data.Avatar
            };

            return new Return<GetUserProfileResDto>
            {
                IsSuccess = true,
                Data = user,
                Message = SuccessMessage.Successfully,
                ErrorCode = ErrorCodes.Ok
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
                TotalRecord = 0,
                ErrorCode = ErrorCodes.InternalServerError
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
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                    ErrorCode = currentUser.ErrorCode
                };
            }

            var user = await userRepository.GetUserByIdAsync(id);
            if (!user.IsSuccess)
            {
                return new Return<GetUserProfileResDto>
                {
                    IsSuccess = false,
                    Message = user.Message,
                    InternalErrorMessage = user.InternalErrorMessage,
                    ErrorCode = user.ErrorCode
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
                Avatar = user.Data?.Avatar
            };

            return new Return<GetUserProfileResDto>
            {
                IsSuccess = true,
                Data = userProfileDto,
                Message = SuccessMessage.Successfully,
                TotalRecord = user.TotalRecord,
                ErrorCode = ErrorCodes.Ok
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
                TotalRecord = 0,
                ErrorCode = ErrorCodes.InternalServerError
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
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                    ErrorCode = currentUser.ErrorCode
                };
            }

            var user = await userRepository.GetUserByIdAsync(currentUser.Data.UserId);
            if (!user.IsSuccess || user.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = user.Message,
                    InternalErrorMessage = user.InternalErrorMessage,
                    ErrorCode = user.ErrorCode
                };
            }

            var existedPhone = await userRepository.GetUserByPhoneAsync(req.Phone);
            if (existedPhone is { IsSuccess: true, Data: not null })
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.PhoneAlreadyExists,
                    ErrorCode = ErrorCodes.PhoneAlreadyExists
                };
            }

            if (req.Dob > DateOnly.FromDateTime(DateTime.Now))
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InvalidDate,
                    ErrorCode = ErrorCodes.InvalidDate
                };
            }

            if (req.Phone.Equals(user.Data.Phone) && req.FullName.Equals(user.Data.FullName) &&
                req.Dob.Equals(user.Data.DateOfBirth) && req.Gender.Equals(user.Data.Gender))
            {
                return new Return<bool>
                {
                    IsSuccess = true,
                    Message = SuccessMessage.Successfully,
                    Data = true,
                    ErrorCode = ErrorCodes.Ok
                };
            }

            user.Data.FullName = req.FullName;
            user.Data.Phone = req.Phone;
            user.Data.DateOfBirth = req.Dob;
            user.Data.Gender = req.Gender;
            user.Data.ModifiedAt = DateTime.Now;
            user.Data.ModifiedById = currentUser.Data.UserId;

            var updateResult = await userRepository.UpdateUser(user.Data);
            if (!updateResult.IsSuccess || updateResult.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = updateResult.Message,
                    InternalErrorMessage = updateResult.InternalErrorMessage,
                    ErrorCode = updateResult.ErrorCode
                };
            }

            scope.Complete();

            return new Return<bool>
            {
                IsSuccess = true,
                Message = SuccessMessage.Updated,
                Data = true,
                ErrorCode = ErrorCodes.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                ErrorCode = ErrorCodes.InternalServerError
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
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                    ErrorCode = currentUser.ErrorCode
                };
            }

            var user = await userRepository.GetUserByIdAsync(id);
            if (!user.IsSuccess || user.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.UserNotFound,
                    InternalErrorMessage = user.InternalErrorMessage,
                    ErrorCode = user.ErrorCode
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
                    Message = updateResult.Message,
                    InternalErrorMessage = updateResult.InternalErrorMessage,
                    ErrorCode = updateResult.ErrorCode
                };
            }

            scope.Complete();

            return new Return<bool>
            {
                IsSuccess = true,
                Message = SuccessMessage.Updated,
                Data = true,
                ErrorCode = ErrorCodes.Ok
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
                TotalRecord = 0,
                ErrorCode = ErrorCodes.InternalServerError
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
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                    ErrorCode = currentUser.ErrorCode
                };
            }

            var user = await userRepository.GetUserByIdAsync(id);
            if (!user.IsSuccess || user.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.UserNotFound,
                    InternalErrorMessage = user.InternalErrorMessage,
                    ErrorCode = user.ErrorCode
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
                    InternalErrorMessage = updateResult.InternalErrorMessage,
                    ErrorCode = updateResult.ErrorCode
                };
            }

            scope.Complete();

            return new Return<bool>
            {
                IsSuccess = true,
                Message = SuccessMessage.Updated,
                Data = true,
                ErrorCode = ErrorCodes.Ok
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
                TotalRecord = 0,
                ErrorCode = ErrorCodes.InternalServerError
            };
        }
    }
}