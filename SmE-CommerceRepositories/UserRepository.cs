using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DatabaseContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories;

public class UserRepository : IUserRepository
{
    private readonly DefaultdbContext _dbContext;

    public UserRepository(DefaultdbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Return<IEnumerable<User>>> GetAllUsersAsync()
    {
        try
        {
            var result = await _dbContext.Users.Where(x => x.Status != GeneralStatus.Deleted).ToListAsync();

            return new Return<IEnumerable<User>>
            {
                Data = result,
                IsSuccess = true,
                Message = result.Count > 0 ? SuccessfulMessage.Found : ErrorMessage.NotFound,
                TotalRecord = result.Count
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
            var result = await _dbContext.Users
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
            var result = await _dbContext.Users
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
            var result = await _dbContext.Users
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
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

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
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

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
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

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
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

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
