﻿using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories;

public class UserRepository(SmECommerceContext dbContext) : IUserRepository
{
    public async Task<Return<IEnumerable<User>>> GetAllUsersAsync(
        string? status,
        int? pageSize,
        int? pageNumber,
        string? phone,
        string? email,
        string? name
    )
    {
        try
        {
            var query = dbContext.Users.Where(x => x.Status != GeneralStatus.Deleted);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            if (!string.IsNullOrWhiteSpace(phone))
                query = query.Where(x => x.Phone != null && x.Phone.Contains(phone));

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(x => x.Email.Contains(email));

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(x => x.FullName.Contains(name));

            var totalRecord = await query.CountAsync();

            if (pageSize is > 0)
            {
                pageNumber ??= 1;
                query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            var result = await query.ToListAsync();

            // Trả về kết quả
            return new Return<IEnumerable<User>>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = result.Count != 0 ? ErrorCode.Ok : ErrorCode.UserNotFound,
                TotalRecord = totalRecord
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
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<User>> GetUserByIdAsync(Guid id)
    {
        try
        {
            var result = await dbContext
                .Users.Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.UserId == id);

            return new Return<User>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = result != null ? ErrorCode.Ok : ErrorCode.UserNotFound,
                TotalRecord = result != null ? 1 : 0
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
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<User>> GetUserByEmailAsync(string email)
    {
        try
        {
            var result = await dbContext
                .Users.Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.Email == email);

            return new Return<User>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = result != null ? ErrorCode.Ok : ErrorCode.UserNotFound,
                TotalRecord = result != null ? 1 : 0
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
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<User>> GetUserByPhoneAsync(string phone)
    {
        try
        {
            var result = await dbContext
                .Users.Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.Phone == phone);

            return new Return<User>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = result != null ? ErrorCode.Ok : ErrorCode.UserNotFound,
                TotalRecord = result != null ? 1 : 0
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
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<User>> GetUserByEmailOrPhone(string emailOrPhone)
    {
        try
        {
            var result = await dbContext
                .Users.Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.Email == emailOrPhone || x.Phone == emailOrPhone);

            return new Return<User>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = result != null ? ErrorCode.Ok : ErrorCode.UserNotFound,
                TotalRecord = result != null ? 1 : 0
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
                StatusCode = ErrorCode.Ok,

                TotalRecord = 1
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
                TotalRecord = 0
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
                TotalRecord = 1
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
                TotalRecord = 0
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
                TotalRecord = discountCodes.Count
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
                TotalRecord = 0
            };
        }
    }
}
