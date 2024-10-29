using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceModels.ResponseDtos.Product;

namespace SmE_CommerceServices.Interface
{
    public interface IProductService
    {
        Task<Return<bool>> AddProductAsync(AddProductReqDto req);

        Task<Return<IEnumerable<GetProductsResDto>>> GetProductsForCustomerAsync(string? keyword, string? sortBy,
            int pageNumber, int pageSize);
    }
}
