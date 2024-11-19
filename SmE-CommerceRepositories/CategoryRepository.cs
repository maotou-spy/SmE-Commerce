using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories
{
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
                    Message = SuccessMessage.Created,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<Category>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<Category>> GetCategoryByNameAsync(string name)
        {
            try
            {
                var result = await dbContext.Categories
                .Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.Name == name);

                return new Return<Category>
                {
                    Data = result,
                    IsSuccess = true,
                    Message = result != null ? SuccessMessage.Found : ErrorMessage.NotFound,
                    TotalRecord = result != null ? 1 : 0
                };
            }
            catch (Exception ex)
            {
                return new Return<Category>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<IEnumerable<Category>>> GetCategories(string? name, string? status,
            int pageNumber = PagingEnum.PageNumber, int pageSize = PagingEnum.PageSize)
        {
            try
            {
                var query = dbContext.Categories.AsQueryable();

                query = query.Where(x => x.Status != GeneralStatus.Deleted);

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(x => x.Status.Equals(status));
                }

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                var totalRecords = await query.CountAsync();

                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

                var result = await query.ToListAsync();

                return new Return<IEnumerable<Category>>
                {
                    Data = result,
                    IsSuccess = true,
                    Message = SuccessMessage.Successfully,
                    TotalRecord = totalRecords
                };
            }
            catch (Exception ex)
            {
                return new Return<IEnumerable<Category>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<Category>> GetCategoryByIdAsync(Guid id)
        {
            try
            {
                var result = await dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);

                return new Return<Category>
                {
                    Data = result,
                    IsSuccess = true,
                    Message = result != null ? SuccessMessage.Found : ErrorMessage.NotFound,
                    TotalRecord = result != null ? 1 : 0
                };
            }
            catch (Exception ex)
            {
                return new Return<Category>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
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
                    Message = SuccessMessage.Updated,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<Category>
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
}
