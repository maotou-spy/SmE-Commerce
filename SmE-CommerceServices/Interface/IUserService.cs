using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.User;
using SmE_CommerceModels.ResponseDtos.User;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IUserService
{
    Task<Return<IEnumerable<AdminGetUsersByRoleResDto>>> GetUsersByRoleAsync(
        string? searchTerm,
        string? status,
        string? role,
        int? pageSize,
        int? pageNumber
    );

    Task<Return<bool>> CreateUser(CreateUserReqDto req);

    Task<Return<GetUserProfileResDto>> GetUserProfileAsync();

    Task<Return<GetUserProfileResDto>> GetUserProfileByManagerAsync(Guid id);

    Task<Return<bool>> UpdateProfileAsync(UpdateUserProfileReqDto req);

    Task<Return<bool>> DeleteUserAsync(Guid id);

    Task<Return<bool>> ChangeUserStatusAsync(Guid id);

    Task<Return<bool>> ChangePassword(ChangePasswordReqDto req);

    Task<Return<IEnumerable<UserGetTheirDiscountResDto>>> UserGetTheirDiscountsAsync();
}
