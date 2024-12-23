using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories;

public class DiscountRepository(SmECommerceContext dbContext) : IDiscountRepository
{
    #region Discount
    public async Task<Return<Discount>> AddDiscountAsync(Discount discount)
    {
        try
        {
            await dbContext.Discounts.AddAsync(discount);
            await dbContext.SaveChangesAsync();

            return new Return<Discount>
            {
                Data = discount,
                IsSuccess = true,

                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<Discount>
            {
                Data = null,
                IsSuccess = false,

                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<Discount>> GetDiscountByNameAsync(string name)
    {
        try
        {
            var discount = await dbContext.Discounts.FirstOrDefaultAsync(x =>
                x.DiscountName == name
            );
            if (discount == null)
                return new Return<Discount>
                {
                    Data = null,
                    IsSuccess = true,
                    StatusCode = ErrorCode.DiscountNotFound,
                    TotalRecord = 0,
                };
            return new Return<Discount>
            {
                Data = discount,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception e)
        {
            return new Return<Discount>
            {
                Data = null,
                IsSuccess = false,

                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e
            };
        }
    }

    // GetDiscountByIdForUpdateAsync to using FOR UPDATE
    public async Task<Return<Discount>> GetDiscountByIdForUpdateAsync(Guid id)
    {
        try
        {
            var discount = await dbContext.Discounts.Where(d => d.DiscountId == id).FirstOrDefaultAsync();
            // 1. Fetching the discount data
            var discount = await dbContext
                .Discounts.Where(d => d.DiscountId == id)
                .FirstOrDefaultAsync();
            if (discount == null)
                return new Return<Discount>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.DiscountNotFound,
                };

            await dbContext.Database.ExecuteSqlRawAsync(
                "SELECT * FROM public.\"Discounts\" WHERE \"discountId\" = {0} FOR UPDATE",
                id
            );

            return new Return<Discount>
            {
                Data = discount,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception e)
        {
            return new Return<Discount>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    public async Task<Return<Discount>> UpdateDiscountAsync(Discount discount)
    {
        try
        {
            dbContext.Discounts.Update(discount);
            await dbContext.SaveChangesAsync();

            return new Return<Discount>
            {
                Data = discount,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception e)
        {
            return new Return<Discount>
            {
                Data = null,
                IsSuccess = false,

                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    public async Task<Return<Discount>> GetDiscountByIdAsync(Guid id)
    {
        try
        {
            var discount = await dbContext.Discounts.FirstOrDefaultAsync(x => x.DiscountId == id);
            if (discount == null)
                return new Return<Discount>
                {
                    Data = null,
                    IsSuccess = true,
                    StatusCode = ErrorCode.DiscountNotFound,
                    TotalRecord = 0,
                };

            return new Return<Discount>
            {
                Data = discount,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception e)
        {
            return new Return<Discount>
            {
                Data = null,
                IsSuccess = false,

                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }
    #endregion

    #region DiscountCode
    public async Task<Return<DiscountCode>> AddDiscountCodeAsync(DiscountCode discountCode)
    {
        try
        {
            await dbContext.DiscountCodes.AddAsync(discountCode);
            await dbContext.SaveChangesAsync();

            return new Return<DiscountCode>
            {
                Data = discountCode,
                IsSuccess = true,

                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<DiscountCode>
            {
                Data = null,
                IsSuccess = false,

                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    // GetDiscountCodeByIdForUpdateAsync to using FOR UPDATE
    public async Task<Return<DiscountCode>> GetDiscountCodeByIdForUpdateAsync(Guid id)
    {
        try
        {
            var code = await dbContext.DiscountCodes.Where(d => d.CodeId == id).FirstOrDefaultAsync();
            if (code == null)

            {
                return new Return<DiscountCode>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.DiscountNotFound
                };
            }

            await dbContext.Database.ExecuteSqlRawAsync(
                "SELECT * FROM public.\"Discounts\" WHERE \"discountId\" = {0} FOR UPDATE", id);

            return new Return<DiscountCode>
            {
                Data = code,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1
            };
        }
        catch (Exception e)
        {
            return new Return<DiscountCode>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e
            };
        }
    }
    
    public async Task<Return<DiscountCode>> UpdateDiscountCodeAsync(DiscountCode discountCode)
    {
        try
        {
            dbContext.DiscountCodes.Update(discountCode);
            await dbContext.SaveChangesAsync();

            return new Return<DiscountCode>
            {
                Data = discountCode,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception e)
        {
            return new Return<DiscountCode>
            {
                Data = null,
                IsSuccess = false,

                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<DiscountCode>> GetDiscountCodeByCodeAsync(string code)
    {
        try
        {
            var discountCode = await dbContext.DiscountCodes.FirstOrDefaultAsync(x =>
                x.Code == code
            );
            if (discountCode == null)
            {
                return new Return<DiscountCode>
                {
                    Data = null,
                    IsSuccess = true,
                    StatusCode = ErrorCode.DiscountNotFound,
                    TotalRecord = 0,
                };
            }

            return new Return<DiscountCode>
            {
                Data = discountCode,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1
            };
        }
        catch (Exception e)
        {
            return new Return<DiscountCode>
            {
                Data = null,
                IsSuccess = false,

                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }
    #endregion
}
