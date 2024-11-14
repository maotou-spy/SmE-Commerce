using SmE_CommerceModels.Enums;
using System.Transactions;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceModels.ResponseDtos.Category;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;
using SmE_CommerceModels.ResponseDtos.Product;
using SmE_CommerceUtilities;

namespace SmE_CommerceServices;

public class ProductService(IProductRepository productRepository, IHelperService helperService) : IProductService
{
    #region Product

    public async Task<Return<IEnumerable<GetProductsResDto>>> GetProductsForCustomerAsync(string? keyword,
        string? sortBy, int pageNumber, int pageSize)
    {
        try
        {
            var currentCustomer = await helperService.GetCurrentUserWithRole(RoleEnum.Customer);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                return new Return<IEnumerable<GetProductsResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = currentCustomer.Message
                };

            var result = await productRepository.GetProductsForCustomerAsync(keyword, sortBy, pageNumber, pageSize);
            if (!result.IsSuccess)
                return new Return<IEnumerable<GetProductsResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = result.Message
                };

            return new Return<IEnumerable<GetProductsResDto>>
            {
                Data = result.Data,
                IsSuccess = true,
                Message = result.Message,
                TotalRecord = result.TotalRecord
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<GetProductsResDto>>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<GetProductDetailsResDto>> AddProductAsync(AddProductReqDto req)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Get the current user
            var currentUser = await helperService.GetCurrentUserWithRole(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = currentUser.Message
                };

            // Initialize the product
            var product = new Product
            {
                ProductCode = req.ProductCode,
                Name = req.Name,
                Description = req.Description,
                Price = req.Price,
                StockQuantity = req.StockQuantity,
                SoldQuantity = req.SoldQuantity,
                IsTopSeller = req.IsTopSeller,
                Slug = req.Slug ?? SlugUtil.GenerateSlug(req.Name),
                MetaTitle = req.MetaTitle ?? req.Name,
                MetaDescription = req.MetaDescription ?? req.Description,
                Keywords = req.MetaKeywords,
                Status = req.StockQuantity > 0 ? req.Status : ProductStatus.OutOfStock,
                CreateById = currentUser.Data.UserId,
                CreatedAt = DateTime.Now
            };

            // Add Product to the database
            var result = await productRepository.AddProductAsync(product);
            if (!result.IsSuccess)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = result.Message
                };

            // Add Product Categories
            var productCategories = req.CategoryIds.Select(categoryId => new ProductCategory
            {
                ProductId = product.ProductId,
                CategoryId = categoryId
            }).ToList();

            var prdCategories = await productRepository.AddProductCategoriesAsync(productCategories);
            if (!prdCategories.IsSuccess)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = prdCategories.Message
                };

            // Add Product Images
            var productImages = req.Images.Select(image => new ProductImage
            {
                ProductId = product.ProductId,
                Url = image.Url,
                ImageHash = HashUtil.Hash(image.Url),
                AltText = image.AltText
            }).ToList();

            var prdImages = await productRepository.AddProductImagesAsync(productImages);
            if (!prdImages.IsSuccess)
                return new Return<GetProductDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = prdImages.Message
                };

            // Add Product Attributes
            Return<List<ProductAttribute>>? prdAttributes = null;
            if (req.Attributes != null)
            {
                var productAttributes = req.Attributes.Select(attribute => new ProductAttribute
                {
                    Productid = product.ProductId,
                    Attributename = attribute.AttributeName,
                    Attributevalue = attribute.AttributeValue
                }).ToList();

                prdAttributes = await productRepository.AddProductAttributesAsync(productAttributes);
                if (!prdAttributes.IsSuccess)
                    return new Return<GetProductDetailsResDto>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = prdAttributes.Message
                    };
            }

            transaction.Complete();

            return new Return<GetProductDetailsResDto>
            {
                Data = new GetProductDetailsResDto
                {
                    ProductId = product.ProductId,
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
                    Images = prdImages.Data?.Select(image => new GetProductImageResDto
                    {
                        ImageId = image.ImageId,
                        Url = image.Url,
                        AltText = image.AltText
                    }).ToList(),
                    Categories = prdCategories.Data?.Select(category => new GetProductCategoryResDto
                    {
                        CategoryId = category.CategoryId,
                        Name = category.Category?.Name ?? string.Empty
                    }).ToList(),
                    Attributes = prdAttributes?.Data?.Select(attribute => new GetProductAttributeResDto
                    {
                        AttributeId = attribute.Attributeid,
                        Name = attribute.Attributename,
                        Value = attribute.Attributevalue
                    }).ToList()
                },
                IsSuccess = true,
                Message = SuccessfulMessage.Created
            };
        }
        catch (Exception ex)
        {
            // Log exception details here as appropriate for debugging
            return new Return<GetProductDetailsResDto>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
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
            var currentUser = await helperService.GetCurrentUserWithRole(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<List<GetProductCategoryResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = currentUser.Message
                };

            // Get current categories of the product
            var currentCategories = await productRepository.GetProductCategoriesAsync(productId);
            if (!currentCategories.IsSuccess)
                return new Return<List<GetProductCategoryResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = currentCategories.Message
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
                        Message = addResult.Message
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
                        Message = deleteResult.Message
                    };
            }

            // Get updated categories
            var updatedCategories = await productRepository.GetProductCategoriesAsync(productId);
            if (!updatedCategories.IsSuccess)
                return new Return<List<GetProductCategoryResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = updatedCategories.Message
                };

            return new Return<List<GetProductCategoryResDto>>
            {
                Data = updatedCategories.Data?.Select(category => new GetProductCategoryResDto
                {
                    CategoryId = category.CategoryId,
                    Name = category.Category?.Name ?? string.Empty
                }).ToList(),
                IsSuccess = true,
                Message = SuccessfulMessage.Updated
            };
        }
        catch (Exception ex)
        {
            return new Return<List<GetProductCategoryResDto>>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
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
            var currentUser = await helperService.GetCurrentUserWithRole(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<GetProductImageResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = currentUser.Message
                };

            var productImage = new ProductImage
            {
                ProductId = productId,
                Url = req.Url,
                ImageHash = HashUtil.Hash(req.Url),
                AltText = req.AltText
            };

            var result = await productRepository.AddProductImageAsync(productImage);
            if (!result.IsSuccess || result.Data == null)
                return new Return<GetProductImageResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = result.Message
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
                Message = result.Message
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductImageResDto>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
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
            var currentUser = await helperService.GetCurrentUserWithRole(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<GetProductAttributeResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = currentUser.Message
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
                    Message = result.Message
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
                Message = result.Message
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductAttributeResDto>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<GetProductAttributeResDto>> UpdateProductAttributeAsync(Guid productId, Guid AttributeId,
        AddProductAttributeReqDto req)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRole(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<GetProductAttributeResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = currentUser.Message
                };

            var productAttribute = new ProductAttribute
            {
                Productid = productId,
                Attributeid = AttributeId,
                Attributename = req.AttributeName,
                Attributevalue = req.AttributeValue
            };

            var result = await productRepository.UpdateProductAttributeAsync(productAttribute);
            if (!result.IsSuccess || result.Data == null)
                return new Return<GetProductAttributeResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = result.Message
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
                Message = result.Message
            };
        }
        catch (Exception ex)
        {
            return new Return<GetProductAttributeResDto>
            {
                Data = null,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<bool>> DeleteProductAttributeAsync(Guid productId, Guid attributeId)
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRole(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    Message = currentUser.Message
                };

            var result = await productRepository.DeleteProductAttributeAsync(productId, attributeId);
            if (!result.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    Message = result.Message
                };

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                Message = result.Message
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    #endregion
}
