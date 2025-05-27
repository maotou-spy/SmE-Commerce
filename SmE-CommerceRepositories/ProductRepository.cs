using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceUtilities.Utils;

namespace SmE_CommerceRepositories;

public class ProductRepository(SmECommerceContext dbContext) : IProductRepository
{
    #region Product

    public async Task<Return<List<Product>>> GetProductsAsync(
        ProductFilterReqDto filter,
        bool isAdmin = false
    )
    {
        try
        {
            // Step 1: Build a query with only necessary fields
            var query = dbContext.Set<Product>().AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchTerm = VietnameseStringNormalizer
                    .Normalize(filter.SearchTerm)
                    .ToLower()
                    .Trim();

                // Use EF.Functions.ToTsVector for full-text search (PostgreSQL)
                query = query.Where(p =>
                    p.Name.Contains(searchTerm)
                    || EF.Functions.ToTsVector("simple", p.NameUnaccent).Matches(searchTerm)
                );
            }

            if (!isAdmin)
                query = query.Where(p => p.Status == ProductStatus.Active);
            else if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(p => p.Status == filter.Status);

            if (filter.CategoryId.HasValue)
                query = query.Where(p =>
                    p.ProductCategories.Any(pc => pc.CategoryId == filter.CategoryId.Value)
                );

            if (filter.MinPrice.HasValue)
                query = query.Where(p =>
                    p.HasVariant
                        ? p.ProductVariants.Any(v => v.Price >= filter.MinPrice.Value)
                        : p.Price >= filter.MinPrice.Value
                );

            if (filter.MaxPrice.HasValue)
                query = query.Where(p =>
                    p.HasVariant
                        ? p.ProductVariants.Any(v => v.Price <= filter.MaxPrice.Value)
                        : p.Price <= filter.MaxPrice.Value
                );

            if (filter.MinRating.HasValue)
                query = query.Where(p => p.AverageRating >= filter.MinRating.Value);

            // Apply sorting
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var sortBy = filter.SortBy.ToLower();
                var sortOrder = filter.SortOrder.ToLower();

                // Default to Ascending if sortOrder is invalid
                if (
                    sortOrder != FilterSortOrder.Ascending
                    && sortOrder != FilterSortOrder.Descending
                )
                    sortOrder = FilterSortOrder.Ascending;

                query = sortBy switch
                {
                    ProductFilterSortBy.Price => sortOrder == FilterSortOrder.Descending
                        ? query.OrderByDescending(p =>
                            p.HasVariant ? p.ProductVariants.Min(v => v.Price) : p.Price
                        )
                        : query.OrderBy(p =>
                            p.HasVariant ? p.ProductVariants.Min(v => v.Price) : p.Price
                        ),
                    ProductFilterSortBy.AverageRating => sortOrder == FilterSortOrder.Descending
                        ? query.OrderByDescending(p => p.AverageRating)
                        : query.OrderBy(p => p.AverageRating),
                    ProductFilterSortBy.SoldQuantity => sortOrder == FilterSortOrder.Descending
                        ? query.OrderByDescending(p => p.SoldQuantity)
                        : query.OrderBy(p => p.SoldQuantity),
                    _ => query.OrderBy(p => p.Name),
                };
            }
            else
            {
                query = query.OrderBy(p => p.Name); // Default sorting
            }

            // Apply pagination
            var totalCount = await query.CountAsync();
            var items = await query
                .Select(p => new Product
                {
                    ProductId = p.ProductId,
                    ProductCode = p.ProductCode,
                    Name = p.Name,
                    NameUnaccent = p.NameUnaccent,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    SoldQuantity = p.SoldQuantity,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    CreateById = p.CreateById,
                    CreateBy =
                        p.CreateBy != null ? new User { FullName = p.CreateBy.FullName } : null,
                    ModifiedAt = p.ModifiedAt,
                    ModifiedById = p.ModifiedById,
                    ModifiedBy =
                        p.ModifiedBy != null ? new User { FullName = p.ModifiedBy.FullName } : null,
                    IsTopSeller = p.IsTopSeller,
                    PrimaryImage = p.PrimaryImage,
                    AverageRating = p.AverageRating,
                })
                // .Include(x => x.CreateBy)
                // .Include(x => x.ModifiedBy)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            // Step 3: Return result
            return new Return<List<Product>>
            {
                Data = items,
                IsSuccess = true,
                StatusCode = totalCount > 0 ? ErrorCode.Ok : ErrorCode.ProductNotFound,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecord = totalCount,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<Product>>
            {
                Data = [],
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<Product>> GetProductByIdAsync(Guid productId)
    {
        try
        {
            var product = await dbContext
                .Products
                // .Where(x => x.Status != Status.Deleted)
                .Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.ProductAttributes)
                .Include(x => x.ProductVariants)
                .Include(x => x.Reviews)
                .ThenInclude(x => x.User)
                .Include(x => x.CreateBy)
                .Include(x => x.ModifiedBy)
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (product is null)
                return new Return<Product>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound,
                    TotalRecord = 0,
                };

            return new Return<Product>
            {
                Data = product,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<Product>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<Product>> GetProductByIdForUpdateAsync(Guid productId)
    {
        try
        {
            var product = await dbContext
                .Products.Where(x => x.Status != ProductStatus.Deleted)
                .Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.ProductAttributes)
                .Include(x => x.ProductVariants)
                .ThenInclude(x => x.VariantAttributes)
                .Include(x => x.CreateBy)
                .Include(x => x.ModifiedBy)
                .FirstOrDefaultAsync(x => x.ProductId == productId);

            if (product is null)
                return new Return<Product>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound,
                    TotalRecord = 0,
                };

            await dbContext.Database.ExecuteSqlRawAsync(
                "SELECT * FROM public.\"Products\" WHERE public.\"Products\".\"productId\" = {0} FOR UPDATE",
                productId
            );

            return new Return<Product>
            {
                Data = product,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<Product>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<IEnumerable<Product>>> GetRelatedProductsAsync(
        Guid productId,
        int limit = 5
    )
    {
        try
        {
            // Get category IDs for the product
            var categoryIds = await dbContext
                .ProductCategories.Where(pc => pc.ProductId == productId)
                .Select(pc => pc.CategoryId)
                .ToListAsync();

            // Get related products based on the same categories, excluding the current product
            var relatedProducts = await dbContext
                .Products.Join(
                    dbContext.ProductCategories,
                    product => product.ProductId,
                    productCategory => productCategory.ProductId,
                    (product, productCategory) =>
                        new { Product = product, ProductCategory = productCategory }
                )
                .Where(x =>
                    categoryIds.Contains(x.ProductCategory.CategoryId)
                    && x.Product.ProductId != productId
                    && x.Product.Status == ProductStatus.Active
                )
                .Select(x => x.Product)
                .Distinct()
                .Take(limit)
                .ToListAsync();

            // Fallback to top-selling products if not enough related products
            if (relatedProducts.Count >= limit)
                return new Return<IEnumerable<Product>>
                {
                    Data = relatedProducts,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                    TotalRecord = relatedProducts.Count,
                };

            var additionalProducts = await dbContext
                .Products.Where(p =>
                    p.ProductId != productId
                    && p.Status == ProductStatus.Active
                    && p.IsTopSeller
                    && !relatedProducts.Select(rp => rp.ProductId).Contains(p.ProductId)
                )
                .Take(limit - relatedProducts.Count)
                .ToListAsync();

            relatedProducts.AddRange(additionalProducts);

            return new Return<IEnumerable<Product>>
            {
                Data = relatedProducts,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = relatedProducts.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<Product>>
            {
                Data = [],
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<Product>> GetProductByVariantIdForUpdateAsync(Guid productVariantId)
    {
        try
        {
            var product = await dbContext
                .Products.Include(x => x.ProductVariants)
                .Where(x => x.Status != ProductStatus.Deleted)
                .FirstOrDefaultAsync(x =>
                    x.ProductVariants.Any(y => y.ProductVariantId == productVariantId)
                );

            if (product is null)
                return new Return<Product>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound,
                    TotalRecord = 0,
                };

            await dbContext.Database.ExecuteSqlRawAsync(
                "SELECT * FROM public.\"Products\" WHERE public.\"Products\".\"productId\" = {0} FOR UPDATE",
                product.ProductId
            );

            return new Return<Product>
            {
                Data = product,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<Product>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
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
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<Product>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<Product>> GetProductByNameAsync(string productName)
    {
        try
        {
            var product = await dbContext
                .Products.Where(x => x.Status != ProductStatus.Deleted)
                .FirstOrDefaultAsync(x => x.Name == productName);

            if (product is null)
                return new Return<Product>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound,
                    TotalRecord = 0,
                };

            return new Return<Product>
            {
                Data = product,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<Product>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

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
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<Product>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    #endregion

    #region Product Attribute

    public async Task<Return<ProductAttribute>> GetProductAttributeByIdAsync(Guid attributeId)
    {
        try
        {
            var productAttribute = await dbContext.ProductAttributes.FirstOrDefaultAsync(x =>
                x.AttributeId == attributeId
            );

            if (productAttribute is null)
                return new Return<ProductAttribute>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductAttributeNotFound,
                    TotalRecord = 0,
                };

            return new Return<ProductAttribute>
            {
                Data = productAttribute,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<ProductAttribute>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<ProductAttribute>> AddProductAttributeAsync(
        ProductAttribute productAttribute
    )
    {
        try
        {
            await dbContext.ProductAttributes.AddAsync(productAttribute);
            await dbContext.SaveChangesAsync();

            return new Return<ProductAttribute>
            {
                Data = productAttribute,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<ProductAttribute>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<ProductAttribute>> UpdateProductAttributeAsync(
        ProductAttribute productAttributes
    )
    {
        try
        {
            dbContext.ProductAttributes.Update(productAttributes);
            await dbContext.SaveChangesAsync();

            return new Return<ProductAttribute>
            {
                Data = productAttributes,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<ProductAttribute>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<ProductAttribute>> DeleteProductAttributeAsync(Guid attributeId)
    {
        try
        {
            var productAttribute = await dbContext.ProductAttributes.FirstOrDefaultAsync(x =>
                x.AttributeId == attributeId
            );
            if (productAttribute is null)
                return new Return<ProductAttribute>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductAttributeNotFound,
                    TotalRecord = 0,
                };

            dbContext.ProductAttributes.Remove(productAttribute);
            await dbContext.SaveChangesAsync();

            return new Return<ProductAttribute>
            {
                Data = null,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<ProductAttribute>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    #endregion

    #region Product Category

    public async Task<Return<List<ProductCategory>>> GetProductCategoriesAsync(Guid productId)
    {
        try
        {
            var productCategories = await dbContext
                .ProductCategories.Where(x => x.ProductId == productId)
                .ToListAsync();

            return new Return<List<ProductCategory>>
            {
                Data = productCategories,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = productCategories.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<ProductCategory>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<List<ProductCategory>>> AddProductCategoriesAsync(
        List<ProductCategory> productCategories
    )
    {
        try
        {
            await dbContext.ProductCategories.AddRangeAsync(productCategories);
            await dbContext.SaveChangesAsync();

            return new Return<List<ProductCategory>>
            {
                Data = productCategories,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = productCategories.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<ProductCategory>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<ProductCategory>> DeleteProductCategoryAsync(
        Guid productId,
        List<Guid> categoryIds
    )
    {
        try
        {
            var productCategories = await dbContext
                .ProductCategories.Where(x =>
                    x.ProductId == productId && categoryIds.Contains(x.CategoryId)
                )
                .ToListAsync();

            dbContext.ProductCategories.RemoveRange(productCategories);
            await dbContext.SaveChangesAsync();

            return new Return<ProductCategory>
            {
                Data = null,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = productCategories.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<ProductCategory>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,

                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    #endregion

    #region Product Image

    public async Task<Return<List<ProductImage>>> GetProductImagesAsync(Guid productId)
    {
        try
        {
            var productImages = await dbContext
                .ProductImages.Where(x => x.ProductId == productId)
                .ToListAsync();

            return new Return<List<ProductImage>>
            {
                Data = productImages,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = productImages.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<ProductImage>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<ProductImage>> GetProductImageByIdAsync(Guid productImageId)
    {
        try
        {
            var productImage = await dbContext.ProductImages.FirstOrDefaultAsync(x =>
                x.ImageId == productImageId
            );

            if (productImage is null)
                return new Return<ProductImage>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductImageNotFound,
                    TotalRecord = 0,
                };

            return new Return<ProductImage>
            {
                Data = productImage,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<ProductImage>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,

                InternalErrorMessage = ex,
                TotalRecord = 0,
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
                StatusCode = ErrorCode.Ok,

                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<ProductImage>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,

                InternalErrorMessage = ex,
                TotalRecord = 0,
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
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<ProductImage>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,

                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<ProductImage>> DeleteProductImageAsync(Guid productImageId)
    {
        try
        {
            var productImage = await dbContext.ProductImages.FirstOrDefaultAsync(x =>
                x.ImageId == productImageId
            );
            if (productImage is null)
                return new Return<ProductImage>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductImageNotFound,
                    TotalRecord = 0,
                };

            dbContext.ProductImages.Remove(productImage);
            await dbContext.SaveChangesAsync();

            return new Return<ProductImage>
            {
                Data = null,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<ProductImage>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,

                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    #endregion

    #region Product Variant

    public async Task<Return<ProductVariant>> GetProductVariantByIdAsync(Guid productVariantId)
    {
        try
        {
            var productVariant = await dbContext
                .ProductVariants.Include(x => x.Product)
                .Include(x => x.VariantAttributes)
                .FirstOrDefaultAsync(x => x.ProductVariantId == productVariantId);

            if (productVariant is null)
                return new Return<ProductVariant>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductVariantNotFound,
                    TotalRecord = 0,
                };

            return new Return<ProductVariant>
            {
                Data = productVariant,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<ProductVariant>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<bool>> AddProductVariantAsync(ProductVariant productVariant)
    {
        try
        {
            await dbContext.ProductVariants.AddAsync(productVariant);
            await dbContext.SaveChangesAsync();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<ProductVariant>> GetProductVariantByIdForUpdateAsync(
        Guid? productVariantId
    )
    {
        try
        {
            var productVariant = await dbContext.ProductVariants.FirstOrDefaultAsync(x =>
                x.ProductVariantId == productVariantId
            );

            if (productVariant is null)
                return new Return<ProductVariant>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductVariantNotFound,
                    TotalRecord = 0,
                };

            if (productVariantId != null)
                await dbContext.Database.ExecuteSqlRawAsync(
                    "SELECT * FROM public.\"ProductVariants\" WHERE public.\"ProductVariants\".\"productVariantId\" = {0} FOR UPDATE",
                    productVariantId
                );

            return new Return<ProductVariant>
            {
                Data = productVariant,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<ProductVariant>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<bool>> UpdateProductVariantAsync(ProductVariant productVariant)
    {
        try
        {
            dbContext.ProductVariants.Update(productVariant);
            await dbContext.SaveChangesAsync();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    #endregion

    #region Variant Attribute

    public async Task<Return<List<ProductVariant>>> GetProductVariantsByProductIdAsync(
        Guid productId
    )
    {
        try
        {
            var productVariants = await dbContext
                .ProductVariants.Where(x => x.ProductId == productId)
                .Include(x => x.VariantAttributes)
                .ToListAsync();

            return new Return<List<ProductVariant>>
            {
                Data = productVariants,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = productVariants.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<ProductVariant>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    public async Task<Return<bool>> BulkAddVariantAttributeAsync(
        List<VariantAttribute> variantAttributes
    )
    {
        try
        {
            await dbContext.VariantAttributes.AddRangeAsync(variantAttributes);
            await dbContext.SaveChangesAsync();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = variantAttributes.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0,
            };
        }
    }

    #endregion
}
