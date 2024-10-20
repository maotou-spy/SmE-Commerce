using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Auth;
using SmE_CommerceModels.ResponseDtos.Auth;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceUtilities;

public class AuthService(IUserRepository userRepository, BearerTokenUtil bearerTokenUtil)
    : IAuthService
{
    public async Task<Return<LoginResDto>> LoginWithAccount(LoginWithAccountReqDto reqDto)
    {
        try
        {
            var userResult = await userRepository.GetUserByEmailOrPhoneAsync(reqDto.EmailOrPhone);
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
                    Message = ErrorMessage.AccountIsInactive
                };
            }

            if (user.PasswordHash != null && !HashUtil.VerifyHashedString(reqDto.Password, user.PasswordHash))
            {
                return new Return<LoginResDto>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.InvalidCredentials
                };
            }

            user.LastLogin = DateTime.Now;
            user.ModifiedById = user.UserId;
            user.ModifiedAt = DateTime.Now;
            var updateResult = await userRepository.UpdateUser(user);
            if (!updateResult.IsSuccess || updateResult.Data == null)
            {
                return new Return<LoginResDto>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.ServerError
                };
            }

            var token = bearerTokenUtil.GenerateBearerToken(user.UserId, user.Role);
            return new Return<LoginResDto>
            {
                Data = new LoginResDto
                {
                    BearerToken = token,
                    Name = user.FullName ?? "",
                    Email = user.Email,
                    Phone = user.Phone,
                },
                IsSuccess = true,
                Message = SuccessfulMessage.Successfully
            };
        }
        catch (Exception ex)
        {
            return new Return<LoginResDto>
            {
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<bool>> RegisterWithAccount(RegisterWithAccountReqDto reqDto)
    {
        try
        {
            var existedUser = await userRepository.GetUserByEmailOrPhoneAsync(reqDto.Email);
            if (existedUser is { IsSuccess: true, Data: not null })
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.UserAlreadyExists
                };
            }

            User newUser = new()
            {
                Email = reqDto.Email,
                PasswordHash = HashUtil.Hash(reqDto.Password),
                FullName = reqDto.FullName,
                Phone = reqDto.Phone,
                Role = RoleEnum.Customer,
                Status = UserStatus.Active,
                CreateById = Guid.Empty,
                CreatedAt = DateTime.Now
            };

            var userResult = await userRepository.CreateNewUser(newUser);
            if (!userResult.IsSuccess || userResult.Data == null)
            {
                return new Return<bool>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.ServerError
                };
            }

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                Message = SuccessfulMessage.Successfully
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }
}
