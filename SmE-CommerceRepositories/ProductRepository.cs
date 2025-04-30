using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories;

public class ProductRepository(SmECommerceContext dbContext) : IProductRepository
{
    #region Product

    public async Task<Return<List<Product>>> GetProductsAsync(
        string keyword = "",
        int pageSize = 20,
        int page = 1,
        string status = ProductStatus.Active
    )
    {
        try
        {
            if (page < 1)
                page = 1;

            // Limit page size in range [1, 100]
            if (pageSize < 1)
                pageSize = 20;
            if (pageSize > 100)
                pageSize = 100;

            var query = dbContext.Products.Where(x => x.Status == status).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword.Trim()));

            var totalRecords = await query.CountAsync();

            var products = await query
                .Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Return<List<Product>>
            {
                Data = products,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = totalRecords,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<Product>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
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
                .Include(x => x.CreateBy)
                .Include(x => x.ModifiedBy)
                .Select(x => new Product
                {
                    ProductId = x.ProductId,
                    ProductCategories = x.ProductCategories,
                    ProductImages = x.ProductImages,
                    ProductAttributes = x.ProductAttributes,
                    ProductVariants = x.ProductVariants,
                    CreateBy = x.CreateBy,
                    ModifiedBy = x.ModifiedBy,
                    Reviews = x
                        .Reviews.Where(review => review.Status == GeneralStatus.Active)
                        .OrderBy(review => review.IsTop)
                        .Take(5)
                        .ToList(),
                })
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

    public async Task<Return<List<Product>>> GetProductsByIdsAsync(List<Guid> productIds)
    {
        try
        {
            var products = await dbContext
                .Products.Where(x => productIds.Contains(x.ProductId))
                .Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.ProductAttributes)
                .Include(x => x.ProductVariants)
                .ThenInclude(x => x.VariantAttributes)
                .ToListAsync();

            return new Return<List<Product>>
            {
                Data = products,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = products.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<Product>>
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
