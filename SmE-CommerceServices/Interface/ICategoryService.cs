using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Category;
using SmE_CommerceModels.ResponseDtos.Category;
using SmE_CommerceModels.ResponseDtos.Category.Custumer;
using SmE_CommerceModels.ResponseDtos.Category.Manager;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface
{
    public interface ICategoryService
    {
        Task<Return<bool>> AddCategoryAsync(AddCategoryReqDto req);

        Task<Return<IEnumerable<GetCategoryResDto>>> GetCategoriesForCustomerAsync(string? name, int pageNumber,
            int pageSize);

        Task<Return<IEnumerable<ManagerGetCategoryResDto>>> GetCategoriesForManagerAsync(string? name, int pageNumber,
            int pageSize);

        Task<Return<GetCategoryDetailResDto?>> GetCategoryDetailForCustomerAsync(Guid id);
        Task<Return<Category>> GetCategoryDetailForManagerAsync(Guid id);
        Task<Return<bool>> UpdateCategoryDetailAsync(Guid id, AddCategoryReqDto req);
        Task<Return<bool>> DeleteCategoryAsync(Guid id);
    }
}