using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IHelperService
{
    Task<Return<User>> GetCurrentUser(string role);
}
