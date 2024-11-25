using Microsoft.EntityFrameworkCore;
using Npgsql;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories
{
    public class ProductRepository(SmECommerceContext dbContext) : IProductRepository
    {
        #region Product

        public async Task<Return<Product>> GetProductByIdAsync(Guid productId)
        {
            try
            {
                var product = await dbContext.Products
                    // .Where(x => x.Status != ProductStatus.Deleted)
                    .Include(x => x.ProductCategories)
                    .ThenInclude(x => x.Category)
                    .Include(x => x.ProductImages)
                    .Include(x => x.ProductAttributes)
                    .FirstOrDefaultAsync(x => x.ProductId == productId);

                if (product is null)
                {
                    return new Return<Product>
                    {
                        Data = null,
                        IsSuccess = false,
                        ErrorCode = ErrorCodes.ProductNotFound,
                        Message = ErrorMessage.ProductNotFound,
                        TotalRecord = 0
                    };
                }

                return new Return<Product>
                {
                    Data = product,
                    IsSuccess = true,
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Successfully,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<Product>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

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
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Created,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<Product>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<string>> GetProductSlugAsync(string slug)
        {
            try
            {
                var product = await dbContext.Products
                    .Where(x => x.Status != ProductStatus.Deleted)
                    .FirstOrDefaultAsync(x => x.Slug == slug);

                if (product is null)
                {
                    return new Return<string>
                    {
                        Data = null,
                        IsSuccess = false,
                        ErrorCode = ErrorCodes.ProductNotFound,
                        Message = ErrorMessage.ProductNotFound,
                        TotalRecord = 0
                    };
                }

                return new Return<string>
                {
                    Data = product.Slug,
                    IsSuccess = true,
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Successfully,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<string>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        // public async Task<Return<IEnumerable<GetProductsResDto>>> GetProductsForCustomerAsync(string? keyword, string? sortBy, int pageNumber = PagingEnum.PageNumber, int pageSize = PagingEnum.PageSize)
        // {
        //     try
        //     {
        //         var productsQuery = dbContext.Products
        //             .Include(x => x.ProductCategories)
        //                 .ThenInclude(x => x.Category)
        //             .Include(x => x.ProductImages)
        //             .AsQueryable();
        //
        //         // Search by keyword
        //         if (!string.IsNullOrEmpty(keyword))
        //         {
        //             productsQuery = productsQuery.Where(x =>
        //                 x.Name.Contains(keyword) ||
        //                 x.ProductCategories.Any(pc => pc.Category != null && pc.Category.Name.Contains(keyword)));
        //         }
        //
        //         // Sort by
        //         if (!string.IsNullOrEmpty(sortBy))
        //         {
        //             productsQuery = sortBy switch
        //             {
        //                 ProductFilterEnum.PriceLowToHigh => productsQuery.OrderBy(x => x.Price),
        //                 ProductFilterEnum.PriceHighToLow => productsQuery.OrderByDescending(x => x.Price),
        //                 ProductFilterEnum.NameAscending => productsQuery.OrderBy(x => x.Name),
        //                 ProductFilterEnum.NameDescending => productsQuery.OrderByDescending(x => x.Name),
        //                 _ => productsQuery.OrderByDescending(x => x.CreatedAt)
        //             };
        //         }
        //
        //         var totalRecords = await productsQuery.CountAsync();
        //
        //         var products = await productsQuery
        //             .Skip((pageNumber - 1) * pageSize)
        //             .Take(pageSize)
        //             .Select(x => new GetProductsResDto
        //             {
        //                 ProductId = x.ProductId,
        //                 ProductName = x.Name,
        //                 ProductPrice = x.Price,
        //                 ProductStock = x.StockQuantity,
        //                 ProductImage = x.ProductImages.FirstOrDefault()!.Url,
        //                 Categories = x.ProductCategories.Select(pc => new GetCategoryDetailResDto
        //                 {
        //                     CategoryId = pc.Category!.CategoryId,
        //                     CategoryName = pc.Category.Name,
        //                     Description = pc.Category.Description,
        //                 }).ToList()
        //             })
        //             .ToListAsync();
        //
        //         return new Return<IEnumerable<GetProductsResDto>>
        //         {
        //             Data = products,
        //             IsSuccess = true,
        //             Message = SuccessMessage.Successfully,
        //             TotalRecord = totalRecords
        //         };
        //     }
        //     catch (Exception ex)
        //     {
        //         return new Return<IEnumerable<GetProductsResDto>>
        //         {
        //             Data = null,
        //             IsSuccess = false,
        //             Message = ErrorMessage.InternalServerError,
        //             InternalErrorMessage = ex,
        //             TotalRecord = 0
        //         };
        //     }
        // }

        public async Task<Return<Product>> UpdateProductAsync(Product product)
        {
            try
            {
                dbContext.Products.Update(product);
                await dbContext.SaveChangesAsync();

                return new Return<Product>
                {
                    Data = product,
                    IsSuccess = true,
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Updated,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<Product>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        #endregion

        #region Product Attribute

        public async Task<Return<ProductAttribute>> GetProductAttributeByIdAsync(Guid attributeId)
        {
            try
            {
                var productAttribute = await dbContext.ProductAttributes
                    .FirstOrDefaultAsync(x => x.Attributeid == attributeId);

                if (productAttribute is null)
                {
                    return new Return<ProductAttribute>
                    {
                        Data = null,
                        IsSuccess = false,
                        ErrorCode = ErrorCodes.ProductAttributeNotFound,
                        Message = ErrorMessage.ProductAttributeNotFound,
                        TotalRecord = 0
                    };
                }

                return new Return<ProductAttribute>
                {
                    Data = productAttribute,
                    IsSuccess = true,
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Successfully,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<ProductAttribute>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
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
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Created,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<ProductAttribute>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
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
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Updated,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<ProductAttribute>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<ProductAttribute>> DeleteProductAttributeAsync(Guid attributeId)
        {
            try
            {
                var productAttribute = await dbContext.ProductAttributes
                    .FirstOrDefaultAsync(x => x.Attributeid == attributeId);
                if (productAttribute is null)
                {
                    return new Return<ProductAttribute>
                    {
                        Data = null,
                        IsSuccess = false,
                        ErrorCode = ErrorCodes.ProductAttributeNotFound,
                        Message = ErrorMessage.ProductAttributeNotFound,
                        TotalRecord = 0
                    };
                }

                dbContext.ProductAttributes.Remove(productAttribute);
                await dbContext.SaveChangesAsync();

                return new Return<ProductAttribute>
                {
                    Data = null,
                    IsSuccess = true,
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Deleted,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<ProductAttribute>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
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
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Successfully,
                    TotalRecord = productCategories.Count
                };
            }
            catch (Exception ex)
            {
                return new Return<List<ProductCategory>>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<List<ProductCategory>>> AddProductCategoriesAsync(
            List<ProductCategory> productCategories)
        {
            try
            {
                await dbContext.ProductCategories.AddRangeAsync(productCategories);
                await dbContext.SaveChangesAsync();

                return new Return<List<ProductCategory>>
                {
                    Data = productCategories,
                    IsSuccess = true,
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Created,
                    TotalRecord = productCategories.Count
                };
            }
            catch (Exception ex)
            {
                return new Return<List<ProductCategory>>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
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
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Deleted,
                    TotalRecord = productCategories.Count
                };
            }
            catch (Exception ex)
            {
                return new Return<ProductCategory>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        #endregion

        #region Product Image

        public async Task<Return<ProductImage>> GetProductImageByIdAsync(Guid productImageId)
        {
            try
            {
                var productImage = await dbContext.ProductImages
                    .FirstOrDefaultAsync(x => x.ImageId == productImageId);

                if (productImage is null)
                {
                    return new Return<ProductImage>
                    {
                        Data = null,
                        IsSuccess = false,
                        ErrorCode = ErrorCodes.ProductImageNotFound,
                        Message = ErrorMessage.ProductImageNotFound,
                        TotalRecord = 0
                    };
                }

                return new Return<ProductImage>
                {
                    Data = productImage,
                    IsSuccess = true,
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Successfully,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<ProductImage>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
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
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Created,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<ProductImage>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<ProductImage>> UpdateProductImageAsync(ProductImage productImage)
        {
            try
            {
                dbContext.ProductImages.Update(productImage);
                await dbContext.SaveChangesAsync();

                return new Return<ProductImage>
                {
                    Data = productImage,
                    IsSuccess = true,
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Updated,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<ProductImage>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<ProductImage>> DeleteProductImageAsync(Guid productImageId)
        {
            try
            {
                var productImage = await dbContext.ProductImages
                    .FirstOrDefaultAsync(x => x.ImageId == productImageId);
                if (productImage is null)
                {
                    return new Return<ProductImage>
                    {
                        Data = null,
                        IsSuccess = false,
                        ErrorCode = ErrorCodes.ProductImageNotFound,
                        Message = ErrorMessage.ProductImageNotFound,
                        TotalRecord = 0
                    };
                }

                dbContext.ProductImages.Remove(productImage);
                await dbContext.SaveChangesAsync();

                return new Return<ProductImage>
                {
                    Data = null,
                    IsSuccess = true,
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Deleted,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<ProductImage>
                {
                    Data = null,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.InternalServerError,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        #endregion
    }
}