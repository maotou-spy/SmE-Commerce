using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ResponseDtos.Category;
using SmE_CommerceModels.ResponseDtos.Product;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories
{
    public class ProductRepository(SmECommerceContext dbContext) : IProductRepository
    {
        public async Task<Return<Product>> AddProductAsync(Product product)
        {
            try
            {
                await dbContext.Products.AddAsync(product);
                await dbContext.SaveChangesAsync();

                return new Return<Product>
                {
                    Data = product,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Created,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<Product>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<IEnumerable<GetProductsResDto>>> GetProductsForCustomerAsync(string? keyword,  string? sortBy, int pageNumber = PagingEnum.PageNumber, int pageSize = PagingEnum.PageSize)
        {
            try
            {
                var productsQuery = dbContext.Products
                    .Include(x => x.ProductCategories)
                        .ThenInclude(x => x.Category)
                    .Include(x => x.ProductImages)
                    .AsQueryable();

                // Search by keyword
                if (!string.IsNullOrEmpty(keyword))
                {
                    productsQuery = productsQuery.Where(x =>
                        x.Name.Contains(keyword) ||
                        x.ProductCategories.Any(pc => pc.Category != null && pc.Category.Name.Contains(keyword)));
                }

                // Sort by
                if (!string.IsNullOrEmpty(sortBy))
                {
                    productsQuery = sortBy switch
                    {
                        ProductFilterEnum.PriceLowToHigh => productsQuery.OrderBy(x => x.Price),
                        ProductFilterEnum.PriceHighToLow => productsQuery.OrderByDescending(x => x.Price),
                        ProductFilterEnum.NameAscending => productsQuery.OrderBy(x => x.Name),
                        ProductFilterEnum.NameDescending => productsQuery.OrderByDescending(x => x.Name),
                        _ => productsQuery.OrderByDescending(x => x.CreatedAt)
                    };
                }

                var totalRecords = await productsQuery.CountAsync();

                var products = await productsQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new GetProductsResDto
                    {
                        ProductId = x.ProductId,
                        ProductName = x.Name,
                        ProductPrice = x.Price,
                        ProductStock = x.StockQuantity,
                        ProductImage = x.ProductImages.FirstOrDefault()!.Url,
                        Categories = x.ProductCategories.Select(pc => new GetCategoryResDto
                        {
                            CategoryId = pc.Category!.CategoryId,
                            CategoryName = pc.Category.Name,
                            Description = pc.Category.Description,
                        }).ToList()
                    })
                    .ToListAsync();

                return new Return<IEnumerable<GetProductsResDto>>
                {
                    Data = products,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Successfully,
                    TotalRecord = totalRecords
                };
            }
            catch (Exception ex)
            {
                return new Return<IEnumerable<GetProductsResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }
        
        public async Task<Return<Product>> GetProductByName(string name)
        {
            try
            {
                var result = await dbContext.Products
                .Where(x => x.Status != GeneralStatus.Deleted)
                .FirstOrDefaultAsync(x => x.Name == name);

                return new Return<Product>
                {
                    Data = result,
                    IsSuccess = true,
                    Message = result != null ? SuccessfulMessage.Found : ErrorMessage.NotFound,
                    TotalRecord = result != null ? 1 : 0
                };
            }
            catch (Exception ex)
            {
                return new Return<Product>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

    }
}
