using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories;

public class VariantNameRepository(SmECommerceContext dbContext) : IVariantNameRepository
{
    public async Task<Return<List<VariantName>>> GetVariantNamesAsync()
    {
        try
        {
            var variants = await dbContext.VariantNames.ToListAsync();

            return new Return<List<VariantName>>
            {
                Data = variants,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = variants.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<VariantName>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<VariantName>> GetVariantNameByIdAsync(Guid id)
    {
        try
        {
            var variant = await dbContext.VariantNames.FirstOrDefaultAsync(x =>
                x.VariantNameId == id
            );

            return new Return<VariantName>
            {
                Data = variant,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<VariantName>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<bool>> BulkCreateVariantName(List<VariantName> variants)
    {
        try
        {
            await dbContext.VariantNames.AddRangeAsync(variants);
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

    public async Task<Return<bool>> UpdateVariantNameAsync(VariantName variants)
    {
        try
        {
            dbContext.VariantNames.Update(variants);
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

    public async Task<Return<bool>> DeleteVariantNamesAsync(VariantName variants)
    {
        try
        {
            dbContext.VariantNames.Remove(variants);
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
