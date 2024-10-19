using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos.Auth;
using SmE_CommerceModels.ResponseDtos;
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
                    Message = ErrorMessage.ACCOUNT_IS_INACTIVE
                };
            }

            if (user.PasswordHash != null && !HashUtil.VerifyHashedString(reqDto.Password, user.PasswordHash))
            {
                return new Return<LoginResDto>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.INVALID_CREDENTIALS
                };
            }

            user.LastLogin = DateTime.Now;

            var updateResult = await userRepository.UpdateUser(user);

            if (!updateResult.IsSuccess || updateResult.Data == null)
            {
                return new Return<LoginResDto>
                {
                    IsSuccess = false,
                    Message = ErrorMessage.SERVER_ERROR
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

}
