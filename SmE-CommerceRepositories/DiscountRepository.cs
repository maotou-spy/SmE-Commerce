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
                TotalRecord = 1
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
                TotalRecord = 0
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
                    TotalRecord = 0
                };
            return new Return<Discount>
            {
                Data = discount,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1
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
            var discount = await dbContext
                .Discounts.Include(x => x.DiscountProducts)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.DiscountId == id);

            if (discount == null)
                return new Return<Discount>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.DiscountNotFound,
                    TotalRecord = 0
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
                TotalRecord = 1
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
                TotalRecord = 0
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
                TotalRecord = 1
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
                    TotalRecord = 0
                };

            return new Return<Discount>
            {
                Data = discount,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1
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

    public async Task<Return<IEnumerable<Discount>>> GetDiscountsAsync(
        string? name,
        int? pageNumber,
        int? pageSize
    )
    {
        try
        {
            var query = dbContext.Discounts.AsQueryable();

            query = query.Where(x => x.Status != GeneralStatus.Deleted);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.DiscountName.Contains(name));

            var totalRecords = await query.CountAsync();

            if (pageNumber.HasValue && pageSize.HasValue)
                query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);

            var result = await query.ToListAsync();

            return new Return<IEnumerable<Discount>>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = totalRecords
            };
        }
        catch (Exception e)
        {
            return new Return<IEnumerable<Discount>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
                TotalRecord = 0
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
                TotalRecord = 1
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
                TotalRecord = 0
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
                TotalRecord = 0
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
                return new Return<DiscountCode>
                {
                    Data = null,
                    IsSuccess = true,
                    StatusCode = ErrorCode.DiscountNotFound,
                    TotalRecord = 0
                };

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
                InternalErrorMessage = e
            };
        }
    }

    public async Task<Return<DiscountCode>> GetDiscountCodeByIdForUpdateAsync(Guid id)
    {
        try
        {
            // Lấy discount code từ cơ sở dữ liệu cùng các liên kết liên quan (nếu có)
            var discountCode = await dbContext
                .DiscountCodes.Include(x => x.Discount)
                .ThenInclude(x => x.DiscountProducts)
                .Where(x => x.Status != DiscountCodeStatus.Deleted)
                .FirstOrDefaultAsync(x => x.CodeId == id);

            // Nếu không tìm thấy discount code
            if (discountCode == null)
                return new Return<DiscountCode>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.DiscountCodeNotFound,
                    TotalRecord = 0
                };

            // Đánh dấu bản ghi FOR UPDATE để thực hiện đồng bộ
            await dbContext.Database.ExecuteSqlRawAsync(
                "SELECT * FROM public.\"DiscountCodes\" WHERE \"codeId\" = {0} FOR UPDATE",
                id
            );

            // Trả về dữ liệu thành công
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
            // Xử lý lỗi và trả về thông tin lỗi
            return new Return<DiscountCode>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<DiscountCode>> GetDiscountCodeByIdAsync(Guid id)
    {
        try
        {
            var discountCode = await dbContext
                .DiscountCodes.Include(x => x.Discount)
                .ThenInclude(x => x.DiscountProducts)
                .Where(x => x.Status != DiscountCodeStatus.Deleted)
                .FirstOrDefaultAsync(x => x.CodeId == id);
            if (discountCode == null)
                return new Return<DiscountCode>
                {
                    Data = null,
                    IsSuccess = true,
                    StatusCode = ErrorCode.DiscountCodeNotFound,
                    TotalRecord = 0
                };

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
                InternalErrorMessage = e
            };
        }
    }

    public async Task<Return<IEnumerable<DiscountCode>>> GetDiscountCodesByDiscountIdAsync(
        Guid id,
        int? pageNumber,
        int? pageSize
    )
    {
        try
        {
            var query = dbContext
                .DiscountCodes.Where(x => x.Status != GeneralStatus.Deleted && x.DiscountId == id)
                .AsQueryable();

            var totalRecords = await query.CountAsync();

            if (pageNumber.HasValue && pageSize.HasValue)
                query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);

            var result = await query.ToListAsync();

            return new Return<IEnumerable<DiscountCode>>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = totalRecords
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

    public async Task<Return<IEnumerable<DiscountCode>>> GetDiscountCodesByDiscountIdAsyncForUpdate(
        Guid id
    )
    {
        try
        {
            // Lọc các bản ghi theo DiscountId và trạng thái không bị xóa
            var query = dbContext
                .DiscountCodes.Where(x => x.Status != GeneralStatus.Deleted && x.DiscountId == id)
                .AsQueryable();

            // Đếm tổng số bản ghi sau khi áp dụng các bộ lọc
            var totalRecords = await query.CountAsync();

            // Truy vấn toàn bộ kết quả
            var result = await query.ToListAsync();

            // Trả về kết quả thành công
            return new Return<IEnumerable<DiscountCode>>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = totalRecords
            };
        }
        catch (Exception e)
        {
            // Xử lý lỗi và trả về thông tin lỗi
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

    #endregion
}
