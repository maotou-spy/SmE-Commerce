using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Auth;
using SmE_CommerceModels.ResponseDtos.Auth;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Firebase;
using SmE_CommerceServices.Interface;
using SmE_CommerceUtilities;

namespace SmE_CommerceServices;

public class AuthService(
    IUserRepository userRepository,
    IFirebaseAuthService firebaseService,
    BearerTokenUtil bearerTokenUtil
) : IAuthService
{
    public async Task<Return<LoginResDto>> LoginWithAccount(LoginWithAccountReqDto reqDto)
    {
        try
        {
            var userResult = await userRepository.GetUserByEmailAsync(
                reqDto.EmailOrPhone.ToLower()
            );
            if (!userResult.IsSuccess || userResult.Data == null)
                return new Return<LoginResDto>
                {
                    IsSuccess = false,
                    StatusCode = userResult.StatusCode,
                    InternalErrorMessage = userResult.InternalErrorMessage
                };

            var user = userResult.Data;
            if (user.Status == UserStatus.Inactive)
                return new Return<LoginResDto>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.AccountIsInactive
                };

            if (
                user.PasswordHash != null
                && !HashUtil.VerifyHashedString(reqDto.Password, user.PasswordHash)
            )
                return new Return<LoginResDto>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidCredentials
                };

            user.LastLogin = DateTime.Now;
            user.ModifiedById = user.UserId;
            user.ModifiedAt = DateTime.Now;
            var updateResult = await userRepository.UpdateUserAsync(user);
            if (!updateResult.IsSuccess || updateResult.Data == null)
                return new Return<LoginResDto>
                {
                    IsSuccess = false,
                    StatusCode = updateResult.StatusCode
                };

            var token = bearerTokenUtil.GenerateBearerToken(user.UserId, user.Role);
            return new Return<LoginResDto>
            {
                Data = new LoginResDto
                {
                    BearerToken = token,
                    Name = user.FullName ?? "",
                    Email = user.Email,
                    Phone = user.Phone
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<LoginResDto>
            {
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<bool>> RegisterWithAccount(RegisterWithAccountReqDto reqDto)
    {
        try
        {
            var existedUser = await userRepository.GetUserByEmailAsync(reqDto.Email.ToLower());
            if (existedUser is { IsSuccess: true, Data: not null })
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.EmailAlreadyExists
                };

            existedUser = await userRepository.GetUserByPhoneAsync(reqDto.Phone);
            if (existedUser is { IsSuccess: true, Data: not null })
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.PhoneAlreadyExists
                };

            User newUser = new()
            {
                Email = reqDto.Email.ToLower(),
                PasswordHash = HashUtil.Hash(reqDto.Password),
                FullName = reqDto.FullName,
                Phone = reqDto.Phone,
                Point = 0,
                Role = RoleEnum.Customer,
                IsEmailVerified = false,
                IsPhoneVerified = false,
                Status = UserStatus.Active,
                CreatedAt = DateTime.Now
            };

            var userResult = await userRepository.CreateNewUser(newUser);
            if (!userResult.IsSuccess || userResult.Data == null)
                return new Return<bool> { IsSuccess = false, StatusCode = userResult.StatusCode };

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }
}
