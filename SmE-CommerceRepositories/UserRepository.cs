using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DatabaseContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories;

public class UserRepository(DefaultdbContext dbContext) : IUserRepository
{
    public async Task<Return<IEnumerable<User>>> GetAllUsersAsync(
    string? status, int? pageSize, int? pageNumber,
    string? phone, string? email, string? name)
    {
        try
        {
            var query = dbContext.Users.Where(x => x.Status != GeneralStatus.Deleted);

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(x => x.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                query = query.Where(x => x.Phone.Contains(phone));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(x => x.Email.Contains(email));
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(x => x.FullName.Contains(name));
            }

            var totalRecord = await query.CountAsync();

            List<User> result;
            if (pageSize is > 0)
            {
                pageNumber ??= 1;
                result = await query
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToListAsync();
            }
            else
            {
                result = await query.ToListAsync();
            }

            // Trả về kết quả
            return new Return<IEnumerable<User>>
            {
                Data = result,
                IsSuccess = true,
                Message = result.Any() ? SuccessfulMessage.Found : ErrorMessage.NotFound,
                TotalRecord = totalRecord
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<User>>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<User>> GetUserByIdAsync(Guid id)
    {
        try
        {
            var result = await dbContext.Users
                .Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.UserId == id);

            return new Return<User>
            {
                Data = result,
                IsSuccess = true,
                Message = result != null ? SuccessfulMessage.Found : ErrorMessage.NotFound,
                TotalRecord = result != null ? 1 : 0
            };
        }
        catch (Exception ex)
        {
            return new Return<User>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<User>> GetUserByEmailAsync(string email)
    {
        try
        {
            var result = await dbContext.Users
                .Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.Email == email);

            return new Return<User>
            {
                Data = result,
                IsSuccess = true,
                Message = result != null ? SuccessfulMessage.Found : ErrorMessage.NotFound,
                TotalRecord = result != null ? 1 : 0
            };
        }
        catch (Exception ex)
        {
            return new Return<User>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<User>> GetUserByEmailOrPhoneAsync(string emailOrPhone)
    {
        try
        {
            var result = await dbContext.Users
                .Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.Email == emailOrPhone || x.Phone == emailOrPhone);

            return new Return<User>
            {
                Data = result,
                IsSuccess = true,
                Message = result != null ? SuccessfulMessage.Found : ErrorMessage.NotFound,
                TotalRecord = result != null ? 1 : 0
            };
        }
        catch (Exception ex)
        {
            return new Return<User>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
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
                Message = SuccessfulMessage.Created,
                TotalRecord = 1
            };
        }
        catch (Exception ex)
        {
            return new Return<User>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<User>> UpdateUser(User user)
    {
        try
        {
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();

            return new Return<User>
            {
                Data = user,
                IsSuccess = true,
                Message = SuccessfulMessage.Updated,
                TotalRecord = 1
            };
        }
        catch (Exception ex)
        {
            return new Return<User>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<User>> ChangeUserStatus(User user)
    {
        try
        {
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();

            return new Return<User>
            {
                Data = user,
                IsSuccess = true,
                Message = SuccessfulMessage.Updated,
                TotalRecord = 1
            };
        }
        catch (Exception ex)
        {
            return new Return<User>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<User>> ChangeUserPassword(User user)
    {
        try
        {
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync();

            return new Return<User>
            {
                Data = user,
                IsSuccess = true,
                Message = SuccessfulMessage.Updated,
                TotalRecord = 1
            };
        }
        catch (Exception ex)
        {
            return new Return<User>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }
}
