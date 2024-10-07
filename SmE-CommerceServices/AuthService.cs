using SmE_CommerceModels.Enums;
using SmE_CommerceModels.RequestDtos;
using SmE_CommerceModels.ResponseDtos;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceUtilities;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly BearerTokenUtil _bearerTokenUtil;

    public AuthService(IUserRepository userRepository, BearerTokenUtil bearerTokenUtil)
    {
        _userRepository = userRepository;
        _bearerTokenUtil = bearerTokenUtil;
    }

    public async Task<Return<LoginResDto>> LoginWithAccount(LoginWithAccountReqDto reqDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(reqDto.Email);

        if (!user.IsSuccess)
            return new Return<LoginResDto>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = user.InternalErrorMessage
            };

        if (user.Data == null)
            return new Return<LoginResDto>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.NotFound
            };

        //if(_)

        var token = _bearerTokenUtil.GenerateBearerToken(user.Data.UserId, user.Data.Role);

        return new Return<LoginResDto>
        {
            Data = new LoginResDto
            {
                BearerToken = token,
                Email = user.Data.Email,
                Name = user.Data.FullName
            },
            IsSuccess = true,
            Message = SuccessfulMessage.Successfully
        };
    }
}
