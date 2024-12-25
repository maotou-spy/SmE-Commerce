using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface IProductRepository
{
    Task<Return<Product>> GetProductByIdAsync(Guid productId);

    Task<Return<Product>> GetProductByNameAsync(string productName);

    Task<Return<Product>> AddProductAsync(Product product);

    Task<Return<Product>> GetProductByIdForUpdateAsync(Guid productId);

    Task<Return<Product>> UpdateProductAsync(Product product);

    Task<Return<List<ProductCategory>>> GetProductCategoriesAsync(Guid productId);

    Task<Return<List<ProductCategory>>> AddProductCategoriesAsync(
        List<ProductCategory> productCategories
    );

    Task<Return<ProductCategory>> DeleteProductCategoryAsync(
        Guid productId,
        List<Guid> categoryIds
    );

    Task<Return<List<ProductImage>>> GetProductImagesAsync(Guid productId);

    Task<Return<ProductImage>> GetProductImageByIdAsync(Guid productImageId);

    Task<Return<ProductImage>> AddProductImageAsync(ProductImage productImage);

    Task<Return<ProductImage>> UpdateProductImageAsync(ProductImage productImage);

    Task<Return<ProductImage>> DeleteProductImageAsync(Guid productImageId);

    Task<Return<ProductAttribute>> GetProductAttributeByIdAsync(Guid attributeId);

    Task<Return<ProductAttribute>> AddProductAttributeAsync(ProductAttribute productAttribute);

    Task<Return<ProductAttribute>> UpdateProductAttributeAsync(ProductAttribute productAttributes);

    Task<Return<ProductAttribute>> DeleteProductAttributeAsync(Guid attributeId);

    Task<Return<bool>> AddProductVariantAsync(ProductVariant productVariant);

    Task<Return<ProductVariant>> GetProductVariantByIdAsync(Guid variantNameId);

    Task<Return<bool>> IsProductVariantExistAsync(string variantValue, Guid productId);

    Task<Return<bool>> UpdateProductVariantAsync(ProductVariant productVariant);
}
