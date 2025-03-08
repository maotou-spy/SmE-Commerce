using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories;

public class CategoryRepository(SmECommerceContext dbContext) : ICategoryRepository
{
    public async Task<Return<Category>> AddCategoryAsync(Category category)
    {
        try
        {
            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();

            return new Return<Category>
            {
                Data = category,
                IsSuccess = true,

                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<Category>
            {
                Data = null,
                IsSuccess = false,

                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<Category>> GetCategoryByNameAsync(string name)
    {
        try
        {
            var result = await dbContext
                .Categories.Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.Name == name);

            if (result == null)
                return new Return<Category>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CategoryNotFound,
                    TotalRecord = 0,
                };

            return new Return<Category>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<Category>
            {
                Data = null,
                IsSuccess = false,

                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<IEnumerable<Category>>> GetCategoriesAsync(
        string? name,
        string? status,
        int? pageNumber,
        int? pageSize
    )
    {
        try
        {
            var query = dbContext.Categories.AsQueryable();

            query = query.Where(x => x.Status != GeneralStatus.Deleted);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.Status.Equals(status));

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Name.Contains(name));

            var totalRecords = await query.CountAsync();

            if (pageNumber.HasValue && pageSize.HasValue)
                query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);

            var result = await query.ToListAsync();

            return new Return<IEnumerable<Category>>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = totalRecords,
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<Category>>
            {
                Data = null,
                IsSuccess = false,

                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<Category>> GetCategoryByIdAsync(Guid id)
    {
        try
        {
            var result = await dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (result == null)
                return new Return<Category>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CategoryNotFound,
                    TotalRecord = 0,
                };

            return new Return<Category>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<Category>
            {
                Data = null,
                IsSuccess = false,

                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<Category>> GetCategoryByIdForUpdateAsync(Guid categoryId)
    {
        try
        {
            var category = await dbContext
                .Categories.Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.CategoryId == categoryId);

            if (category is null)
                return new Return<Category>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CategoryNotFound,
                    TotalRecord = 0,
                };

            await dbContext.Database.ExecuteSqlRawAsync(
                "SELECT * FROM public.\"Categories\" WHERE public.\"Categories\".\"categoryId\" = {0} FOR UPDATE",
                categoryId
            );

            return new Return<Category>
            {
                Data = category,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<Category>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<Category>> UpdateCategoryAsync(Category category)
    {
        try
        {
            dbContext.Categories.Update(category);
            await dbContext.SaveChangesAsync();

            return new Return<Category>
            {
                Data = category,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<Category>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<IEnumerable<Category>>> GetProductsByCategoryIdAsync(Guid id)
    {
        try
        {
            var result = await dbContext
                .Categories.Include(x => x.ProductCategories)
                .ThenInclude(y => y.Product)
                .Where(x => x.CategoryId == id)
                .ToListAsync();

            return new Return<IEnumerable<Category>>
            {
                Data = result,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = result.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<Category>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }
}
