using Moq;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceUnitTest.ProductServiceUnitTest;

public class DeleteProductVariantAsyncTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IHelperService> _helperServiceMock;
    private readonly IProductService _productService;

    public DeleteProductVariantAsyncTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _helperServiceMock = new Mock<IHelperService>();
        _productService = new ProductService(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _helperServiceMock.Object
        );
    }

    // Test Case 1: User is not a Manager
    [Fact]
    public async Task DeleteProductVariantAsync_ReturnsNotAuthority_WhenUserIsNotManager()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User> { IsSuccess = false, StatusCode = ErrorCode.NotAuthority }
            );

        // Act
        var result = await _productService.DeleteProductVariantAsync(productId, variantId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.NotAuthority, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 2: Product does not exist
    [Fact]
    public async Task DeleteProductVariantAsync_ReturnsProductNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
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
            .ReturnsAsync(
                new Return<Product> { IsSuccess = false, StatusCode = ErrorCode.ProductNotFound }
            );

        // Act
        var result = await _productService.DeleteProductVariantAsync(productId, variantId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductNotFound, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 3: Product is deleted
    [Fact]
    public async Task DeleteProductVariantAsync_ReturnsProductNotFound_WhenProductIsDeleted()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product { Status = ProductStatus.Deleted };
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
        var result = await _productService.DeleteProductVariantAsync(productId, variantId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductNotFound, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 4: Product has no variants
    [Fact]
    public async Task DeleteProductVariantAsync_ReturnsProductVariantNotFound_WhenProductHasNoVariants()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = false,
            ProductVariants = new List<ProductVariant>(),
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
        var result = await _productService.DeleteProductVariantAsync(productId, variantId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductVariantNotFound, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 5: Product has empty variant list
    [Fact]
    public async Task DeleteProductVariantAsync_ReturnsProductVariantNotFound_WhenVariantListIsEmpty()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>(),
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
        var result = await _productService.DeleteProductVariantAsync(productId, variantId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductVariantNotFound, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 6: Variant does not exist
    [Fact]
    public async Task DeleteProductVariantAsync_ReturnsProductVariantNotFound_WhenVariantDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new() { ProductVariantId = Guid.NewGuid(), Status = ProductStatus.Active },
            },
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
        var result = await _productService.DeleteProductVariantAsync(productId, variantId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductVariantNotFound, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 7: Variant is already deleted
    [Fact]
    public async Task DeleteProductVariantAsync_ReturnsProductVariantNotFound_WhenVariantIsDeleted()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new() { ProductVariantId = variantId, Status = ProductStatus.Deleted },
            },
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
        var result = await _productService.DeleteProductVariantAsync(productId, variantId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductVariantNotFound, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 8: Only two active variants remain
    [Fact]
    public async Task DeleteProductVariantAsync_ReturnsMinimumVariantRequired_WhenTwoActiveVariantsRemain()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = variantId,
                    Status = ProductStatus.Active,
                    StockQuantity = 10,
                },
                new()
                {
                    ProductVariantId = Guid.NewGuid(),
                    Status = ProductStatus.Active,
                    StockQuantity = 20,
                },
                new() { ProductVariantId = Guid.NewGuid(), Status = ProductStatus.Deleted },
            },
            StockQuantity = 30,
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
        var result = await _productService.DeleteProductVariantAsync(productId, variantId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.MinimumVariantRequired, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 9: Only one active variant remains
    [Fact]
    public async Task DeleteProductVariantAsync_ReturnsMinimumVariantRequired_WhenOneActiveVariantRemains()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = variantId,
                    Status = ProductStatus.Active,
                    StockQuantity = 10,
                },
                new() { ProductVariantId = Guid.NewGuid(), Status = ProductStatus.Deleted },
            },
            StockQuantity = 10,
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
        var result = await _productService.DeleteProductVariantAsync(productId, variantId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.MinimumVariantRequired, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 10: Successfully delete variant
    [Fact]
    public async Task DeleteProductVariantAsync_ReturnsSuccess_WhenDeletionIsValid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = variantId,
                    Status = ProductStatus.Active,
                    StockQuantity = 10,
                },
                new()
                {
                    ProductVariantId = Guid.NewGuid(),
                    Status = ProductStatus.Active,
                    StockQuantity = 20,
                },
                new()
                {
                    ProductVariantId = Guid.NewGuid(),
                    Status = ProductStatus.Active,
                    StockQuantity = 30,
                },
            },
            StockQuantity = 60,
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
        var result = await _productService.DeleteProductVariantAsync(productId, variantId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(1, result.TotalRecord);
        Assert.Equal(
            ProductStatus.Deleted,
            product.ProductVariants.First(v => v.ProductVariantId == variantId).Status
        );
        Assert.Equal(
            0,
            product.ProductVariants.First(v => v.ProductVariantId == variantId).StockQuantity
        );
        Assert.Equal(userId, product.ModifiedById);
        Assert.Equal(50, product.StockQuantity);
    }

    // Test Case 11: Update product fails
    [Fact]
    public async Task DeleteProductVariantAsync_ReturnsInternalServerError_WhenUpdateProductFails()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    ProductVariantId = variantId,
                    Status = ProductStatus.Active,
                    StockQuantity = 10,
                },
                new()
                {
                    ProductVariantId = Guid.NewGuid(),
                    Status = ProductStatus.Active,
                    StockQuantity = 20,
                },
                new()
                {
                    ProductVariantId = Guid.NewGuid(),
                    Status = ProductStatus.Active,
                    StockQuantity = 30,
                },
            },
            StockQuantity = 60,
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
        var result = await _productService.DeleteProductVariantAsync(productId, variantId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.InternalServerError, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 12: Exception occurs during processing
    [Fact]
    public async Task DeleteProductVariantAsync_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
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
        var result = await _productService.DeleteProductVariantAsync(productId, variantId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.InternalServerError, result.StatusCode);
        Assert.False(result.Data);
        Assert.NotNull(result.InternalErrorMessage);
    }
}
