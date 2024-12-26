using SmE_CommerceModels.RequestDtos.Category;
using SmE_CommerceModels.ResponseDtos.Category;
using SmE_CommerceModels.ResponseDtos.Category.Manager;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface ICategoryService
{
    Task<Return<bool>> AddCategoryAsync(AddCategoryReqDto req);

    Task<Return<IEnumerable<GetCategoryResDto>>> GetCategoriesForCustomerAsync(string? name, int pageNumber,
        int pageSize);

    Task<Return<IEnumerable<ManagerGetCategoryResDto>>> GetCategoriesForManagerAsync(string? name, int pageNumber,
        int pageSize);

    Task<Return<bool>> UpdateCategoryDetailAsync(Guid id, AddCategoryReqDto req);
    Task<Return<bool>> DeleteCategoryAsync(Guid id);
    Task<Return<bool>> UpdateCategoryStatusAsync(Guid id);
}