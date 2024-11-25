using SmE_CommerceModels.RequestDtos.Auth;
using SmE_CommerceModels.ResponseDtos.Auth;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IAuthService
{
    Task<Return<LoginResDto>> LoginWithAccount(LoginWithAccountReqDto reqDto);
    Task<Return<bool>> RegisterWithAccount(RegisterWithAccountReqDto reqDto);
}