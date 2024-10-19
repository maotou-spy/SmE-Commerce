using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface IUserRepository
{
    Task<Return<IEnumerable<User>>> GetAllUsersAsync();
    Task<Return<User>> GetUserByIdAsync(Guid id);
    Task<Return<User>> GetUserByEmailAsync(string email);
    Task<Return<User>> GetUserByEmailOrPhoneAsync(string emailOrPhone);
    Task<Return<User>> CreateNewUser(User user);
}
