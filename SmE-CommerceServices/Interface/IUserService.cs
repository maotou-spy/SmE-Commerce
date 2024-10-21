using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.User;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IUserService
{
    Task<Return<IEnumerable<User>>> GetAllUsersAsync();
    Task<Return<bool>> CreateUser(CreateUserReqDto req);
}
