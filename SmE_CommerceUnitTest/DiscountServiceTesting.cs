using Moq;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Discount;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceUnitTest;

public class DiscountServiceTesting
{
    private readonly Mock<IDiscountRepository> _discountRepositoryMock;
    private readonly Mock<IHelperService> _helperServiceMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly DiscountService _discountService;

    public DiscountServiceTesting()
    {
        _discountRepositoryMock = new Mock<IDiscountRepository>();
        _helperServiceMock = new Mock<IHelperService>();
        _productRepositoryMock = new Mock<IProductRepository>();
        Mock<IUserRepository> userRepositoryMock = new();

        _discountService = new DiscountService(
            _discountRepositoryMock.Object,
            _helperServiceMock.Object,
            _productRepositoryMock.Object,
            userRepositoryMock.Object
        );
    }

    // Update Discount
    [Fact]
    public async Task UpdateDiscountAsync_ShouldReturnSuccess_WhenConcurrentRequests()
    {
        // Arrange
        var discountId = Guid.NewGuid();
        var discount = new Discount
        {
            DiscountId = discountId,
            DiscountName = "Discount 1",
            DiscountValue = 10,
            IsPercentage = true
        };
        var discountReq = new UpdateDiscountReqDto
        {
            DiscountName = "Discount 1"
        };

        _helperServiceMock.Setup(x => x.GetCurrentUserWithRoleAsync(nameof(RoleEnum.Manager))).ReturnsAsync(
            new Return<User>
            {
                Data = new User
                {
                    UserId = Guid.NewGuid(),
                    Role = RoleEnum.Manager
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1
            });

        _discountRepositoryMock.Setup(x => x.GetDiscountByIdForUpdateAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Return<Discount>
            {
                Data = discount,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1
            });

        _discountRepositoryMock.Setup(x => x.GetDiscountByNameAsync(It.IsAny<string>())).ReturnsAsync(
            new Return<Discount>
            {
                Data = null,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 0
            });

        _productRepositoryMock.Setup(x => x.GetProductByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Return<Product>
            {
                Data = new Product
                {
                    ProductId = Guid.NewGuid(),
                    PrimaryImage = "image.jpg"
                },
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1
            });

        _discountRepositoryMock.Setup(x => x.UpdateDiscountAsync(It.IsAny<Discount>())).ReturnsAsync(
            new Return<Discount>
            {
                Data = discount,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1
            });

        // Act
        var task1 = _discountService.UpdateDiscountAsync(discountId, discountReq);
        var task2 = _discountService.UpdateDiscountAsync(discountId, discountReq);

        var results = await Task.WhenAll(task1, task2);

        // Assert
        var result1 = results[0];
        var result2 = results[1];

        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);

        Assert.Equal(ErrorCode.Ok, result1.StatusCode);
        Assert.Equal(ErrorCode.Ok, result2.StatusCode);
    }
}
