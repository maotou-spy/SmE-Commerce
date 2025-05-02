using Moq;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceUnitTest.ProductServiceUnitTest;

public class UpdateProductVariantAsyncTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IHelperService> _helperServiceMock;
    private readonly IProductService _productService;

    public UpdateProductVariantAsyncTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _helperServiceMock = new Mock<IHelperService>();
        _productService = new ProductService(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _helperServiceMock.Object
        );
    }

    // Test Case 1: Request is null
    // [Fact]
    // public async Task UpdateProductVariantAsync_ReturnsBadRequest_WhenRequestIsNull()
    // {
    //     // Arrange
    //     var productId = Guid.NewGuid();
    //     var productVariantId = Guid.NewGuid();
    //     ProductVariantReqDto request = null;
    //
    //     // Act
    //     var result = await _productService.UpdateProductVariantAsync(
    //         productId,
    //         productVariantId,
    //         request
    //     );
    //
    //     // Assert
    //     Assert.False(result.IsSuccess);
    //     Assert.Equal(ErrorCode.BadRequest, result.StatusCode);
    //     Assert.False(result.Data);
    // }

    // Test Case 2: VariantValues is empty
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsBadRequest_WhenVariantValuesIsEmpty()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 100000,
            StockQuantity = 50,
            VariantImage = "https://example.com/image.jpg",
            VariantValues = [],
        };

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.BadRequest, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 3: Negative Price
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsBadRequest_WhenPriceIsNegative()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = -100000,
            StockQuantity = 50,
            VariantImage = "https://example.com/image.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto
                {
                    VariantNameId = Guid.NewGuid(),
                    VariantValue = "S",
                },
                // Act
            ],
        };

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.BadRequest, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 4: Negative StockQuantity
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsBadRequest_WhenStockQuantityIsNegative()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 100000,
            StockQuantity = -50,
            VariantImage = "https://example.com/image.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto
                {
                    VariantNameId = Guid.NewGuid(),
                    VariantValue = "S",
                },
            ],
        };

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.BadRequest, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 5: Duplicate VariantValues
    [Fact]
    public async Task UpdateProductVariantAsync_HandlesDuplicateVariantValues_Correctly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 120000,
            StockQuantity = 30,
            VariantImage = "https://example.com/newimage.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "L" }, // Duplicate
                new ProductVariantValueReqDto { VariantNameId = colorId, VariantValue = "Blue" },
            ],
        };
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = productVariantId,
                    Price = 100000,
                    StockQuantity = 50,
                    VariantImage = "https://example.com/image.jpg",
                    Status = ProductStatus.Active,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "S" },
                        new() { VariantNameId = colorId, Value = "Red" },
                    },
                },
            },
            StockQuantity = 50,
            Status = ProductStatus.Active,
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.UpdateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(1, result.TotalRecord);
        Assert.Equal(
            "M",
            product
                .ProductVariants.First()
                .VariantAttributes.First(va => va.VariantNameId == sizeId)
                .Value
        );
    }

    // Test Case 6: User is not a Manager
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsUnauthorized_WhenUserIsNotManager()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 100000,
            StockQuantity = 50,
            VariantImage = "https://example.com/image.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto
                {
                    VariantNameId = Guid.NewGuid(),
                    VariantValue = "S",
                },
            ],
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User> { IsSuccess = false, StatusCode = ErrorCode.NotAuthority }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.NotAuthority, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 7: Product does not exist
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsProductNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 100000,
            StockQuantity = 50,
            VariantImage = "https://example.com/image.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto
                {
                    VariantNameId = Guid.NewGuid(),
                    VariantValue = "S",
                },
            ],
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = Guid.NewGuid() },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product> { IsSuccess = false, StatusCode = ErrorCode.ProductNotFound }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductNotFound, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 8: Product is deleted
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsProductNotFound_WhenProductIsDeleted()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 100000,
            StockQuantity = 50,
            VariantImage = "https://example.com/image.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto
                {
                    VariantNameId = Guid.NewGuid(),
                    VariantValue = "S",
                },
            ],
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = Guid.NewGuid() },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = new Product { Status = ProductStatus.Deleted },
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductNotFound, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 9: Product has no variants
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsProductNoVariant_WhenProductHasNoVariants()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 100000,
            StockQuantity = 50,
            VariantImage = "https://example.com/image.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto
                {
                    VariantNameId = Guid.NewGuid(),
                    VariantValue = "S",
                },
            ],
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = Guid.NewGuid() },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = new Product
                    {
                        HasVariant = false,
                        ProductVariants = new List<ProductVariant>(),
                        Status = ProductStatus.Active,
                    },
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductNoVariant, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 10: Variant does not exist
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsProductVariantNotFound_WhenVariantDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 100000,
            StockQuantity = 50,
            VariantImage = "https://example.com/image.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "S" },
            ],
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = Guid.NewGuid() },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = new Product
                    {
                        HasVariant = true,
                        ProductVariants = new List<ProductVariant>
                        {
                            new()
                            {
                                ProductVariantId = Guid.NewGuid(),
                                Status = ProductStatus.Active,
                            },
                        },
                        Status = ProductStatus.Active,
                    },
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductVariantNotFound, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 11: Variant is deleted
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsProductVariantNotFound_WhenVariantIsDeleted()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 100000,
            StockQuantity = 50,
            VariantImage = "https://example.com/image.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "S" },
            ],
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = Guid.NewGuid() },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = new Product
                    {
                        HasVariant = true,
                        ProductVariants = new List<ProductVariant>
                        {
                            new()
                            {
                                ProductVariantId = productVariantId,
                                Status = ProductStatus.Deleted,
                                VariantAttributes = new List<VariantAttribute>
                                {
                                    new() { VariantNameId = sizeId, Value = "S" },
                                },
                            },
                        },
                        Status = ProductStatus.Active,
                    },
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductVariantNotFound, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 12: No changes detected
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsSuccess_WhenNoChangesDetected()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 100000,
            StockQuantity = 50,
            VariantImage = "https://example.com/image.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "S" },
                new ProductVariantValueReqDto { VariantNameId = colorId, VariantValue = "Red" },
            ],
        };
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = productVariantId,
                    Price = 100000,
                    StockQuantity = 50,
                    VariantImage = "https://example.com/image.jpg",
                    Status = ProductStatus.Active,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "S" },
                        new() { VariantNameId = colorId, Value = "Red" },
                    },
                },
            },
            StockQuantity = 50,
            Status = ProductStatus.Active,
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(0, result.TotalRecord);
    }

    // Test Case 13: Variant attributes count mismatch
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsDataInconsistency_WhenVariantAttributesCountMismatch()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 120000,
            StockQuantity = 30,
            VariantImage = "https://example.com/newimage.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
            ],
        };
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = productVariantId,
                    Price = 100000,
                    StockQuantity = 50,
                    VariantImage = "https://example.com/image.jpg",
                    Status = ProductStatus.Active,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "S" },
                        new() { VariantNameId = colorId, Value = "Red" },
                    },
                },
            },
            StockQuantity = 50,
            Status = ProductStatus.Active,
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.DataInconsistency, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 14: Variant attributes type mismatch
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsDataInconsistency_WhenVariantAttributesTypeMismatch()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var brandId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 120000,
            StockQuantity = 30,
            VariantImage = "https://example.com/newimage.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
                new ProductVariantValueReqDto { VariantNameId = brandId, VariantValue = "Nike" },
            ],
        };
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = productVariantId,
                    Price = 100000,
                    StockQuantity = 50,
                    VariantImage = "https://example.com/image.jpg",
                    Status = ProductStatus.Active,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "S" },
                        new() { VariantNameId = colorId, Value = "Red" },
                    },
                },
            },
            StockQuantity = 50,
            Status = ProductStatus.Active,
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.DataInconsistency, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 15: Variant duplicates existing variant
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsProductVariantAlreadyExists_WhenVariantDuplicatesExisting()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 120000,
            StockQuantity = 30,
            VariantImage = "https://example.com/newimage.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
                new ProductVariantValueReqDto { VariantNameId = colorId, VariantValue = "Blue" },
            ],
        };
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = productVariantId,
                    Price = 100000,
                    StockQuantity = 50,
                    VariantImage = "https://example.com/image.jpg",
                    Status = ProductStatus.Active,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "S" },
                        new() { VariantNameId = colorId, Value = "Red" },
                    },
                },
                new()
                {
                    ProductVariantId = Guid.NewGuid(),
                    Price = 110000,
                    StockQuantity = 40,
                    Status = ProductStatus.Active,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "M" },
                        new() { VariantNameId = colorId, Value = "Blue" },
                    },
                },
            },
            StockQuantity = 90,
            Status = ProductStatus.Active,
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductVariantAlreadyExists, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 16: Total stock quantity is negative
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsInvalidStockQuantity_WhenTotalStockQuantityIsNegative()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 120000,
            StockQuantity = -60,
            VariantImage = "https://example.com/newimage.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
                new ProductVariantValueReqDto { VariantNameId = colorId, VariantValue = "Blue" },
            ],
        };
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = productVariantId,
                    Price = 100000,
                    StockQuantity = 50,
                    VariantImage = "https://example.com/image.jpg",
                    Status = ProductStatus.Active,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "S" },
                        new() { VariantNameId = colorId, Value = "Red" },
                    },
                },
            },
            StockQuantity = 50,
            Status = ProductStatus.Active,
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.BadRequest, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 17: Successfully update with Active status
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsSuccess_WhenUpdateSetsActiveStatus()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 120000,
            StockQuantity = 30,
            VariantImage = "https://example.com/newimage.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
                new ProductVariantValueReqDto { VariantNameId = colorId, VariantValue = "Blue" },
            ],
        };
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = productVariantId,
                    Price = 100000,
                    StockQuantity = 50,
                    VariantImage = "https://example.com/image.jpg",
                    Status = ProductStatus.Active,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "S" },
                        new() { VariantNameId = colorId, Value = "Red" },
                    },
                },
            },
            StockQuantity = 50,
            Status = ProductStatus.Active,
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.UpdateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(1, result.TotalRecord);
        Assert.Equal(ProductStatus.Active, product.ProductVariants.First().Status);
    }

    // Test Case 18: Successfully update with Inactive status
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsSuccess_WhenUpdateSetsInactiveStatus()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 120000,
            StockQuantity = 30,
            VariantImage = "https://example.com/newimage.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
                new ProductVariantValueReqDto { VariantNameId = colorId, VariantValue = "Blue" },
            ],
        };
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = productVariantId,
                    Price = 100000,
                    StockQuantity = 50,
                    VariantImage = "https://example.com/image.jpg",
                    Status = ProductStatus.Active,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "S" },
                        new() { VariantNameId = colorId, Value = "Red" },
                    },
                },
            },
            StockQuantity = 50,
            Status = ProductStatus.Inactive,
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.UpdateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(1, result.TotalRecord);
        Assert.Equal(ProductStatus.Inactive, product.ProductVariants.First().Status);
    }

    // Test Case 19: Successfully update with OutOfStock status
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsSuccess_WhenUpdateSetsOutOfStockStatus()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 120000,
            StockQuantity = 0,
            VariantImage = "https://example.com/newimage.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
                new ProductVariantValueReqDto { VariantNameId = colorId, VariantValue = "Blue" },
            ],
        };
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = productVariantId,
                    Price = 100000,
                    StockQuantity = 50,
                    VariantImage = "https://example.com/image.jpg",
                    Status = ProductStatus.Active,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "S" },
                        new() { VariantNameId = colorId, Value = "Red" },
                    },
                },
            },
            StockQuantity = 50,
            Status = ProductStatus.Active,
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.UpdateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(1, result.TotalRecord);
        Assert.Equal(ProductStatus.OutOfStock, product.ProductVariants.First().Status);
    }

    // Test Case 20: Successfully update with maximum stock quantity
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsSuccess_WhenStockQuantityIsMaxValue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 120000,
            StockQuantity = int.MaxValue,
            VariantImage = "https://example.com/newimage.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
                new ProductVariantValueReqDto { VariantNameId = colorId, VariantValue = "Blue" },
            ],
        };
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = productVariantId,
                    Price = 100000,
                    StockQuantity = 50,
                    VariantImage = "https://example.com/image.jpg",
                    Status = ProductStatus.Active,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "S" },
                        new() { VariantNameId = colorId, Value = "Red" },
                    },
                },
            },
            StockQuantity = 50,
            Status = ProductStatus.Active,
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.UpdateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(1, result.TotalRecord);
    }

    // Test Case 21: Successfully update only price
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsSuccess_WhenOnlyPriceChanges()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 120000,
            StockQuantity = 50,
            VariantImage = "https://example.com/image.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "S" },
                new ProductVariantValueReqDto { VariantNameId = colorId, VariantValue = "Red" },
            ],
        };
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = productVariantId,
                    Price = 100000,
                    StockQuantity = 50,
                    VariantImage = "https://example.com/image.jpg",
                    Status = ProductStatus.Active,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "S" },
                        new() { VariantNameId = colorId, Value = "Red" },
                    },
                },
            },
            StockQuantity = 50,
            Status = ProductStatus.Active,
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.UpdateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(1, result.TotalRecord);
    }

    // Test Case 22: Update product fails
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsInternalServerError_WhenUpdateProductFails()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 120000,
            StockQuantity = 30,
            VariantImage = "https://example.com/newimage.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
                new ProductVariantValueReqDto { VariantNameId = colorId, VariantValue = "Blue" },
            ],
        };
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = productVariantId,
                    Price = 100000,
                    StockQuantity = 50,
                    VariantImage = "https://example.com/image.jpg",
                    Status = ProductStatus.Active,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "S" },
                        new() { VariantNameId = colorId, Value = "Red" },
                    },
                },
            },
            StockQuantity = 50,
            Status = ProductStatus.Active,
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.UpdateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.InternalServerError,
                }
            );

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.InternalServerError, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 23: Exception occurs during processing
    [Fact]
    public async Task UpdateProductVariantAsync_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productVariantId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var request = new ProductVariantReqDto
        {
            Price = 120000,
            StockQuantity = 30,
            VariantImage = "https://example.com/newimage.jpg",
            VariantValues =
            [
                new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
            ],
        };
        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok,
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _productService.UpdateProductVariantAsync(
            productId,
            productVariantId,
            request
        );

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.InternalServerError, result.StatusCode);
        Assert.False(result.Data);
        Assert.NotNull(result.InternalErrorMessage);
    }
}
