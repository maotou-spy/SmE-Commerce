using Moq;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Product;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceUnitTest.ProductServiceUnitTest;

public class AddProductVariantAsyncTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IHelperService> _helperServiceMock;
    private readonly IProductService _productService;

    public AddProductVariantAsyncTests()
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

    // Test Case 1: Empty request list
    [Fact]
    public async Task AddProductVariantAsync_ReturnsBadRequest_WhenRequestIsEmpty()
    {
        // Arrange - Prepare an empty request list to simulate invalid input
        var productId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>();

        // Act - Call the method with the empty request
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Verify that the method returns a failure with BadRequest status
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.BadRequest, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 2: VariantValues is empty
    [Fact]
    public async Task AddProductVariantAsync_ReturnsBadRequest_WhenVariantValuesIsEmpty()
    {
        // Arrange - Set up a request with an empty VariantValues list
        var productId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 10,
                VariantValues = new List<ProductVariantValueReqDto>()
            }
        };

        // Act - Execute the method with the invalid request
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Expect a BadRequest due to missing VariantValues
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.BadRequest, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 3: Negative Price
    [Fact]
    public async Task AddProductVariantAsync_ReturnsBadRequest_WhenPriceIsNegative()
    {
        // Arrange - Create a request with a negative price
        var productId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = -10,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = Guid.NewGuid(),
                        VariantValue = "value"
                    }
                ]
            }
        };

        // Act - Call the method with the invalid price
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Check for failure with BadRequest status due to negative price
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.BadRequest, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 4: Negative StockQuantity
    [Fact]
    public async Task AddProductVariantAsync_ReturnsBadRequest_WhenStockQuantityIsNegative()
    {
        // Arrange - Set up a request with a negative stock quantity
        var productId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = -5,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = Guid.NewGuid(),
                        VariantValue = "value"
                    }
                ]
            }
        };

        // Act - Execute the method with the invalid stock quantity
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Expect a BadRequest due to negative stock quantity
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.BadRequest, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 5: User is not a Manager
    [Fact]
    public async Task AddProductVariantAsync_ReturnsUnauthorized_WhenUserIsNotManager()
    {
        // Arrange - Prepare a valid request but mock the user as non-Manager
        var productId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = Guid.NewGuid(),
                        VariantValue = "value"
                    }
                ]
            }
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User> { IsSuccess = false, StatusCode = ErrorCode.NotAuthority }
            );

        // Act - Call the method with unauthorized user
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Verify Unauthorized status due to insufficient role
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.NotAuthority, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 6: User is a Manager
    [Fact]
    public async Task AddProductVariantAsync_ContinuesProcessing_WhenUserIsManager()
    {
        // Arrange - Set up a valid request and mock the user as Manager
        var productId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = Guid.NewGuid(),
                        VariantValue = "value"
                    }
                ]
            }
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = Guid.NewGuid() },
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method with a valid Manager user
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Ensure the result is not null, indicating processing continues
        Assert.NotNull(result);
    }

    // Test Case 7: Product does not exist
    [Fact]
    public async Task AddProductVariantAsync_ReturnsProductNotFound_WhenProductDoesNotExist()
    {
        // Arrange - Prepare a valid request and mock product retrieval to fail
        var productId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = Guid.NewGuid(),
                        VariantValue = "value"
                    }
                ]
            }
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = Guid.NewGuid() },
                    StatusCode = ErrorCode.Ok
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product> { IsSuccess = false, StatusCode = ErrorCode.ProductNotFound }
            );

        // Act - Call the method with a non-existent product
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Expect ProductNotFound status
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductNotFound, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 8: Product is deleted
    [Fact]
    public async Task AddProductVariantAsync_ReturnsProductNotFound_WhenProductIsDeleted()
    {
        // Arrange - Set up a valid request and mock a deleted product
        var productId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = Guid.NewGuid(),
                        VariantValue = "value"
                    }
                ]
            }
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = Guid.NewGuid() },
                    StatusCode = ErrorCode.Ok
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = new Product { Status = ProductStatus.Deleted },
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method with a deleted product
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Verify ProductNotFound status for deleted product
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductNotFound, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 9: Product has no variants and less than 2 variants added
    [Fact]
    public async Task AddProductVariantAsync_ReturnsAtLeastTwoProductVariant_WhenProductHasNoVariantsAndLessThanTwoAdded()
    {
        // Arrange - Create a request with only 1 variant for a product with no variants
        var productId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = Guid.NewGuid(),
                        VariantValue = "value"
                    }
                ]
            }
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = Guid.NewGuid() },
                    StatusCode = ErrorCode.Ok
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
                        ProductVariants = new List<ProductVariant>()
                    },
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Call the method with insufficient variants
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Expect AtLeastTwoProductVariant error
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.AtLeastTwoProductVariant, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 10: New variant is not uniform with existing variants
    [Fact]
    public async Task AddProductVariantAsync_ReturnsInvalidUniformity_WhenVariantsAreNotUniform()
    {
        // Arrange - Set up a request with a variant that differs from existing ones
        var productId = Guid.NewGuid();
        var variantNameId1 = Guid.NewGuid();
        var variantNameId2 = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = variantNameId2,
                        VariantValue = "value2"
                    },
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = variantNameId2,
                        VariantValue = "value3"
                    }
                ]
            }
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = Guid.NewGuid() },
                    StatusCode = ErrorCode.Ok
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
                                VariantAttributes = new List<VariantAttribute>
                                {
                                    new() { VariantNameId = variantNameId1, Value = "value1" }
                                }
                            }
                        }
                    },
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method with non-uniform variant
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Expect DataInconsistency error
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.DataInconsistency, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 11: New variant duplicates an existing variant
    [Fact]
    public async Task AddProductVariantAsync_ReturnsProductVariantAlreadyExists_WhenVariantDuplicatesExisting()
    {
        // Arrange - Prepare a request with a variant that matches an existing one
        var productId = Guid.NewGuid();
        var variantNameId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = variantNameId,
                        VariantValue = "value1"
                    }
                ]
            }
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = Guid.NewGuid() },
                    StatusCode = ErrorCode.Ok
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                    Data = new Product
                    {
                        HasVariant = true,
                        ProductVariants =
                        [
                            new ProductVariant
                            {
                                VariantAttributes =
                                [
                                    new VariantAttribute
                                    {
                                        VariantNameId = variantNameId,
                                        Value = "value1"
                                    }
                                ]
                            },
                            new ProductVariant
                            {
                                VariantAttributes =
                                [
                                    new VariantAttribute
                                    {
                                        VariantNameId = variantNameId,
                                        Value = "value2"
                                    }
                                ]
                            }
                        ]
                    }
                }
            );

        // Act - Call the method with a duplicate variant
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Verify ProductVariantAlreadyExists error
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ProductVariantAlreadyExists, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 12: New variants duplicate each other
    [Fact]
    public async Task AddProductVariantAsync_ReturnsBadRequest_WhenNewVariantsDuplicateEachOther()
    {
        // Arrange - Create a request with two identical new variants
        var productId = Guid.NewGuid();
        var variantNameId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = variantNameId,
                        VariantValue = "value1"
                    }
                ]
            },
            new()
            {
                Price = 20,
                StockQuantity = 20,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = variantNameId,
                        VariantValue = "value1"
                    }
                ]
            }
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = Guid.NewGuid() },
                    StatusCode = ErrorCode.Ok
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
                        ProductVariants = new List<ProductVariant>()
                    },
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method with duplicate new variants
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Expect BadRequest due to duplicate variants in the request
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.BadRequest, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 13: Product has no variants and VariantValues is empty
    [Fact]
    public async Task AddProductVariantAsync_ReturnsBadRequest_WhenNoVariantsAndVariantValuesEmpty()
    {
        // Arrange - Set up a request with empty VariantValues for a product with no variants
        var productId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 10,
                VariantValues = new List<ProductVariantValueReqDto>()
            }
        };
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = Guid.NewGuid() },
                    StatusCode = ErrorCode.Ok
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
                        ProductVariants = new List<ProductVariant>()
                    },
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Call the method with invalid VariantValues
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Verify BadRequest due to empty VariantValues
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.BadRequest, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 14: Successfully add variants to a product without variants
    [Fact]
    public async Task AddProductVariantAsync_ReturnsSuccess_WhenAddingVariantsToProductWithoutVariants()
    {
        // Arrange - Prepare a valid request with 2 variants for a product without variants
        var productId = Guid.NewGuid();
        var variantNameId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 5,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = variantNameId,
                        VariantValue = "value1"
                    }
                ]
            },
            new()
            {
                Price = 20,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = variantNameId,
                        VariantValue = "value2"
                    }
                ]
            }
        };
        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
                }
            );
        var product = new Product
        {
            HasVariant = false,
            ProductVariants = new List<ProductVariant>(),
            StockQuantity = 0,
            Status = ProductStatus.Active
        };
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );
        _productRepositoryMock
            .Setup(x => x.UpdateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method to add variants
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Verify successful addition with correct TotalRecord
        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(2, result.TotalRecord);
    }

    // Test Case 15: Successfully add variants to a product with existing variants
    [Fact]
    public async Task AddProductVariantAsync_ReturnsSuccess_WhenAddingVariantsToProductWithVariants()
    {
        // Arrange - Set up a request to add a new variant to a product with existing variants
        var productId = Guid.NewGuid();
        var variantNameId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 20,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = variantNameId,
                        VariantValue = "value3"
                    }
                ]
            }
        };
        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
                }
            );
        var product = new Product
        {
            HasVariant = true,
            ProductVariants =
            [
                new ProductVariant
                {
                    VariantAttributes =
                    [
                        new VariantAttribute { VariantNameId = variantNameId, Value = "value1" }
                    ]
                },
                new ProductVariant
                {
                    VariantAttributes =
                    [
                        new VariantAttribute { VariantNameId = variantNameId, Value = "value2" }
                    ]
                }
            ],
            StockQuantity = 5,
            Status = ProductStatus.Active
        };
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );
        _productRepositoryMock
            .Setup(x => x.UpdateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Call the method to add a new variant
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Ensure successful addition with correct TotalRecord
        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(1, result.TotalRecord);
    }

    // Test Case 16: Update product fails
    [Fact]
    public async Task AddProductVariantAsync_ReturnsError_WhenUpdateProductFails()
    {
        // Arrange - Set up a valid request but mock product update to fail
        var productId = Guid.NewGuid();
        var variantNameId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 5,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = variantNameId,
                        VariantValue = "value1"
                    }
                ]
            },
            new()
            {
                Price = 10,
                StockQuantity = 5,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = variantNameId,
                        VariantValue = "value2"
                    }
                ]
            }
        };

        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
                }
            );

        var product = new Product
        {
            HasVariant = false,
            ProductVariants = new List<ProductVariant>(),
            StockQuantity = 0
        };

        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );

        _productRepositoryMock
            .Setup(x => x.UpdateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.InternalServerError
                }
            );

        // Act - Call the method with a failing update
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Verify failure due to update error
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.InternalServerError, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 17: Exception occurs during processing
    [Fact]
    public async Task AddProductVariantAsync_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange - Prepare a valid request and mock an exception during product retrieval
        var productId = Guid.NewGuid();
        var variantNameId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 5,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = variantNameId,
                        VariantValue = "value1"
                    }
                ]
            }
        };
        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
                }
            );
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ThrowsAsync(new Exception("Database error"));

        // Act - Execute the method to trigger the exception
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Expect InternalServerError with exception details
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.InternalServerError, result.StatusCode);
        Assert.False(result.Data);
        Assert.NotNull(result.InternalErrorMessage);
    }

    // Test Case 18: Inconsistent number of VariantAttributes among new variants
    [Fact]
    public async Task AddProductVariantAsync_ReturnsInvalidUniformity_WhenNewVariantsHaveInconsistentAttributeCount()
    {
        // Arrange - Set up a request where new variants have different numbers of VariantAttributes
        var productId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 5,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "S" }
                ]
            },
            new()
            {
                Price = 20,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = colorId,
                        VariantValue = "Blue"
                    }
                ]
            }
        };
        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
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
                        ProductVariants = new List<ProductVariant>()
                    },
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method with inconsistent attribute counts
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Expect InvalidUniformity due to differing attribute counts
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.DataInconsistency, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 19: Inconsistent types of VariantAttributes among new variants
    [Fact]
    public async Task AddProductVariantAsync_ReturnsInvalidUniformity_WhenNewVariantsHaveInconsistentAttributeTypes()
    {
        // Arrange - Set up a request where new variants have different attribute types
        var productId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 5,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "S" }
                ]
            },
            new()
            {
                Price = 20,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = colorId,
                        VariantValue = "Blue"
                    }
                ]
            }
        };
        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
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
                        ProductVariants = new List<ProductVariant>()
                    },
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method with inconsistent attribute types
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Expect InvalidUniformity due to differing attribute types
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.DataInconsistency, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 20: Number of VariantAttributes in new variant does not match existing variants
    [Fact]
    public async Task AddProductVariantAsync_ReturnsInvalidUniformity_WhenNewVariantAttributeCountMismatchesExisting()
    {
        // Arrange - Set up a request where new variant has fewer attributes than existing ones
        var productId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 20,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "S" }
                ]
            }
        };
        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
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
                                VariantAttributes = new List<VariantAttribute>
                                {
                                    new() { VariantNameId = sizeId, Value = "L" },
                                    new() { VariantNameId = colorId, Value = "Red" } // 2 attributes
                                }
                            }
                        }
                    },
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method with mismatched attribute count
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Expect InvalidUniformity due to attribute count mismatch
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.DataInconsistency, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 21: Types of VariantAttributes in new variant do not match existing variants
    [Fact]
    public async Task AddProductVariantAsync_ReturnsInvalidUniformity_WhenNewVariantAttributeTypesMismatchExisting()
    {
        // Arrange - Set up a request where new variant has different attribute types than existing ones
        var productId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var materialId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 20,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = materialId,
                        VariantValue = "Cotton"
                    }
                ]
            }
        };
        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
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
                                VariantAttributes = new List<VariantAttribute>
                                {
                                    new() { VariantNameId = sizeId, Value = "S" },
                                    new() { VariantNameId = colorId, Value = "Red" } // Size + Color
                                }
                            }
                        }
                    },
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method with mismatched attribute types
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Expect InvalidUniformity due to attribute type mismatch
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.DataInconsistency, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 22: Data inconsistency when HasVariant is true but ProductVariants has 1 or fewer variants
    [Fact]
    public async Task AddProductVariantAsync_ReturnsDataInconsistency_WhenHasVariantTrueButInsufficientVariants()
    {
        // Arrange - Set up a product with HasVariant = true but only 1 variant
        var productId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 5,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "S" }
                ]
            }
        };
        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
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
                                VariantAttributes = new List<VariantAttribute>
                                {
                                    new() { VariantNameId = sizeId, Value = "M" }
                                }
                            }
                        },
                        Status = ProductStatus.Active
                    },
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method with inconsistent data
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Expect DataInconsistency error
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.DataInconsistency, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 23: StockQuantity = 0 sets variant status to OutOfStock
    [Fact]
    public async Task AddProductVariantAsync_SetsOutOfStockStatus_WhenStockQuantityIsZero()
    {
        // Arrange - Prepare a request with StockQuantity = 0
        var productId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 0,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "S" },
                    new ProductVariantValueReqDto { VariantNameId = colorId, VariantValue = "Red" }
                ]
            },
            new()
            {
                Price = 20,
                StockQuantity = 5,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = colorId,
                        VariantValue = "Blue"
                    }
                ]
            }
        };
        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
                }
            );
        var product = new Product
        {
            HasVariant = false,
            ProductVariants = [],
            StockQuantity = 0,
            Status = ProductStatus.Active
        };
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );
        _productRepositoryMock
            .Setup(x =>
                x.UpdateProductAsync(
                    It.Is<Product>(p =>
                        p.ProductVariants.Any(v => v.Status == ProductStatus.OutOfStock)
                    )
                )
            )
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method with zero stock
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Verify success and OutOfStock status applied
        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(2, result.TotalRecord);
    }

    // Test Case 24: Product is Inactive affecting new variant status
    [Fact]
    public async Task AddProductVariantAsync_SetsInactiveStatus_WhenProductIsInactive()
    {
        // Arrange - Prepare a request for a product with Inactive status
        var productId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 5,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "S" },
                    new ProductVariantValueReqDto { VariantNameId = colorId, VariantValue = "Red" }
                ]
            },
            new()
            {
                Price = 20,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" },
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = colorId,
                        VariantValue = "Blue"
                    }
                ]
            }
        };
        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
                }
            );
        var product = new Product
        {
            HasVariant = false,
            ProductVariants = new List<ProductVariant>(),
            StockQuantity = 0,
            Status = ProductStatus.Inactive
        };
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );
        _productRepositoryMock
            .Setup(x =>
                x.UpdateProductAsync(
                    It.Is<Product>(p =>
                        p.ProductVariants.All(v => v.Status == ProductStatus.Inactive)
                    )
                )
            )
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method with an Inactive product
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Verify success and Inactive status applied to new variants
        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(2, result.TotalRecord);
    }

    // Test Case 25: Add third variant with fewer attributes than existing (size + color)
    [Fact]
    public async Task AddProductVariantAsync_ReturnsInvalidUniformity_WhenThirdVariantHasFewerAttributes()
    {
        // Arrange - Set up a product with 2 variants (size + color), add a third with only size
        var productId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 30,
                StockQuantity = 15,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "L" } // Only size
                ]
            }
        };
        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
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
                                VariantAttributes = new List<VariantAttribute>
                                {
                                    new() { VariantNameId = sizeId, Value = "S" },
                                    new() { VariantNameId = colorId, Value = "Red" }
                                }
                            },
                            new()
                            {
                                VariantAttributes = new List<VariantAttribute>
                                {
                                    new() { VariantNameId = sizeId, Value = "M" },
                                    new() { VariantNameId = colorId, Value = "Blue" }
                                }
                            }
                        },
                        StockQuantity = 20,
                        Status = ProductStatus.Active
                    },
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method with fewer attributes
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Expect InvalidUniformity due to fewer attributes
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.DataInconsistency, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 26: Add third variant with more attributes than existing (size + color + material)
    [Fact]
    public async Task AddProductVariantAsync_ReturnsInvalidUniformity_WhenThirdVariantHasMoreAttributes()
    {
        // Arrange - Set up a product with 2 variants (size + color), add a third with size + color + material
        var productId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var materialId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 30,
                StockQuantity = 15,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "L" },
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = colorId,
                        VariantValue = "Green"
                    },
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = materialId,
                        VariantValue = "Cotton"
                    } // Extra attribute
                ]
            }
        };
        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
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
                                VariantAttributes = new List<VariantAttribute>
                                {
                                    new() { VariantNameId = sizeId, Value = "S" },
                                    new() { VariantNameId = colorId, Value = "Red" }
                                }
                            },
                            new()
                            {
                                VariantAttributes = new List<VariantAttribute>
                                {
                                    new() { VariantNameId = sizeId, Value = "M" },
                                    new() { VariantNameId = colorId, Value = "Blue" }
                                }
                            }
                        },
                        StockQuantity = 20,
                        Status = ProductStatus.Active
                    },
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method with more attributes
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Expect InvalidUniformity due to extra attributes
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.DataInconsistency, result.StatusCode);
        Assert.False(result.Data);
    }

    // Test Case 27: Successfully add multiple variants to a product with existing variants
    [Fact]
    public async Task AddProductVariantAsync_ReturnsSuccess_WhenAddingMultipleVariantsToProductWithVariants()
    {
        // Arrange - Set up a request to add 2 new variants to a product with 2 existing variants
        var productId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 30,
                StockQuantity = 15,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "L" },
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = colorId,
                        VariantValue = "Green"
                    }
                ]
            },
            new()
            {
                Price = 40,
                StockQuantity = 20,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "XL" },
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = colorId,
                        VariantValue = "Black"
                    }
                ]
            }
        };
        var userId = Guid.NewGuid();
        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
                }
            );
        var product = new Product
        {
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "S" },
                        new() { VariantNameId = colorId, Value = "Red" }
                    }
                },
                new()
                {
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "M" },
                        new() { VariantNameId = colorId, Value = "Blue" }
                    }
                }
            },
            StockQuantity = 20,
            Status = ProductStatus.Active
        };
        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );
        _productRepositoryMock
            .Setup(x => x.UpdateProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );

        // Act - Execute the method to add multiple variants
        var result = await _productService.AddProductVariantAsync(productId, request);

        // Assert - Ensure successful addition with correct TotalRecord
        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(2, result.TotalRecord);
    }

    // Thêm vào file AddProductVariantAsyncTests.cs

    // Test Case 28: Cập nhật StockQuantity khi sản phẩm không có biến thể ban đầu
    [Fact]
    public async Task AddProductVariantAsync_UpdatesStockQuantity_WhenProductHasNoVariants()
    {
        var productId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 10,
                StockQuantity = 5,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "S" }
                ]
            },
            new()
            {
                Price = 20,
                StockQuantity = 10,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "M" }
                ]
            }
        };
        var userId = Guid.NewGuid();
        var product = new Product
        {
            ProductId = productId,
            HasVariant = false,
            ProductVariants = new List<ProductVariant>(),
            StockQuantity = 0,
            Status = ProductStatus.Active
        };

        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
                }
            );

        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );

        _productRepositoryMock
            .Setup(x =>
                x.UpdateProductAsync(
                    It.Is<Product>(p => p.StockQuantity == 15 && p.HasVariant == true)
                )
            )
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );

        var result = await _productService.AddProductVariantAsync(productId, request);

        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(2, result.TotalRecord);

        _productRepositoryMock.Verify(
            x =>
                x.UpdateProductAsync(
                    It.Is<Product>(p => p.StockQuantity == 15 && p.HasVariant == true)
                ),
            Times.Once()
        );
    }

    [Fact]
    public async Task AddProductVariantAsync_UpdatesStockQuantity_WhenProductHasExistingVariants()
    {
        var productId = Guid.NewGuid();
        var sizeId = Guid.NewGuid();
        var colorId = Guid.NewGuid();
        var request = new List<ProductVariantReqDto>
        {
            new()
            {
                Price = 30,
                StockQuantity = 15,
                VariantValues =
                [
                    new ProductVariantValueReqDto { VariantNameId = sizeId, VariantValue = "L" },
                    new ProductVariantValueReqDto
                    {
                        VariantNameId = colorId,
                        VariantValue = "Green"
                    }
                ]
            }
        };
        var userId = Guid.NewGuid();
        var product = new Product
        {
            ProductId = productId,
            HasVariant = true,
            ProductVariants = new List<ProductVariant>
            {
                new()
                {
                    StockQuantity = 20,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "S" },
                        new() { VariantNameId = colorId, Value = "Red" }
                    }
                },
                new()
                {
                    StockQuantity = 30,
                    VariantAttributes = new List<VariantAttribute>
                    {
                        new() { VariantNameId = sizeId, Value = "M" },
                        new() { VariantNameId = colorId, Value = "Blue" }
                    }
                }
            },
            StockQuantity = 50,
            Status = ProductStatus.Active
        };

        _helperServiceMock
            .Setup(x => x.GetCurrentUserWithRoleAsync(RoleEnum.Manager))
            .ReturnsAsync(
                new Return<User>
                {
                    IsSuccess = true,
                    Data = new User { UserId = userId },
                    StatusCode = ErrorCode.Ok
                }
            );

        _productRepositoryMock
            .Setup(x => x.GetProductByIdForUpdateAsync(productId))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );

        _productRepositoryMock
            .Setup(x => x.UpdateProductAsync(It.Is<Product>(p => p.StockQuantity == 65)))
            .ReturnsAsync(
                new Return<Product>
                {
                    IsSuccess = true,
                    Data = product,
                    StatusCode = ErrorCode.Ok
                }
            );

        var result = await _productService.AddProductVariantAsync(productId, request);

        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);
        Assert.True(result.Data);
        Assert.Equal(1, result.TotalRecord);

        _productRepositoryMock.Verify(
            x => x.UpdateProductAsync(It.Is<Product>(p => p.StockQuantity == 65)),
            Times.Once()
        );
    }
}
