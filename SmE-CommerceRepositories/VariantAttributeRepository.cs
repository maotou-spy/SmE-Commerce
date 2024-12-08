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
            var attributes = await dbContext.VariantAttributes.ToListAsync();

            return new Return<List<VariantAttribute>>
            {
                Data = attributes,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = attributes.Count,
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
            var attribute = await dbContext
                .VariantAttributes.Include(x => x.ProductVariants)
                .FirstOrDefaultAsync(x => x.AttributeId == id);

            return new Return<VariantAttribute>
            {
                Data = attribute,
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
        List<Guid> attributeIds
    )
    {
        try
        {
            var attributes = await dbContext
                .VariantAttributes.Where(x => attributeIds.Contains(x.AttributeId))
                .ToListAsync();

            return new Return<List<VariantAttribute>>
            {
                Data = attributes,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = attributes.Count,
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

    public async Task<Return<bool>> BulkCreateVariantAttribute(List<VariantAttribute> attributes)
    {
        try
        {
            await dbContext.VariantAttributes.AddRangeAsync(attributes);
            await dbContext.SaveChangesAsync();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = attributes.Count,
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

    public async Task<Return<bool>> UpdateVariantAttribute(VariantAttribute attribute)
    {
        try
        {
            dbContext.VariantAttributes.Update(attribute);
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

    public async Task<Return<bool>> DeleteVariantAttributes(VariantAttribute attribute)
    {
        try
        {
            dbContext.VariantAttributes.Remove(attribute);
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
