using System.Transactions;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceModels.ResponseDtos;
using SmE_CommerceModels.ResponseDtos.Product;
using SmE_CommerceModels.ResponseDtos.Product.Manager;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;
using SmE_CommerceUtilities;

namespace SmE_CommerceServices;

public class ProductService(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IVariantNameRepository variantNameRepository,
    IHelperService helperService
) : IProductService
{
    #region Product

    public async Task<Return<GetProductDetailsResDto>> CustomerGetProductDetailsAsync(
        Guid productId
    )
    {
        try
        {
            var result = await productRepository.GetProductByIdAsync(productId);
            if (!result.IsSuccess || result.Data is not { Status: ProductStatus.Active })
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage,
                };
            if (result.Data.Status != ProductStatus.Active)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound,
                };

            return new Return<GetProductDetailsResDto>
            {
                Data = new GetProductDetailsResDto
                {
                    ProductId = result.Data.ProductId,
                    ProductCode = result.Data.ProductCode,
                    Name = result.Data.Name,
                    PrimaryImage = result.Data.PrimaryImage,
                    Description = result.Data.Description,
                    Price = result.Data.Price,
                    StockQuantity = result.Data.StockQuantity,
                    SoldQuantity = result.Data.SoldQuantity,
                    IsTopSeller = result.Data.IsTopSeller,
                    SeoMetadata = new SeoMetadata
                    {
                        Slug = result.Data.Slug,
                        MetaTitle = result.Data.MetaTitle,
                        MetaDescription = result.Data.MetaDescription,
                    },
                    Status = result.Data.Status,
                    Images = result
                        .Data.ProductImages.Select(image => new GetProductImageResDto
                        {
                            ImageId = image.ImageId,
                            Url = image.Url,
                            AltText = image.AltText,
                        })
                        .ToList(),
                    Categories = result
                        .Data.ProductCategories.Select(category => new GetProductCategoryResDto
                        {
                            CategoryId = category.CategoryId,
                            Name = category.Category.Name,
                        })
                        .ToList(),
                    Attributes = result
                        .Data.ProductAttributes.Select(attribute => new GetProductAttributeResDto
                        {
                            AttributeId = attribute.AttributeId,
                            Name = attribute.AttributeName,
                            Value = attribute.AttributeValue,
                        })
                        .ToList(),
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductDetailsResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<ManagerGetProductDetailResDto>> ManagerGetProductDetailsAsync(
        Guid productId
    )
    {
        try
        {
            var currentManager = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentManager.IsSuccess || currentManager.Data == null)
                return new Return<ManagerGetProductDetailResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentManager.StatusCode,
                    InternalErrorMessage = currentManager.InternalErrorMessage,
                };

            var result = await productRepository.GetProductByIdAsync(productId);
            if (!result.IsSuccess || result.Data == null)
                return new Return<ManagerGetProductDetailResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound,
                };

            return new Return<ManagerGetProductDetailResDto>
            {
                Data = new ManagerGetProductDetailResDto
                {
                    ProductId = result.Data.ProductId,
                    ProductCode = result.Data.ProductCode,
                    Name = result.Data.Name,
                    PrimaryImage = result.Data.PrimaryImage,
                    Description = result.Data.Description,
                    Price = result.Data.Price,
                    StockQuantity = result.Data.StockQuantity,
                    SoldQuantity = result.Data.SoldQuantity,
                    IsTopSeller = result.Data.IsTopSeller,
                    SeoMetadata = new SeoMetadata
                    {
                        Slug = result.Data.Slug,
                        MetaTitle = result.Data.MetaTitle,
                        MetaDescription = result.Data.MetaDescription,
                    },
                    Status = result.Data.Status,
                    Images = result
                        .Data.ProductImages.Select(image => new GetProductImageResDto
                        {
                            ImageId = image.ImageId,
                            Url = image.Url,
                            AltText = image.AltText,
                        })
                        .ToList(),
                    Categories = result
                        .Data.ProductCategories.Select(category => new GetProductCategoryResDto
                        {
                            CategoryId = category.CategoryId,
                            Name = category.Category.Name,
                        })
                        .ToList(),
                    Attributes = result
                        .Data.ProductAttributes.Select(attribute => new GetProductAttributeResDto
                        {
                            AttributeId = attribute.AttributeId,
                            Name = attribute.AttributeName,
                            Value = attribute.AttributeValue,
                        })
                        .ToList(),
                    AuditMetadata = new AuditMetadata
                    {
                        CreatedById = result.Data.CreateById,
                        CreatedAt = result.Data.CreatedAt,
                        CreatedBy = result.Data.CreateBy?.FullName,
                        ModifiedById = result.Data.ModifiedById,
                        ModifiedAt = result.Data.ModifiedAt,
                        ModifiedBy = result.Data.ModifiedBy?.FullName,
                    },
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<ManagerGetProductDetailResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<GetProductDetailsResDto>> AddProductAsync(AddProductReqDto req)
    {
        try
        {
            // Get the current user
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            // Check if the product data is valid
            if (req.Description == null || req.Price < 0 || req.StockQuantity < 0)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.BadRequest,
                };

            if (req.CategoryIds.Count == 0)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.BadRequest,
                };

            // Initialize the product
            var prdResult = await AddProductPrivateAsync(req, currentUser.Data.UserId);
            if (!prdResult.IsSuccess || prdResult.Data == null)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = prdResult.StatusCode,
                };

            return new Return<GetProductDetailsResDto>
            {
                Data = prdResult.Data,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductDetailsResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<GetProductDetailsResDto>> UpdateProductAsync(
        Guid productId,
        UpdateProductReqDto req
    )
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Get the current user
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var productResult = await productRepository.GetProductByIdForUpdateAsync(productId);
            if (!productResult.IsSuccess || productResult.Data == null)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = productResult.StatusCode,
                    InternalErrorMessage = productResult.InternalErrorMessage,
                };

            // Check if the product is deleted
            if (productResult.Data.Status == ProductStatus.Deleted)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound,
                };

            // Check if the product data is valid
            if (req.Description == null || req.Price < 0 || req.StockQuantity < 0)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.BadRequest,
                };

            // Check if the product name is unique except the current product
            var exitedProductResult = await productRepository.GetProductByNameAsync(req.Name);
            if (
                exitedProductResult is { IsSuccess: true, Data: not null }
                && exitedProductResult.Data.ProductId != productId
            )
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNameAlreadyExists,
                };

            productResult.Data.Name = req.Name.Trim();
            productResult.Data.Description = req.Description.Trim();
            productResult.Data.Price = req.Price < 0 ? 0 : req.Price;
            productResult.Data.StockQuantity = req.StockQuantity;
            productResult.Data.IsTopSeller = req.IsTopSeller;
            productResult.Data.Slug = SlugUtil.GenerateSlug(req.Name).Trim();
            productResult.Data.MetaTitle = (req.MetaTitle ?? req.Name).Trim();
            productResult.Data.MetaDescription = (req.MetaDescription ?? req.Description).Trim();
            productResult.Data.Status =
                req.StockQuantity > 0
                    ? req.Status != ProductStatus.Inactive
                        ? ProductStatus.Active
                        : ProductStatus.Inactive
                    : ProductStatus.OutOfStock;
            productResult.Data.ModifiedById = currentUser.Data.UserId;
            productResult.Data.ModifiedAt = DateTime.Now;

            // Update Product in the database
            var result = await productRepository.UpdateProductAsync(productResult.Data);
            if (!result.IsSuccess || result.Data == null)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                };

            // Mark the transaction as complete
            transaction.Complete();
            return new Return<GetProductDetailsResDto>
            {
                Data = new GetProductDetailsResDto
                {
                    ProductId = result.Data.ProductId,
                    ProductCode = result.Data.ProductCode,
                    Name = result.Data.Name,
                    PrimaryImage = result.Data.PrimaryImage,
                    Description = result.Data.Description,
                    Price = result.Data.Price,
                    StockQuantity = result.Data.StockQuantity,
                    SoldQuantity = result.Data.SoldQuantity,
                    IsTopSeller = result.Data.IsTopSeller,
                    Attributes = result
                        .Data.ProductAttributes.Select(attribute => new GetProductAttributeResDto
                        {
                            AttributeId = attribute.AttributeId,
                            Name = attribute.AttributeName,
                            Value = attribute.AttributeValue,
                        })
                        .ToList(),
                    Categories = result
                        .Data.ProductCategories.Select(category => new GetProductCategoryResDto
                        {
                            CategoryId = category.CategoryId,
                            Name = category.Category.Name,
                        })
                        .ToList(),
                    Images = result
                        .Data.ProductImages.Select(image => new GetProductImageResDto
                        {
                            ImageId = image.ImageId,
                            Url = image.Url,
                            AltText = image.AltText,
                        })
                        .ToList(),
                    SeoMetadata = new SeoMetadata
                    {
                        Slug = result.Data.Slug,
                        MetaTitle = result.Data.MetaTitle,
                        MetaDescription = result.Data.MetaDescription,
                    },
                    Status = result.Data.Status,
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductDetailsResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<bool>> DeleteProductAsync(Guid productId)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Get the current user
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            // Get the product
            var product = await productRepository.GetProductByIdForUpdateAsync(productId);
            if (!product.IsSuccess || product.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = product.StatusCode,
                    InternalErrorMessage = product.InternalErrorMessage,
                };
            // Check if the product is deleted
            if (product.Data.Status == ProductStatus.Deleted)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound,
                };

            // Update product status to deleted
            product.Data.Status = ProductStatus.Deleted;
            product.Data.ModifiedById = currentUser.Data.UserId;
            product.Data.ModifiedAt = DateTime.Now;

            var result = await productRepository.UpdateProductAsync(product.Data);
            if (!result.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage,
                };

            transaction.Complete();
            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
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
            };
        }
    }

    #endregion

    #region Product Category

    public async Task<Return<List<GetProductCategoryResDto>>> UpdateProductCategoryAsync(
        Guid productId,
        List<Guid> categoryIds
    )
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<List<GetProductCategoryResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            // Get current categories of the product
            var currentCategories = await productRepository.GetProductCategoriesAsync(productId);
            if (!currentCategories.IsSuccess)
                return new Return<List<GetProductCategoryResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentCategories.StatusCode,
                    InternalErrorMessage = currentCategories.InternalErrorMessage,
                };

            // Get current category ids
            var currentCategoryIds =
                currentCategories.Data?.Select(c => c.CategoryId).ToList() ?? [];

            // Get categories to add and remove
            var categoriesToAdd = categoryIds.Except(currentCategoryIds).ToList();
            var categoriesToRemove = currentCategoryIds.Except(categoryIds).ToList();

            // Add new categories
            if (categoriesToAdd.Count != 0)
            {
                var productCategories = categoriesToAdd
                    .Select(categoryId => new ProductCategory
                    {
                        ProductId = productId,
                        CategoryId = categoryId,
                    })
                    .ToList();

                var addResult = await productRepository.AddProductCategoriesAsync(
                    productCategories
                );
                if (!addResult.IsSuccess)
                    return new Return<List<GetProductCategoryResDto>>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = addResult.StatusCode,
                    };
            }

            // Delete categories
            if (categoriesToRemove.Count != 0)
            {
                var deleteResult = await productRepository.DeleteProductCategoryAsync(
                    productId,
                    categoriesToRemove
                );
                if (!deleteResult.IsSuccess)
                    return new Return<List<GetProductCategoryResDto>>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = deleteResult.StatusCode,
                        InternalErrorMessage = deleteResult.InternalErrorMessage,
                    };
            }

            // Get updated categories
            var updatedCategories = await productRepository.GetProductCategoriesAsync(productId);
            if (!updatedCategories.IsSuccess)
                return new Return<List<GetProductCategoryResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = updatedCategories.StatusCode,
                    InternalErrorMessage = updatedCategories.InternalErrorMessage,
                };

            transaction.Complete();
            return new Return<List<GetProductCategoryResDto>>
            {
                Data = updatedCategories
                    .Data?.Select(category => new GetProductCategoryResDto
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Category.Name,
                    })
                    .ToList(),
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<GetProductCategoryResDto>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    #endregion

    #region Product Image

    public async Task<Return<GetProductImageResDto>> AddProductImageAsync(
        Guid productId,
        AddProductImageReqDto req
    )
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<GetProductImageResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var productImage = new ProductImage
            {
                ProductId = productId,
                Url = req.Url,
                AltText = req.AltText,
            };

            var result = await productRepository.AddProductImageAsync(productImage);
            if (!result.IsSuccess || result.Data == null)
                return new Return<GetProductImageResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage,
                };

            return new Return<GetProductImageResDto>
            {
                Data = new GetProductImageResDto
                {
                    ImageId = result.Data.ImageId,
                    Url = result.Data.Url,
                    AltText = result.Data.AltText,
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductImageResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<GetProductImageResDto>> UpdateProductImageAsync(
        Guid productId,
        Guid imageId,
        AddProductImageReqDto req
    )
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<GetProductImageResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var productImageResult = await productRepository.GetProductImageByIdAsync(imageId);
            if (!productImageResult.IsSuccess || productImageResult.Data == null)
                return new Return<GetProductImageResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = productImageResult.StatusCode,
                    InternalErrorMessage = productImageResult.InternalErrorMessage,
                };
            // Check if the image belongs to the product
            if (productImageResult.Data.ProductId != productId)
                return new Return<GetProductImageResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductImageNotFound,
                };

            productImageResult.Data.Url = req.Url;
            productImageResult.Data.AltText = req.AltText;

            var result = await productRepository.UpdateProductImageAsync(productImageResult.Data);
            if (!result.IsSuccess || result.Data == null)
                return new Return<GetProductImageResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage,
                };

            return new Return<GetProductImageResDto>
            {
                Data = new GetProductImageResDto
                {
                    ImageId = result.Data.ImageId,
                    Url = result.Data.Url,
                    AltText = result.Data.AltText,
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductImageResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<bool>> DeleteProductImageAsync(Guid productId, Guid imageId)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var productImagesResult = await productRepository.GetProductImagesAsync(productId);
            if (!productImagesResult.IsSuccess || productImagesResult.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = productImagesResult.StatusCode,
                    InternalErrorMessage = productImagesResult.InternalErrorMessage,
                };
            // Check if the image belongs to the product
            if (productImagesResult.Data.All(i => i.ImageId != imageId))
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductImageNotFound,
                };
            // Check if only one image is left
            if (productImagesResult.Data.Count == 1)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductImageMinimum,
                };

            var result = await productRepository.DeleteProductImageAsync(imageId);
            if (!result.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage,
                };

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
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
            };
        }
    }

    #endregion

    #region Product Attribute

    public async Task<Return<GetProductAttributeResDto>> AddProductAttributeAsync(
        Guid productId,
        AddProductAttributeReqDto req
    )
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<GetProductAttributeResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var productAttribute = new ProductAttribute
            {
                ProductId = productId,
                AttributeName = req.AttributeName,
                AttributeValue = req.AttributeValue,
            };

            var result = await productRepository.AddProductAttributeAsync(productAttribute);
            if (!result.IsSuccess || result.Data == null)
                return new Return<GetProductAttributeResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage,
                };

            return new Return<GetProductAttributeResDto>
            {
                Data = new GetProductAttributeResDto
                {
                    AttributeId = result.Data.AttributeId,
                    Name = result.Data.AttributeName,
                    Value = result.Data.AttributeValue,
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductAttributeResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<GetProductAttributeResDto>> UpdateProductAttributeAsync(
        Guid productId,
        Guid attributeId,
        AddProductAttributeReqDto req
    )
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<GetProductAttributeResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var productAttribute = new ProductAttribute
            {
                ProductId = productId,
                AttributeId = attributeId,
                AttributeName = req.AttributeName,
                AttributeValue = req.AttributeValue,
            };

            var result = await productRepository.UpdateProductAttributeAsync(productAttribute);
            if (!result.IsSuccess || result.Data == null)
                return new Return<GetProductAttributeResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage,
                };

            return new Return<GetProductAttributeResDto>
            {
                Data = new GetProductAttributeResDto
                {
                    AttributeId = result.Data.AttributeId,
                    Name = result.Data.AttributeName,
                    Value = result.Data.AttributeValue,
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductAttributeResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<bool>> DeleteProductAttributeAsync(Guid productId, Guid attributeId)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var productAttributeResult = await productRepository.GetProductAttributeByIdAsync(
                attributeId
            );
            if (!productAttributeResult.IsSuccess || productAttributeResult.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = productAttributeResult.StatusCode,
                    InternalErrorMessage = productAttributeResult.InternalErrorMessage,
                };

            // Check if the attribute belongs to the product
            if (productAttributeResult.Data.ProductId != productId)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductAttributeNotFound,
                };

            var result = await productRepository.DeleteProductAttributeAsync(attributeId);
            if (!result.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage,
                };

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
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
            };
        }
    }

    #endregion

    #region private methods

    private async Task<Return<GetProductDetailsResDto>> AddProductPrivateAsync(
        AddProductReqDto req,
        Guid currentUserId
    )
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Check if the product name is unique
            var exitedProductResult = await productRepository.GetProductByNameAsync(req.Name);
            if (exitedProductResult.IsSuccess)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNameAlreadyExists,
                };

            // Only add active categories
            var categoryResult = await categoryRepository.GetCategoriesAsync(
                status: GeneralStatus.Active,
                name: null,
                pageNumber: null,
                pageSize: null
            );
            if (!categoryResult.IsSuccess)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = categoryResult.StatusCode,
                    InternalErrorMessage = categoryResult.InternalErrorMessage,
                };

            var categories = categoryResult
                .Data?.Where(c => req.CategoryIds.Contains(c.CategoryId))
                .ToList();
            if (categories == null)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CategoryNotFound,
                };

            // Initialize the product
            var product = new Product
            {
                Name = req.Name.Trim(),
                PrimaryImage = req.PrimaryImage,
                Description = req.Description?.Trim(),
                Price = req.Price < 0 ? 0 : req.Price,
                StockQuantity = req.StockQuantity,
                SoldQuantity = 0,
                IsTopSeller = req.IsTopSeller,
                Slug = SlugUtil.GenerateSlug(req.Name).Trim(),
                MetaTitle = (req.MetaTitle ?? req.Name).Trim(),
                MetaDescription = (req.MetaDescription ?? req.Description ?? req.Name).Trim(),
                Status =
                    req.StockQuantity > 0
                        ? req.Status != ProductStatus.Inactive
                            ? ProductStatus.Active
                            : ProductStatus.Inactive
                        : ProductStatus.OutOfStock,
                CreateById = currentUserId,
                CreatedAt = DateTime.Now,
                ProductCategories = categories
                    .Select(c => new ProductCategory { CategoryId = c.CategoryId })
                    .ToList(),
                ProductImages = req
                    .Images.Select(i => new ProductImage { Url = i.Url, AltText = i.AltText })
                    .ToList(),
                ProductAttributes = req
                    .Attributes.Select(a => new ProductAttribute
                    {
                        AttributeName = a.AttributeName,
                        AttributeValue = a.AttributeValue,
                    })
                    .ToList(),
            };

            // Add Product to the database
            var result = await productRepository.AddProductAsync(product);
            if (!result.IsSuccess || result.Data == null)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage,
                };

            transaction.Complete();

            return new Return<GetProductDetailsResDto>
            {
                Data = new GetProductDetailsResDto
                {
                    ProductId = product.ProductId,
                    ProductCode = product.ProductCode,
                    Name = product.Name,
                    PrimaryImage = result.Data.PrimaryImage,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    SoldQuantity = product.SoldQuantity,
                    IsTopSeller = product.IsTopSeller,
                    SeoMetadata = new SeoMetadata
                    {
                        Slug = result.Data.Slug,
                        MetaTitle = result.Data.MetaTitle,
                        MetaDescription = result.Data.MetaDescription,
                    },
                    Status = product.Status,
                    Images = result
                        .Data.ProductImages.Select(image => new GetProductImageResDto
                        {
                            ImageId = image.ImageId,
                            Url = image.Url,
                            AltText = image.AltText,
                        })
                        .ToList(),
                    Categories = result
                        .Data.ProductCategories.Select(category => new GetProductCategoryResDto
                        {
                            CategoryId = category.CategoryId,
                            Name = category.Category.Name,
                        })
                        .ToList(),
                    Attributes = result
                        .Data.ProductAttributes.Select(attribute => new GetProductAttributeResDto
                        {
                            AttributeId = attribute.AttributeId,
                            Name = attribute.AttributeName,
                            Value = attribute.AttributeValue,
                        })
                        .ToList(),
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception e)
        {
            return new Return<GetProductDetailsResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    private static string GetVariantKey(IEnumerable<dynamic> values)
    {
        return string.Join(
            "-",
            values.OrderBy(v => v.VariantNameId).Select(v => v.Value ?? v.VariantValue)
        );
    }

    #endregion

    #region Product Variation

    public async Task<Return<bool>> BulkProductVariantAsync(
        Guid productId,
        List<AddProductVariantReqDto> req
    )
    {
        if (req.Count == 0)
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.BadRequest,
            };

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            // Ensure all variants have the same number and type of attributes
            var expectedAttributes = req.First()
                .VariantValues.Select(v => v.VariantNameId)
                .ToHashSet();

            if (
                req.Any(x =>
                    !x
                        .VariantValues.Select(v => v.VariantNameId)
                        .ToHashSet()
                        .SetEquals(expectedAttributes)
                )
            )
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.BadRequest,
                };

            // Check existing variants for duplicates
            var existingVariants = await productRepository.GetProductVariantsByProductIdAsync(
                productId
            );
            if (existingVariants is { IsSuccess: true, Data.Count: > 0 })
            {
                var existingKeys = existingVariants
                    .Data.Select(x => GetVariantKey(x.VariantAttributes))
                    .ToHashSet();

                if (req.Any(x => existingKeys.Contains(GetVariantKey(x.VariantValues))))
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.ProductVariantAlreadyExists,
                    };
            }

            // Check for duplicate variants in request
            var uniqueKeys = new HashSet<string>();
            if (req.Any(x => !uniqueKeys.Add(GetVariantKey(x.VariantValues))))
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.BadRequest,
                };

            // Validate variant names and product in one go
            var variantNameIds = req.SelectMany(x => x.VariantValues.Select(v => v.VariantNameId))
                .Distinct()
                .ToList();

            var variantNames = await variantNameRepository.GetVariantNamesByIdsAsync(
                variantNameIds
            );
            if (!variantNames.IsSuccess || variantNames.Data?.Count != variantNameIds.Count)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.VariantNameNotFound,
                };

            var productResult = await productRepository.GetProductByIdAsync(productId);

            if (!productResult.IsSuccess || productResult.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound,
                    InternalErrorMessage = productResult.InternalErrorMessage,
                };
            if (
                !productResult.IsSuccess
                || productResult.Data == null
                || productResult.Data.Status == ProductStatus.Deleted
            )
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound,
                    InternalErrorMessage = productResult.InternalErrorMessage,
                };

            // Map product variants
            var productVariants = req.Select(x => new ProductVariant
                {
                    ProductId = productId,
                    // Sku = x.Sku,
                    Price = x.Price,
                    StockQuantity = x.StockQuantity,
                    VariantImage = x.VariantImage,
                    Status =
                        x.StockQuantity > 0
                            ? x.Status
                                ? ProductStatus.Active
                                : ProductStatus.Inactive
                            : ProductStatus.OutOfStock,
                    CreateById = currentUser.Data.UserId,
                    CreatedAt = DateTime.Now,
                })
                .ToList();

            // Save product variants
            var addResult = await productRepository.BulkAddProductVariantAsync(productVariants);
            if (!addResult.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = addResult.StatusCode,
                    InternalErrorMessage = addResult.InternalErrorMessage,
                };

            // Map and save variant attributes
            var variantAttributes = productVariants
                .Zip(
                    req,
                    (productVariant, x) =>
                        x.VariantValues.Select(v => new VariantAttribute
                        {
                            ProductVariantId = productVariant.ProductVariantId,
                            VariantNameId = v.VariantNameId,
                            Value = v.VariantValue,
                            CreatedById = currentUser.Data.UserId,
                            CreatedAt = DateTime.Now,
                        })
                )
                .SelectMany(attributes => attributes)
                .ToList();

            var addVariantAttributesResult = await productRepository.BulkAddVariantAttributeAsync(
                variantAttributes
            );
            if (!addVariantAttributesResult.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = addVariantAttributesResult.StatusCode,
                    InternalErrorMessage = addVariantAttributesResult.InternalErrorMessage,
                };

            transaction.Complete();
            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
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
            };
        }
    }

    // public async Task<Return<bool>> UpdateProductVariantAsync(
    //     Guid variantId,
    //     UpdateProductVariantReqDto req
    // )
    // {
    //     using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    //     try
    //     {
    //         var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
    //         if (!currentUser.IsSuccess || currentUser.Data == null)
    //             return new Return<bool>
    //             {
    //                 Data = false,
    //                 IsSuccess = false,
    //                 StatusCode = currentUser.StatusCode,
    //                 InternalErrorMessage = currentUser.InternalErrorMessage,
    //             };
    //
    //         var productVariant = await productRepository.GetProductVariantByIdAsync(variantId);
    //         if (!productVariant.IsSuccess || productVariant.Data == null)
    //             return new Return<bool>
    //             {
    //                 Data = false,
    //                 IsSuccess = false,
    //                 StatusCode = productVariant.StatusCode,
    //                 InternalErrorMessage = productVariant.InternalErrorMessage,
    //             };
    //
    //         if (productVariant.Data.Status == GeneralStatus.Deleted)
    //             return new Return<bool>
    //             {
    //                 Data = false,
    //                 IsSuccess = false,
    //                 StatusCode = ErrorCode.ProductVariantNotFound,
    //             };
    //
    //         // No change
    //         if (
    //             productVariant.Data.Sku == req.Sku
    //             && productVariant.Data.Price == req.Price
    //             && productVariant.Data.StockQuantity == req.StockQuantity
    //             && productVariant.Data.Status == req.Status
    //         )
    //             return new Return<bool>
    //             {
    //                 Data = true,
    //                 IsSuccess = true,
    //                 StatusCode = ErrorCode.Ok,
    //             };
    //
    //         var product = await productRepository.GetProductByIdAsync(
    //             productVariant.Data.ProductId
    //         );
    //         if (product is not { IsSuccess: true, Data: not null })
    //             return new Return<bool>
    //             {
    //                 Data = false,
    //                 IsSuccess = false,
    //                 StatusCode = product.StatusCode,
    //                 InternalErrorMessage = product.InternalErrorMessage,
    //             };
    //
    //         if (product.Data.Status == ProductStatus.Deleted)
    //             return new Return<bool>
    //             {
    //                 Data = false,
    //                 IsSuccess = false,
    //                 StatusCode = ErrorCode.ProductNotFound,
    //             };
    //
    //         var variant = await variantNameRepository.GetVariantNameByIdAsync(
    //             productVariant.Data.VariantNameId
    //         );
    //         if (!variant.IsSuccess || variant.Data == null)
    //             return new Return<bool>
    //             {
    //                 Data = false,
    //                 IsSuccess = false,
    //                 StatusCode = variant.StatusCode,
    //                 InternalErrorMessage = variant.InternalErrorMessage,
    //             };
    //
    //         var isVariantExisted = await productRepository.IsProductVariantExistAsync(
    //             req.VariantValue,
    //             productVariant.Data.ProductId
    //         );
    //         if (isVariantExisted is { IsSuccess: true, Data: true })
    //             return new Return<bool>
    //             {
    //                 Data = false,
    //                 IsSuccess = false,
    //                 StatusCode = ErrorCode.ProductVariantAlreadyExists,
    //             };
    //
    //         productVariant.Data.Sku = req.Sku;
    //         productVariant.Data.Price = req.Price;
    //         productVariant.Data.StockQuantity = req.StockQuantity;
    //         productVariant.Data.Status =
    //             req.StockQuantity > 0
    //                 ? req.Status ?? ProductStatus.Active
    //                 : ProductStatus.OutOfStock;
    //         productVariant.Data.ModifiedById = currentUser.Data.UserId;
    //         productVariant.Data.ModifiedAt = DateTime.Now;
    //
    //         var result = await productRepository.UpdateProductVariantAsync(productVariant.Data);
    //         if (!result.IsSuccess || result.Data == false)
    //             return new Return<bool>
    //             {
    //                 Data = false,
    //                 IsSuccess = false,
    //                 StatusCode = result.StatusCode,
    //                 InternalErrorMessage = result.InternalErrorMessage,
    //             };
    //
    //         transaction.Complete();
    //
    //         return new Return<bool>
    //         {
    //             Data = true,
    //             IsSuccess = true,
    //             StatusCode = ErrorCode.Ok,
    //         };
    //     }
    //     catch (Exception ex)
    //     {
    //         return new Return<bool>
    //         {
    //             Data = false,
    //             IsSuccess = false,
    //             StatusCode = ErrorCode.InternalServerError,
    //             InternalErrorMessage = ex,
    //         };
    //     }
    // }

    #endregion
}
