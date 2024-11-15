using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface
{
    public interface IProductRepository
    {
        Task<Return<Product>> GetProductByIdAsync(Guid productId);
        Task<Return<Product>> AddProductAsync(Product product);
        // Task<Return<Product>> GetProductByName(string name);
        // Task<Return<IEnumerable<GetProductsResDto>>> GetProductsForCustomerAsync(string? keyword, string? sortBy,
        //     int pageNumber = 1, int pageSize = 10);
        Task<Return<Product>> UpdateProductAsync(Product product);
        Task<Return<Product>> DeleteProductAsync(Guid productId);
        Task<Return<List<ProductCategory>>> GetProductCategoriesAsync(Guid productId);
        Task<Return<List<ProductCategory>>> AddProductCategoriesAsync(List<ProductCategory> productCategories);
        Task<Return<ProductCategory>> DeleteProductCategoryAsync(Guid productId, List<Guid> categoryIds);
        Task<Return<List<ProductImage>>> AddProductImagesAsync(List<ProductImage> productImages);
        Task<Return<ProductImage>> AddProductImageAsync(ProductImage productImage);
        Task<Return<ProductImage>> UpdateProductImageAsync(ProductImage productImage);
        Task<Return<ProductImage>> DeleteProductImageAsync(Guid productId, Guid imageId);
        Task<Return<List<ProductAttribute>>> AddProductAttributesAsync(List<ProductAttribute> productAttributes);
        Task<Return<ProductAttribute>> AddProductAttributeAsync(ProductAttribute productAttribute);
        Task<Return<ProductAttribute>> UpdateProductAttributeAsync(ProductAttribute productAttributes);
        Task<Return<ProductAttribute>> DeleteProductAttributeAsync(Guid productId, Guid attributeId);
    }
}
