using SmE_CommerceModels.RequestDtos.Category;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface
{
    public interface ICategoryService
    {
        Task<Return<bool>> AddCategoryAsync(AddCategoryReqDto req);
    }
}
