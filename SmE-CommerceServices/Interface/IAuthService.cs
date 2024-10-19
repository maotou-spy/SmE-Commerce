using SmE_CommerceModels.RequestDtos.Auth;
using SmE_CommerceModels.ResponseDtos;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IAuthService
{
    Task<Return<LoginResDto>> LoginWithAccount(LoginWithAccountReqDto reqDto);
}