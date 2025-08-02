using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface IUserRepository
{
    Task<Return<IEnumerable<User>>> GetUsersByRoleAsync(
        string? searchTerm,
        string? status,
        string? role,
        int pageSize,
        int pageNumber
    );

    Task<Return<User>> GetUserByIdAsync(Guid id);
    Task<Return<User>> GetUserByEmailAsync(string email);
    Task<Return<User>> GetUserByPhoneAsync(string phone);
    Task<Return<User>> GetUserByEmailOrPhone(string emailOrPhone);
    Task<Return<User>> CreateNewUser(User user);
    Task<Return<User>> UpdateUserAsync(User user);

    Task<Return<IEnumerable<DiscountCode>>> UserGetDiscountsByUserIdAsync(Guid cusId);
}
