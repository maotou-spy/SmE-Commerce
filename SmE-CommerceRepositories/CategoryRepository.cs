using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DatabaseContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories
{
    public class CategoryRepository(DefaultdbContext dbContext) : ICategoryRepository
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
                    Message = SuccessfulMessage.Created,
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
                    Message = result != null ? SuccessfulMessage.Found : ErrorMessage.NotFound,
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
    }
}
