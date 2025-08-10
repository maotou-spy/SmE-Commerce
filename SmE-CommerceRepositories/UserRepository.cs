using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceUtilities.Utils;

namespace SmE_CommerceRepositories;

public class UserRepository(SmECommerceContext dbContext) : IUserRepository
{
    public async Task<Return<IEnumerable<User>>> GetUsersByRoleAsync(
        string? searchTerm,
        string? status,
        string? role,
        int pageSize,
        int pageNumber
    )
    {
        try
        {
            var query = dbContext
                .Users.AsNoTracking()
                .Include(u => u.AddressUsers.Where(a => a.IsDefault))
                .Where(x => x.Status != UserStatus.Deleted);

            if (!string.IsNullOrWhiteSpace(role))
            {
                role = role.Trim().ToLower();
                query = query.Where(x => x.Role.Contains(role));
            }
            else
            {
                query = query.Where(x => x.Role == RoleEnum.Customer);
            }

            if (!string.IsNullOrWhiteSpace(status) && status != UserStatus.Deleted)
                query = query.Where(x => x.Status == status);

            var allUsers = await query.ToListAsync(); // fetch full result first

            // Apply search term filtering on client side (case + accent insensitive)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedSearch = StringUtils.SimplifyText(searchTerm);
                allUsers = allUsers
                    .Where(x =>
                        StringUtils.SimplifyText(x.FullName).Contains(normalizedSearch)
                        || x.Email.Contains(normalizedSearch)
                        || (!string.IsNullOrEmpty(x.Phone) && x.Phone.Contains(searchTerm))
                    )
                    .ToList();
            }

            var totalRecord = allUsers.Count;

            if (pageSize <= 0)
                return new Return<IEnumerable<User>>
                {
                    Data = allUsers,
                    IsSuccess = true,
                    StatusCode = allUsers.Count != 0 ? ErrorCode.Ok : ErrorCode.UserNotFound,
                    TotalRecord = totalRecord,
                };
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            allUsers = allUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new Return<IEnumerable<User>>
            {
                Data = allUsers,
                IsSuccess = true,
                StatusCode = allUsers.Count != 0 ? ErrorCode.Ok : ErrorCode.UserNotFound,
                TotalRecord = totalRecord,
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<User>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<User>> GetUserByIdAsync(Guid id)
    {
        try
        {
            var result = await dbContext
                .Users.Where(x => x.Status != GeneralStatus.Deleted)
                .Include(x => x.AddressUsers.Where(a => a.IsDefault))
                .FirstOrDefaultAsync(x => x.UserId == id);

            return new Return<User>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = result != null ? ErrorCode.Ok : ErrorCode.UserNotFound,
                TotalRecord = result != null ? 1 : 0,
            };
        }
        catch (Exception ex)
        {
            return new Return<User>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,

                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<User>> GetUserByEmailAsync(string email)
    {
        try
        {
            var result = await dbContext
                .Users.Where(x => x.Status != UserStatus.Deleted)
                .FirstOrDefaultAsync(x => x.Email == email);

            return new Return<User>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = result != null ? ErrorCode.Ok : ErrorCode.UserNotFound,
                TotalRecord = result != null ? 1 : 0,
            };
        }
        catch (Exception ex)
        {
            return new Return<User>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,

                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<User>> GetUserByPhoneAsync(string phone)
    {
        try
        {
            var result = await dbContext
                .Users.Where(x => x.Status != UserStatus.Deleted)
                .FirstOrDefaultAsync(x => x.Phone == phone);

            return new Return<User>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = result != null ? ErrorCode.Ok : ErrorCode.UserNotFound,
                TotalRecord = result != null ? 1 : 0,
            };
        }
        catch (Exception ex)
        {
            return new Return<User>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,

                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<User>> GetUserByEmailOrPhone(string emailOrPhone)
    {
        try
        {
            var result = await dbContext
                .Users.Where(x => x.Status != UserStatus.Deleted)
                .FirstOrDefaultAsync(x => x.Email == emailOrPhone || x.Phone == emailOrPhone);

            return new Return<User>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = result != null ? ErrorCode.Ok : ErrorCode.UserNotFound,
                TotalRecord = result != null ? 1 : 0,
            };
        }
        catch (Exception ex)
        {
            return new Return<User>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,

                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<User>> CreateNewUser(User user)
    {
        try
        {
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return new Return<User>
            {
                Data = user,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,

                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<User>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,

                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<User>> UpdateUserAsync(User user)
    {
        try
        {
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();

            return new Return<User>
            {
                Data = user,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<User>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,

                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<IEnumerable<DiscountCode>>> UserGetDiscountsByUserIdAsync(Guid cusId)
    {
        try
        {
            var discountCodes = await dbContext
                .DiscountCodes.Include(x => x.User)
                .Include(x => x.Discount)
                .Where(x =>
                    x.UserId == cusId || (x.UserId == null && x.Status == GeneralStatus.Active)
                )
                .ToListAsync();

            return new Return<IEnumerable<DiscountCode>>
            {
                Data = discountCodes,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = discountCodes.Count,
            };
        }
        catch (Exception e)
        {
            return new Return<IEnumerable<DiscountCode>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
                TotalRecord = 0,
            };
        }
    }
}
