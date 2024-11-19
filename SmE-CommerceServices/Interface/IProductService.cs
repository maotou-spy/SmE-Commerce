using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceModels.ResponseDtos.Product;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceModels.ResponseDtos.Product.Manager;

namespace SmE_CommerceServices.Interface
{
    public interface IProductService
    {
        Task<Return<GetProductDetailsResDto>> CustomerGetProductDetailsAsync(Guid productId);
        Task<Return<ManagerGetProductDetailResDto>> ManagerGetProductDetailsAsync(Guid productId);
        Task<Return<GetProductDetailsResDto>> AddProductAsync(AddProductReqDto req);
        // Task<Return<IEnumerable<GetProductsResDto>>> CustomerGetProductsAsync(string? keyword, string? sortBy,
        //     int pageNumber, int pageSize);
        Task<Return<GetProductDetailsResDto>> UpdateProductAsync(Guid productId, UpdateProductReqDto req);
        Task<Return<bool>> DeleteProductAsync(Guid productId);

        Task<Return<GetProductAttributeResDto>> AddProductAttributeAsync(Guid productId, AddProductAttributeReqDto req);
        Task<Return<GetProductAttributeResDto>> UpdateProductAttributeAsync(Guid productId, Guid AttributeId, AddProductAttributeReqDto req);
        Task<Return<bool>> DeleteProductAttributeAsync(Guid productId, Guid attributeId);

        Task<Return<GetProductImageResDto>> AddProductImageAsync(Guid productId, AddProductImageReqDto req);
        Task<Return<GetProductImageResDto>> UpdateProductImageAsync(Guid productId, Guid imageId, AddProductImageReqDto req);
        Task<Return<bool>> DeleteProductImageAsync(Guid productId, Guid imageId);

        Task<Return<List<GetProductCategoryResDto>>> UpdateProductCategoryAsync(Guid productId, List<Guid> categoryIds);
    }
}
