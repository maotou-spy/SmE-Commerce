using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceModels.ResponseDtos.Product;
using SmE_CommerceModels.ResponseDtos.Product.Manager;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IProductService
{
    Task<Return<List<GetProductsResDto>>> CustomerGetProductsAsync(ProductFilterReqDto filter);

    Task<Return<List<ManagerGetProductsResDto>>> ManagerGetProductsAsync(
        ProductFilterReqDto filter
    );

    Task<Return<GetProductDetailsResDto>> CustomerGetProductDetailsAsync(Guid productId);

    Task<Return<ManagerGetProductDetailResDto>> ManagerGetProductDetailsAsync(Guid productId);

    Task<Return<GetProductDetailsResDto>> AddProductAsync(AddProductReqDto req);

    Task<Return<GetProductDetailsResDto>> UpdateProductAsync(
        Guid productId,
        UpdateProductReqDto req
    );

    Task<Return<bool>> DeleteProductAsync(Guid productId);

    Task<Return<GetProductAttributeResDto>> AddProductAttributeAsync(
        Guid productId,
        AddProductAttributeReqDto req
    );

    Task<Return<IEnumerable<GetProductsResDto>>> GetRelatedProducts(Guid productId, int limit = 5);

    Task<Return<GetProductAttributeResDto>> UpdateProductAttributeAsync(
        Guid productId,
        Guid attributeId,
        AddProductAttributeReqDto req
    );

    Task<Return<bool>> DeleteProductAttributeAsync(Guid productId, Guid attributeId);

    Task<Return<GetProductImageResDto>> AddProductImageAsync(
        Guid productId,
        AddProductImageReqDto req
    );

    Task<Return<GetProductImageResDto>> UpdateProductImageAsync(
        Guid productId,
        Guid imageId,
        AddProductImageReqDto req
    );

    Task<Return<bool>> DeleteProductImageAsync(Guid productId, Guid imageId);

    Task<Return<List<GetProductCategoryResDto>>> UpdateProductCategoryAsync(
        Guid productId,
        List<Guid> categoryIds
    );

    Task<Return<bool>> AddProductVariantAsync(Guid productId, List<ProductVariantReqDto> req);

    Task<Return<bool>> UpdateProductVariantAsync(
        Guid productId,
        Guid productVariantId,
        ProductVariantReqDto req
    );

    Task<Return<bool>> DeleteProductVariantAsync(Guid productId, Guid variantId);
}
