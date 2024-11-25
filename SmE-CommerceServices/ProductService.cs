using System.Transactions;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Product;
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
    IHelperService helperService) : IProductService
{
    #region Product

    public async Task<Return<GetProductDetailsResDto>> CustomerGetProductDetailsAsync(Guid productId)
    {
        try
        {
            var result = await productRepository.GetProductByIdAsync(productId);
            if (!result.IsSuccess || result.Data == null || result.Data.Status != ProductStatus.Active)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound
                };
            if (result.Data.Status != ProductStatus.Active)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound
                };

            return new Return<GetProductDetailsResDto>
            {
                Data = new GetProductDetailsResDto
                {
                    ProductId = result.Data.ProductId,
                    ProductCode = result.Data.ProductCode ?? string.Empty,
                    Name = result.Data.Name,
                    Description = result.Data.Description,
                    Price = result.Data.Price,
                    StockQuantity = result.Data.StockQuantity,
                    SoldQuantity = result.Data.SoldQuantity,
                    IsTopSeller = result.Data.IsTopSeller,
                    Slug = result.Data.Slug,
                    MetaTitle = result.Data.MetaTitle,
                    MetaDescription = result.Data.MetaDescription,
                    Keywords = result.Data.Keywords,
                    Status = result.Data.Status,
                    Images = result.Data.ProductImages.Select(image => new GetProductImageResDto
                    {
                        ImageId = image.ImageId,
                        Url = image.Url,
                        AltText = image.AltText
                    }).ToList(),
                    Categories = result.Data.ProductCategories.Select(category => new GetProductCategoryResDto
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Category?.Name ?? string.Empty
                    }).ToList(),
                    Attributes = result.Data.ProductAttributes.Select(attribute => new GetProductAttributeResDto
                    {
                        AttributeId = attribute.Attributeid,
                        Name = attribute.Attributename,
                        Value = attribute.Attributevalue
                    }).ToList()
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductDetailsResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<ManagerGetProductDetailResDto>> ManagerGetProductDetailsAsync(Guid productId)
    {
        try
        {
            var currentManager = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentManager.IsSuccess || currentManager.Data == null)
                return new Return<ManagerGetProductDetailResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentManager.StatusCode
                };

            var result = await productRepository.GetProductByIdAsync(productId);
            if (!result.IsSuccess || result.Data == null)
                return new Return<ManagerGetProductDetailResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound
                };

            return new Return<ManagerGetProductDetailResDto>
            {
                Data = new ManagerGetProductDetailResDto
                {
                    ProductId = result.Data.ProductId,
                    ProductCode = result.Data.ProductCode ?? string.Empty,
                    Name = result.Data.Name,
                    Description = result.Data.Description,
                    Price = result.Data.Price,
                    StockQuantity = result.Data.StockQuantity,
                    SoldQuantity = result.Data.SoldQuantity,
                    IsTopSeller = result.Data.IsTopSeller,
                    Slug = result.Data.Slug,
                    MetaTitle = result.Data.MetaTitle,
                    MetaDescription = result.Data.MetaDescription,
                    Keywords = result.Data.Keywords,
                    Status = result.Data.Status,
                    Images = result.Data.ProductImages.Select(image => new GetProductImageResDto
                    {
                        ImageId = image.ImageId,
                        Url = image.Url,
                        AltText = image.AltText
                    }).ToList(),
                    Categories = result.Data.ProductCategories.Select(category => new GetProductCategoryResDto
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Category?.Name ?? string.Empty
                    }).ToList(),
                    Attributes = result.Data.ProductAttributes.Select(attribute => new GetProductAttributeResDto
                    {
                        AttributeId = attribute.Attributeid,
                        Name = attribute.Attributename,
                        Value = attribute.Attributevalue
                    }).ToList(),
                    CreatedAt = result.Data.CreatedAt,
                    CreatedBy = result.Data.CreateBy?.FullName,
                    ModifiedAt = result.Data.ModifiedAt,
                    ModifiedBy = result.Data.ModifiedBy?.FullName
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<ManagerGetProductDetailResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    // public async Task<Return<IEnumerable<GetProductsResDto>>> CustomerGetProductsAsync(string? keyword,
    //     string? sortBy, int pageNumber, int pageSize)
    // {
    //     try
    //     {
    //         var result = await productRepository.CustomerGetProductsAsync(keyword, sortBy, pageNumber, pageSize);
    //         if (!result.IsSuccess)
    //             return new Return<IEnumerable<GetProductsResDto>>
    //             {
    //                 Data = null,
    //                 IsSuccess = false
    //             };
    //
    //         return new Return<IEnumerable<GetProductsResDto>>
    //         {
    //             Data = result.Data,
    //             IsSuccess = true,
    //             TotalRecord = result.TotalRecord
    //         };
    //     }
    //     catch (Exception ex)
    //     {
    //         return new Return<IEnumerable<GetProductsResDto>>
    //         {
    //             Data = null,
    //             IsSuccess = false,
    //             InternalErrorMessage = ex
    //         };
    //     }
    // }

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
                    StatusCode = currentUser.StatusCode
                };

            // Check if the product data is valid
            if (req.Description == null || req.Price < 0 || req.StockQuantity < 0)
            {
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ValidationError
                };
            }

            if (req.CategoryIds.Count == 0)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ValidationError
                };

            if (req.Slug != null)
            {
                var productSlug = await productRepository.GetProductSlugAsync(req.Slug);
                if (productSlug.IsSuccess)
                    return new Return<GetProductDetailsResDto>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = ErrorCode.SlugAlreadyExists
                    };
            }

            // Initialize the product
            var prdResult = await AddProductPrivateAsync(req, currentUser.Data.UserId);
            if (!prdResult.IsSuccess || prdResult.Data == null)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = prdResult.StatusCode
                };

            return new Return<GetProductDetailsResDto>
            {
                Data = prdResult.Data,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            // Log exception details here as appropriate for debugging
            return new Return<GetProductDetailsResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<GetProductDetailsResDto>> UpdateProductAsync(Guid productId, UpdateProductReqDto req)
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
                    StatusCode = currentUser.StatusCode
                };

            var productResult = await productRepository.GetProductByIdAsync(productId);
            if (!productResult.IsSuccess || productResult.Data == null)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = productResult.StatusCode
                };
            // Check if the product is deleted
            if (productResult.Data.Status == ProductStatus.Deleted)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound
                };

            // Check if the product data is valid
            if (req.Description == null || req.Price < 0 || req.StockQuantity < 0)
            {
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ValidationError
                };
            }

            productResult.Data.Name = req.Name.Trim();
            productResult.Data.Description = req.Description.Trim();
            productResult.Data.Price = req.Price < 0 ? 0 : req.Price;
            productResult.Data.StockQuantity = req.StockQuantity;
            productResult.Data.SoldQuantity = req.SoldQuantity;
            productResult.Data.IsTopSeller = req.IsTopSeller;
            productResult.Data.Slug = (req.Slug ?? SlugUtil.GenerateSlug(req.Name)).Trim();
            productResult.Data.MetaTitle = (req.MetaTitle ?? req.Name).Trim();
            productResult.Data.MetaDescription = (req.MetaDescription ?? req.Description).Trim();
            if (req.MetaKeywords != null)
                productResult.Data.Keywords = req.MetaKeywords;
            productResult.Data.Status = req.StockQuantity > 0
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
                    StatusCode = result.StatusCode
                };

            return new Return<GetProductDetailsResDto>
            {
                Data = new GetProductDetailsResDto
                {
                    ProductId = result.Data.ProductId,
                    ProductCode = result.Data.ProductCode ?? string.Empty,
                    Name = result.Data.Name,
                    Description = result.Data.Description,
                    Price = result.Data.Price,
                    StockQuantity = result.Data.StockQuantity,
                    SoldQuantity = result.Data.SoldQuantity,
                    IsTopSeller = result.Data.IsTopSeller,
                    Attributes = result.Data.ProductAttributes.Select(attribute => new GetProductAttributeResDto
                    {
                        AttributeId = attribute.Attributeid,
                        Name = attribute.Attributename,
                        Value = attribute.Attributevalue
                    }).ToList(),
                    Categories = result.Data.ProductCategories.Select(category => new GetProductCategoryResDto
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Category?.Name ?? string.Empty
                    }).ToList(),
                    Images = result.Data.ProductImages.Select(image => new GetProductImageResDto
                    {
                        ImageId = image.ImageId,
                        Url = image.Url,
                        AltText = image.AltText
                    }).ToList(),
                    Slug = result.Data.Slug,
                    MetaTitle = result.Data.MetaTitle,
                    MetaDescription = result.Data.MetaDescription,
                    Keywords = result.Data.Keywords,
                    Status = result.Data.Status
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductDetailsResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<bool>> DeleteProductAsync(Guid productId)
    {
        try
        {
            // Get the current user
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode
                };

            // Get the product
            var product = await productRepository.GetProductByIdAsync(productId);
            if (!product.IsSuccess || product.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = product.StatusCode
                };
            // Check if the product is deleted
            if (product.Data.Status == ProductStatus.Deleted)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound
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
                    StatusCode = result.StatusCode
                };

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            // Log exception details here as appropriate for debugging
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    #endregion

    #region Product Category

    public async Task<Return<List<GetProductCategoryResDto>>> UpdateProductCategoryAsync(Guid productId,
        List<Guid> categoryIds)
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
                    StatusCode = currentUser.StatusCode
                };

            // Get current categories of the product
            var currentCategories = await productRepository.GetProductCategoriesAsync(productId);
            if (!currentCategories.IsSuccess)
                return new Return<List<GetProductCategoryResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentCategories.StatusCode
                };

            // Get current category ids
            var currentCategoryIds = currentCategories.Data?.Select(c => c.CategoryId).ToList() ?? new List<Guid>();

            // Get categories to add and remove
            var categoriesToAdd = categoryIds.Except(currentCategoryIds).ToList();
            var categoriesToRemove = currentCategoryIds.Except(categoryIds).ToList();

            // Add new categories
            if (categoriesToAdd.Count != 0)
            {
                var productCategories = categoriesToAdd.Select(categoryId => new ProductCategory
                {
                    ProductId = productId,
                    CategoryId = categoryId
                }).ToList();

                var addResult = await productRepository.AddProductCategoriesAsync(productCategories);
                if (!addResult.IsSuccess)
                    return new Return<List<GetProductCategoryResDto>>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = addResult.StatusCode
                    };
            }

            // Delete categories
            if (categoriesToRemove.Count != 0)
            {
                var deleteResult = await productRepository.DeleteProductCategoryAsync(productId, categoriesToRemove);
                if (!deleteResult.IsSuccess)
                    return new Return<List<GetProductCategoryResDto>>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = deleteResult.StatusCode
                    };
            }

            // Get updated categories
            var updatedCategories = await productRepository.GetProductCategoriesAsync(productId);
            if (!updatedCategories.IsSuccess)
                return new Return<List<GetProductCategoryResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = updatedCategories.StatusCode
                };

            return new Return<List<GetProductCategoryResDto>>
            {
                Data = updatedCategories.Data?.Select(category => new GetProductCategoryResDto
                {
                    CategoryId = category.CategoryId,
                    Name = category.Category?.Name ?? string.Empty
                }).ToList(),
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<List<GetProductCategoryResDto>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    #endregion

    #region Product Image

    public async Task<Return<GetProductImageResDto>> AddProductImageAsync(Guid productId, AddProductImageReqDto req)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<GetProductImageResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode
                };

            var productImage = new ProductImage
            {
                ProductId = productId,
                Url = req.Url,
                AltText = req.AltText
            };

            var result = await productRepository.AddProductImageAsync(productImage);
            if (!result.IsSuccess || result.Data == null)
                return new Return<GetProductImageResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode
                };

            return new Return<GetProductImageResDto>
            {
                Data = new GetProductImageResDto
                {
                    ImageId = result.Data.ImageId,
                    Url = result.Data.Url,
                    AltText = result.Data.AltText
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductImageResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<GetProductImageResDto>> UpdateProductImageAsync(Guid productId, Guid imageId,
        AddProductImageReqDto req)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<GetProductImageResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode
                };

            var productImageResult = await productRepository.GetProductImageByIdAsync(imageId);
            if (!productImageResult.IsSuccess || productImageResult.Data == null)
                return new Return<GetProductImageResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = productImageResult.StatusCode
                };
            // Check if the image belongs to the product
            if (productImageResult.Data.ProductId != productId)
                return new Return<GetProductImageResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductImageNotFound
                };

            productImageResult.Data.Url = req.Url;
            productImageResult.Data.AltText = req.AltText;

            var result = await productRepository.UpdateProductImageAsync(productImageResult.Data);
            if (!result.IsSuccess || result.Data == null)
                return new Return<GetProductImageResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode
                };

            return new Return<GetProductImageResDto>
            {
                Data = new GetProductImageResDto
                {
                    ImageId = result.Data.ImageId,
                    Url = result.Data.Url,
                    AltText = result.Data.AltText
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductImageResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
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
                    StatusCode = currentUser.StatusCode
                };

            var productImageResult = await productRepository.GetProductImageByIdAsync(imageId);
            if (!productImageResult.IsSuccess || productImageResult.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = productImageResult.StatusCode
                };
            // Check if the image belongs to the product
            if (productImageResult.Data.ProductId != productId)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductImageNotFound
                };

            var result = await productRepository.DeleteProductImageAsync(imageId);
            if (!result.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = result.StatusCode
                };

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    #endregion

    #region Product Attribute

    public async Task<Return<GetProductAttributeResDto>> AddProductAttributeAsync(Guid productId,
        AddProductAttributeReqDto req)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<GetProductAttributeResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode
                };

            var productAttribute = new ProductAttribute
            {
                Productid = productId,
                Attributename = req.AttributeName,
                Attributevalue = req.AttributeValue
            };

            var result = await productRepository.AddProductAttributeAsync(productAttribute);
            if (!result.IsSuccess || result.Data == null)
                return new Return<GetProductAttributeResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode
                };

            return new Return<GetProductAttributeResDto>
            {
                Data = new GetProductAttributeResDto
                {
                    AttributeId = result.Data.Attributeid,
                    Name = result.Data.Attributename,
                    Value = result.Data.Attributevalue
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductAttributeResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<GetProductAttributeResDto>> UpdateProductAttributeAsync(Guid productId, Guid attributeId,
        AddProductAttributeReqDto req)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<GetProductAttributeResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode
                };

            var productAttribute = new ProductAttribute
            {
                Productid = productId,
                Attributeid = attributeId,
                Attributename = req.AttributeName,
                Attributevalue = req.AttributeValue
            };

            var result = await productRepository.UpdateProductAttributeAsync(productAttribute);
            if (!result.IsSuccess || result.Data == null)
                return new Return<GetProductAttributeResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode
                };

            return new Return<GetProductAttributeResDto>
            {
                Data = new GetProductAttributeResDto
                {
                    AttributeId = result.Data.Attributeid,
                    Name = result.Data.Attributename,
                    Value = result.Data.Attributevalue
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductAttributeResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
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
                    StatusCode = currentUser.StatusCode
                };

            var productAttributeResult = await productRepository.GetProductAttributeByIdAsync(attributeId);
            if (!productAttributeResult.IsSuccess || productAttributeResult.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = productAttributeResult.StatusCode
                };

            // Check if the attribute belongs to the product
            if (productAttributeResult.Data.Productid != productId)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductAttributeNotFound
                };

            var result = await productRepository.DeleteProductAttributeAsync(attributeId);
            if (!result.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = result.StatusCode
                };

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    #endregion

    #region private methods

    private async Task<Return<GetProductDetailsResDto>> AddProductPrivateAsync(AddProductReqDto req, Guid currentUserId)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Initialize the product
            var product = new Product
            {
                Name = req.Name.Trim(),
                Description = req.Description?.Trim(),
                Price = req.Price < 0 ? 0 : req.Price,
                StockQuantity = req.StockQuantity,
                SoldQuantity = req.SoldQuantity,
                IsTopSeller = req.IsTopSeller,
                Slug = (req.Slug ?? SlugUtil.GenerateSlug(req.Name)).Trim(),
                MetaTitle = (req.MetaTitle ?? req.Name).Trim(),
                MetaDescription = (req.MetaDescription ?? req.Description ?? req.Name).Trim(),
                Keywords = req.MetaKeywords,
                Status = req.StockQuantity > 0
                    ? req.Status != ProductStatus.Inactive
                        ? ProductStatus.Active
                        : ProductStatus.Inactive
                    : ProductStatus.OutOfStock,
                CreateById = currentUserId,
                CreatedAt = DateTime.Now
            };

            // Add Product to the database
            var result = await productRepository.AddProductAsync(product);
            if (!result.IsSuccess || result.Data == null)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = result.StatusCode
                };

            // Only add active categories
            var categoryResult = await categoryRepository.GetCategoriesAsync(status: GeneralStatus.Active, name: null,
                pageNumber: null, pageSize: null);
            if (!categoryResult.IsSuccess)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = categoryResult.StatusCode
                };

            var categories = categoryResult.Data?.Where(c => req.CategoryIds.Contains(c.CategoryId)).ToList();
            if (categories == null)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CategoryNotFound
                };

            result.Data.ProductCategories = categories.Select(category => new ProductCategory
            {
                ProductId = result.Data.ProductId,
                CategoryId = category.CategoryId
            }).ToList();

            // Product Images
            var productImages = req.Images.Select(image => new ProductImage
            {
                ProductId = result.Data.ProductId,
                Url = image.Url,
                AltText = image.AltText
            }).ToList();
            result.Data.ProductImages = productImages;

            // Product Attributes
            var productAttributes = req.Attributes.Select(attribute => new ProductAttribute
            {
                Productid = result.Data.ProductId,
                Attributename = attribute.AttributeName,
                Attributevalue = attribute.AttributeValue
            }).ToList();
            result.Data.ProductAttributes = productAttributes;

            // Update Product with Categories, Images and Attributes
            var productResult = await productRepository.UpdateProductAsync(result.Data);
            if (!productResult.IsSuccess)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = productResult.StatusCode
                };

            transaction.Complete();

            return new Return<GetProductDetailsResDto>
            {
                Data = new GetProductDetailsResDto
                {
                    ProductId = product.ProductId,
                    ProductCode = product.ProductCode ?? string.Empty,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    SoldQuantity = product.SoldQuantity,
                    IsTopSeller = product.IsTopSeller,
                    Slug = product.Slug,
                    MetaTitle = product.MetaTitle,
                    MetaDescription = product.MetaDescription,
                    Keywords = product.Keywords,
                    Status = product.Status,
                    Images = result.Data.ProductImages.Select(image => new GetProductImageResDto
                    {
                        ImageId = image.ImageId,
                        Url = image.Url,
                        AltText = image.AltText
                    }).ToList(),
                    Categories = result.Data.ProductCategories.Select(category => new GetProductCategoryResDto
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Category?.Name ?? string.Empty
                    }).ToList(),
                    Attributes = result.Data.ProductAttributes.Select(attribute => new GetProductAttributeResDto
                    {
                        AttributeId = attribute.Attributeid,
                        Name = attribute.Attributename,
                        Value = attribute.Attributevalue
                    }).ToList()
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception e)
        {
            return new Return<GetProductDetailsResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e
            };
        }
    }

    #endregion
}
