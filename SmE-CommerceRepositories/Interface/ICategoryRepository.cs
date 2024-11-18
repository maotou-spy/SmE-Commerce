using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceModels.Enums;

namespace SmE_CommerceRepositories.Interface
{
    public interface ICategoryRepository
    {
        Task<Return<Category>> AddCategoryAsync(Category category);
        Task<Return<Category>> GetCategoryByNameAsync(string name);
        Task<Return<IEnumerable<Category>>> GetCategories(string? name, string? status,
            int pageNumber = PagingEnum.PageNumber, int pageSize = PagingEnum.PageSize);
        Task<Return<Category>> GetCategoryByIdAsync(Guid id);
        Task<Return<Category>> UpdateCategoryAsync(Category category);
    }
}
