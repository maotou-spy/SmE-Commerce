using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceModels.ResponseDtos.Product;

namespace SmE_CommerceServices.Interface
{
    public interface IProductService
    {
        Task<Return<GetProductDetailsResDto>> AddProductAsync(AddProductReqDto req);

        Task<Return<IEnumerable<GetProductsResDto>>> GetProductsForCustomerAsync(string? keyword, string? sortBy,
            int pageNumber, int pageSize);
        Task<Return<GetProductAttributeResDto>> AddProductAttributeAsync(Guid productId, AddProductAttributeReqDto req);

        Task<Return<GetProductAttributeResDto>> UpdateProductAttributeAsync(Guid productId, Guid AttributeId, AddProductAttributeReqDto req);
        Task<Return<bool>> DeleteProductAttributeAsync(Guid productId, Guid attributeId);

        Task<Return<GetProductImageResDto>> AddProductImageAsync(Guid productId, AddProductImageReqDto req);
        Task<Return<List<GetProductCategoryResDto>>> UpdateProductCategoryAsync(Guid productId, List<Guid> categoryIds);
    }
}
