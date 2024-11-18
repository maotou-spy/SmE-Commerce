using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface
{
    public interface IProductRepository
    {
        Task<Return<Product>> GetProductByIdAsync(Guid productId);
        Task<Return<Product>> AddProductAsync(Product product);
        // Task<Return<IEnumerable<GetProductsResDto>>> GetProductsForCustomerAsync(string? keyword, string? sortBy,
        //     int pageNumber = 1, int pageSize = 10);
        Task<Return<Product>> UpdateProductAsync(Product product);
        Task<Return<List<ProductCategory>>> GetProductCategoriesAsync(Guid productId);
        Task<Return<List<ProductCategory>>> AddProductCategoriesAsync(List<ProductCategory> productCategories);
        Task<Return<ProductCategory>> DeleteProductCategoryAsync(Guid productId, List<Guid> categoryIds);
        Task<Return<ProductImage>> GetProductImageByIdAsync(Guid productImageId);
        Task<Return<ProductImage>> AddProductImageAsync(ProductImage productImage);
        Task<Return<ProductImage>> UpdateProductImageAsync(ProductImage productImage);
        Task<Return<ProductImage>> DeleteProductImageAsync(Guid productImageId);
        Task<Return<ProductAttribute>> GetProductAttributeByIdAsync(Guid attributeId);
        Task<Return<ProductAttribute>> AddProductAttributeAsync(ProductAttribute productAttribute);
        Task<Return<ProductAttribute>> UpdateProductAttributeAsync(ProductAttribute productAttributes);
        Task<Return<ProductAttribute>> DeleteProductAttributeAsync(Guid attributeId);
    }
}
