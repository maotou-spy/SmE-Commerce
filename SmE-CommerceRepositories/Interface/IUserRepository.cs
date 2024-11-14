using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface IUserRepository
{
    Task<Return<IEnumerable<User>>> GetAllUsersAsync(string? status, int? pageSize, int? pageNumber, string? phone, string? email, string? name);
    Task<Return<User>> GetUserByIdAsync(Guid id);
    Task<Return<User>> GetUserByEmailAsync(string email);
    Task<Return<User>> GetUserByPhoneAsync(string phone);
    Task<Return<User>> GetUserByEmailOrPhone(string emailOrPhone);
    Task<Return<User>> CreateNewUser(User user);
    Task<Return<User>> UpdateUser(User user);
}
