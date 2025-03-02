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

public class UserService(IUserRepository userRepository, IHelperService helperService)
    : IUserService
{
    public async Task<Return<IEnumerable<User>>> GetAllUsersAsync(
        string? status,
        int? pageSize,
        int? pageNumber,
        string? phone,
        string? email,
        string? name
    )
    {
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(
                nameof(RoleEnum.Manager)
            );
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<IEnumerable<User>>
                {
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                };

            var users = await userRepository.GetAllUsersAsync(
                status,
                pageSize,
                pageNumber,
                phone,
                email,
                name
            );
            if (!users.IsSuccess || users.Data == null)
                return new Return<IEnumerable<User>>
                {
                    IsSuccess = false,
                    StatusCode = users.StatusCode,
                };

            return new Return<IEnumerable<User>>
            {
                IsSuccess = true,
                StatusCode = users.StatusCode,
                Data = users.Data,
                TotalRecord = users.TotalRecord,
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<User>>
            {
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<bool>> CreateUser(CreateUserReqDto req)
    {
        try
        {
            // Validate user
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(
                nameof(RoleEnum.Manager)
            );
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool> { IsSuccess = false, StatusCode = currentUser.StatusCode };

            // Check if email already exists
            var existedManager = await userRepository.GetUserByEmailAsync(req.Email);
            if (existedManager is { IsSuccess: true, Data: not null })
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.UserAlreadyExists,
                };

            User newManager = new()
            {
                Email = req.Email.ToLower(),
                PasswordHash = HashUtil.Hash(req.Password),
                FullName = req.FullName,
                Role = req.Role,
                Status = UserStatus.Active,
                CreateById = currentUser.Data.UserId,
                CreatedAt = DateTime.Now,
            };

            var createResult = await userRepository.CreateNewUser(newManager);
            if (!createResult.IsSuccess || createResult.Data == null)
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = createResult.StatusCode,
                    InternalErrorMessage = createResult.InternalErrorMessage,
                };

            return new Return<bool>
            {
                IsSuccess = true,
                StatusCode = createResult.StatusCode,
                Data = true,
            };
        }
        catch (Exception ex)
        {
            // Log the exception here
            return new Return<bool>
            {
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
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
                return new Return<GetUserProfileResDto>
                {
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

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
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<GetUserProfileResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
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
                return new Return<GetUserProfileResDto>
                {
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var user = await userRepository.GetUserByIdAsync(id);
            if (!user.IsSuccess)
                return new Return<GetUserProfileResDto>
                {
                    IsSuccess = false,
                    InternalErrorMessage = user.InternalErrorMessage,
                    StatusCode = user.StatusCode,
                };

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
                StatusCode = ErrorCode.Ok,
                TotalRecord = user.TotalRecord,
            };
        }
        catch (Exception ex)
        {
            return new Return<GetUserProfileResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
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
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var user = await userRepository.GetUserByIdAsync(currentUser.Data.UserId);
            if (!user.IsSuccess || user.Data == null)
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = user.InternalErrorMessage,
                    StatusCode = user.StatusCode,
                };

            var existedPhone = await userRepository.GetUserByPhoneAsync(req.Phone);
            if (existedPhone is { IsSuccess: true, Data: not null })
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.PhoneAlreadyExists,
                };

            if (req.Dob > DateOnly.FromDateTime(DateTime.Now))
                return new Return<bool> { IsSuccess = false, StatusCode = ErrorCode.BadRequest };

            if (
                req.Phone.Equals(user.Data.Phone)
                && req.FullName.Equals(user.Data.FullName)
                && req.Dob.Equals(user.Data.DateOfBirth)
                && req.Gender.Equals(user.Data.Gender)
            )
                return new Return<bool>
                {
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                    Data = true,
                };

            user.Data.FullName = req.FullName;
            user.Data.Phone = req.Phone;
            user.Data.DateOfBirth = req.Dob;
            user.Data.Gender = req.Gender;
            user.Data.ModifiedAt = DateTime.Now;
            user.Data.ModifiedById = currentUser.Data.UserId;

            var updateResult = await userRepository.UpdateUserAsync(user.Data);
            if (!updateResult.IsSuccess || updateResult.Data == null)
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = updateResult.InternalErrorMessage,
                    StatusCode = updateResult.StatusCode,
                };

            scope.Complete();

            return new Return<bool>
            {
                IsSuccess = true,
                Data = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                InternalErrorMessage = ex,
                StatusCode = ErrorCode.InternalServerError,
            };
        }
    }

    public async Task<Return<bool>> ChangePassword(ChangePasswordReqDto req)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUser();
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            if (
                !HashUtil.VerifyHashedString(
                    req.OldPassword,
                    currentUser.Data.PasswordHash ?? string.Empty
                )
            )
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidPassword,
                };

            currentUser.Data.PasswordHash = HashUtil.Hash(req.NewPassword);
            currentUser.Data.ModifiedAt = DateTime.Now;
            currentUser.Data.ModifiedById = currentUser.Data.UserId;

            var updateResult = await userRepository.UpdateUserAsync(currentUser.Data);
            if (!updateResult.IsSuccess || updateResult.Data == null)
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = updateResult.InternalErrorMessage,
                    StatusCode = updateResult.StatusCode,
                };

            return new Return<bool>
            {
                IsSuccess = true,
                Data = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                InternalErrorMessage = ex,
                StatusCode = ErrorCode.InternalServerError,
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
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                    StatusCode = currentUser.StatusCode,
                };

            var user = await userRepository.GetUserByIdAsync(id);
            if (!user.IsSuccess || user.Data == null)
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = user.InternalErrorMessage,
                    StatusCode = user.StatusCode,
                };

            user.Data.Status = UserStatus.Deleted;
            user.Data.ModifiedAt = DateTime.Now;
            user.Data.ModifiedById = currentUser.Data.UserId;

            var updateResult = await userRepository.UpdateUserAsync(user.Data);
            if (!updateResult.IsSuccess || updateResult.Data == null)
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = updateResult.InternalErrorMessage,
                    StatusCode = updateResult.StatusCode,
                };

            scope.Complete();

            return new Return<bool>
            {
                IsSuccess = true,
                Data = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                InternalErrorMessage = ex,
                TotalRecord = 0,
                StatusCode = ErrorCode.InternalServerError,
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
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                    StatusCode = currentUser.StatusCode,
                };

            var user = await userRepository.GetUserByIdAsync(id);
            if (!user.IsSuccess || user.Data == null)
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = user.InternalErrorMessage,
                    StatusCode = user.StatusCode,
                };

            user.Data.Status =
                user.Data.Status == UserStatus.Active ? UserStatus.Inactive : UserStatus.Active;
            user.Data.ModifiedAt = DateTime.Now;
            user.Data.ModifiedById = currentUser.Data.UserId;

            var updateResult = await userRepository.UpdateUserAsync(user.Data);
            if (!updateResult.IsSuccess || updateResult.Data == null)
                return new Return<bool>
                {
                    IsSuccess = false,
                    InternalErrorMessage = updateResult.InternalErrorMessage,
                    StatusCode = updateResult.StatusCode,
                };

            scope.Complete();

            return new Return<bool>
            {
                IsSuccess = true,
                Data = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                InternalErrorMessage = ex,
                TotalRecord = 0,
                StatusCode = ErrorCode.InternalServerError,
            };
        }
    }

    public async Task<Return<IEnumerable<UserGetTheirDiscountResDto>>> UserGetTheirDiscountsAsync()
    {
        try
        {
            var user = await helperService.GetCurrentUser();
            if (!user.IsSuccess || user.Data == null)
                return new Return<IEnumerable<UserGetTheirDiscountResDto>>
                {
                    IsSuccess = false,
                    StatusCode = user.StatusCode,
                };

            var result = await userRepository.UserGetDiscountsByUserIdAsync(user.Data.UserId);
            if (!result.IsSuccess)
                return new Return<IEnumerable<UserGetTheirDiscountResDto>>
                {
                    IsSuccess = false,
                    InternalErrorMessage = result.InternalErrorMessage,
                    StatusCode = result.StatusCode,
                };

            var res = result
                .Data!.Select(x => new UserGetTheirDiscountResDto
                {
                    CodeId = x.CodeId,
                    DiscountName = x.Discount.DiscountName,
                })
                .ToList();

            return new Return<IEnumerable<UserGetTheirDiscountResDto>>
            {
                Data = res,
                IsSuccess = true,
                TotalRecord = res.Count,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception e)
        {
            return new Return<IEnumerable<UserGetTheirDiscountResDto>>
            {
                Data = null,
                IsSuccess = false,
                InternalErrorMessage = e,
                StatusCode = ErrorCode.InternalServerError,
            };
        }
    }
}
