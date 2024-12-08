using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories;

public class VariantAttributeRepository(SmECommerceContext dbContext) : IVariantAttributeRepository
{
    public async Task<Return<List<VariantAttribute>>> GetVariantAttributes()
    {
        try
        {
            var variants = await dbContext.VariantAttributes.ToListAsync();

            return new Return<List<VariantAttribute>>
            {
                Data = variants,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = variants.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<VariantAttribute>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<VariantAttribute>> GetVariantAttributeById(Guid id)
    {
        try
        {
            var variant = await dbContext
                .VariantAttributes.Include(x => x.ProductVariants)
                .FirstOrDefaultAsync(x => x.VariantId == id);

            return new Return<VariantAttribute>
            {
                Data = variant,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<VariantAttribute>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<List<VariantAttribute>>> GetVariantAttributesByIds(
        List<Guid> variantIds
    )
    {
        try
        {
            var variants = await dbContext
                .VariantAttributes.Where(x => variantIds.Contains(x.VariantId))
                .ToListAsync();

            return new Return<List<VariantAttribute>>
            {
                Data = variants,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = variants.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<VariantAttribute>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<bool>> BulkCreateVariantAttribute(List<VariantAttribute> variants)
    {
        try
        {
            await dbContext.VariantAttributes.AddRangeAsync(variants);
            await dbContext.SaveChangesAsync();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = variants.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<bool>> UpdateVariantAttribute(VariantAttribute variants)
    {
        try
        {
            dbContext.VariantAttributes.Update(variants);
            await dbContext.SaveChangesAsync();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<bool>> DeleteVariantAttributes(VariantAttribute variants)
    {
        try
        {
            dbContext.VariantAttributes.Remove(variants);
            await dbContext.SaveChangesAsync();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }
}
