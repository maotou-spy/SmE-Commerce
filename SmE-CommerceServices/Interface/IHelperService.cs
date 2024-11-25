using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IHelperService
{
    Task<Return<User>> GetCurrentUserWithRoleAsync(string role);
    Task<Return<User>> GetCurrentUser();
}