using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Category;
using SmE_CommerceModels.ResponseDtos.Category;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface
{
    public interface ICategoryService
    {
        Task<Return<bool>> AddCategoryAsync(AddCategoryReqDto req);

        Task<Return<IEnumerable<GetCategoryResDto>>> GetCategoriesForCustomerAsync(string? name, int pageNumber,
            int pageSize);

        Task<Return<IEnumerable<Category>>> GetCategoriesForManagerAsync(string? name, int pageNumber,
            int pageSize);
    }
}
