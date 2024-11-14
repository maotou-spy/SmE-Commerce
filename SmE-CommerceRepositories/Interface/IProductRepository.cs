using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceModels.ResponseDtos.Product;

namespace SmE_CommerceRepositories.Interface
{
    public interface IProductRepository
    {
        Task<Return<Product>> AddProductAsync(Product product);
        Task<Return<Product>> GetProductByName(string name);

        Task<Return<IEnumerable<GetProductsResDto>>> GetProductsForCustomerAsync(string? keyword, string? sortBy,
            int pageNumber = 1, int pageSize = 10);
    }
}
