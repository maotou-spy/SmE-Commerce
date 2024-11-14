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
        #region Product

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

        public Task<Return<Product>> GetProductByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<Return<IEnumerable<GetProductsResDto>>> GetProductsForCustomerAsync(string? keyword, string? sortBy, int pageNumber = PagingEnum.PageNumber, int pageSize = PagingEnum.PageSize)
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

        #endregion

        #region Product Attribute

        public async Task<Return<List<ProductAttribute>>> AddProductAttributesAsync(List<ProductAttribute> productAttributes)
        {
            try
            {
                await dbContext.ProductAttributes.AddRangeAsync(productAttributes);
                await dbContext.SaveChangesAsync();

                return new Return<List<ProductAttribute>>
                {
                    Data = productAttributes,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Created,
                    TotalRecord = productAttributes.Count
                };
            }
            catch (Exception ex)
            {
                return new Return<List<ProductAttribute>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<ProductAttribute>> AddProductAttributeAsync(ProductAttribute productAttribute)
        {
            try
            {
                await dbContext.ProductAttributes.AddAsync(productAttribute);
                await dbContext.SaveChangesAsync();

                return new Return<ProductAttribute>
                {
                    Data = productAttribute,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Created,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<ProductAttribute>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<ProductAttribute>> UpdateProductAttributeAsync(ProductAttribute productAttributes)
        {
            try
            {
                dbContext.ProductAttributes.Update(productAttributes);
                await dbContext.SaveChangesAsync();

                return new Return<ProductAttribute>
                {
                    Data = productAttributes,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Updated,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<ProductAttribute>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<ProductAttribute>> DeleteProductAttributeAsync(Guid productId, Guid attributeId)
        {
            try
            {
                var productAttribute = await dbContext.ProductAttributes
                    .FirstOrDefaultAsync(x => x.Productid == productId && x.Attributeid == attributeId);
                if (productAttribute is null)
                {
                    return new Return<ProductAttribute>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = ErrorMessage.NotFound,
                        TotalRecord = 0
                    };
                }

                dbContext.ProductAttributes.Remove(productAttribute);
                await dbContext.SaveChangesAsync();

                return new Return<ProductAttribute>
                {
                    Data = null,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Deleted,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<ProductAttribute>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        #endregion

        #region Product Category

        public async Task<Return<List<ProductCategory>>> GetProductCategoriesAsync(Guid productId)
        {
            try
            {
                var productCategories = await dbContext.ProductCategories
                    .Where(x => x.ProductId == productId)
                    .ToListAsync();

                return new Return<List<ProductCategory>>
                {
                    Data = productCategories,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Successfully,
                    TotalRecord = productCategories.Count
                };
            }
            catch (Exception ex)
            {
                return new Return<List<ProductCategory>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<List<ProductCategory>>> AddProductCategoriesAsync(List<ProductCategory> productCategories)
        {
            try
            {
                await dbContext.ProductCategories.AddRangeAsync(productCategories);
                await dbContext.SaveChangesAsync();

                return new Return<List<ProductCategory>>
                {
                    Data = productCategories,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Created,
                    TotalRecord = productCategories.Count
                };
            }
            catch (Exception ex)
            {
                return new Return<List<ProductCategory>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<ProductCategory>> DeleteProductCategoryAsync(Guid productId, List<Guid> categoryIds)
        {
            try
            {
                var productCategories = await dbContext.ProductCategories
                    .Where(x => x.ProductId == productId && categoryIds.Contains(x.CategoryId))
                    .ToListAsync();

                dbContext.ProductCategories.RemoveRange(productCategories);
                await dbContext.SaveChangesAsync();

                return new Return<ProductCategory>
                {
                    Data = null,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Deleted,
                    TotalRecord = productCategories.Count
                };
            }
            catch (Exception ex)
            {
                return new Return<ProductCategory>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        #endregion

        #region Product Image

        public async Task<Return<List<ProductImage>>> AddProductImagesAsync(List<ProductImage> productImages)
        {
            try
            {
                await dbContext.ProductImages.AddRangeAsync(productImages);
                await dbContext.SaveChangesAsync();

                return new Return<List<ProductImage>>
                {
                    Data = productImages,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Created,
                    TotalRecord = productImages.Count
                };
            }
            catch (Exception ex)
            {
                return new Return<List<ProductImage>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<ProductImage>> AddProductImageAsync(ProductImage productImage)
        {
            try
            {
                await dbContext.ProductImages.AddAsync(productImage);
                await dbContext.SaveChangesAsync();

                return new Return<ProductImage>
                {
                    Data = productImage,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Created,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<ProductImage>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        #endregion

    }
}
