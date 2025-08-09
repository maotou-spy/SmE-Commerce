using Moq;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceUnitTest.OrderServiceUnitTest;

public class SystemAutoCompleteShippedOrdersAsyncTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ISettingRepository> _settingRepositoryMock;

    private readonly OrderService _orderService;

    public SystemAutoCompleteShippedOrdersAsyncTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        var productRepositoryMock = new Mock<IProductRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var discountRepositoryMock = new Mock<IDiscountRepository>();
        var addressRepositoryMock = new Mock<IAddressRepository>();
        var paymentRepositoryMock = new Mock<IPaymentRepository>();
        var cartRepositoryMock = new Mock<ICartRepository>();
        _settingRepositoryMock = new Mock<ISettingRepository>();
        var helperServiceMock = new Mock<IHelperService>();

        _orderService = new OrderService(
            _orderRepositoryMock.Object,
            productRepositoryMock.Object,
            userRepositoryMock.Object,
            discountRepositoryMock.Object,
            addressRepositoryMock.Object,
            paymentRepositoryMock.Object,
            cartRepositoryMock.Object,
            _settingRepositoryMock.Object,
            helperServiceMock.Object
        );
    }

    // Success
    // Should successfully auto complete shipped orders and return true when all operations succeed
    [Fact]
    public async Task SystemAutoCompleteShippedOrdersAsync_ShouldReturnTrue_WhenAutoCompleteSuccessfully()
    {
        // Arrange

        var orders = new List<Order>
        {
            new()
            {
                OrderId = Guid.NewGuid(),
                Status = OrderStatus.Shipped,
                SubTotal = 100000,
                DiscountAmount = 10000,
                ModifiedAt = DateTime.Now.AddDays(-15),
            },
            new()
            {
                OrderId = Guid.NewGuid(),
                Status = OrderStatus.Shipped,
                SubTotal = 200000,
                DiscountAmount = 20000,
                ModifiedAt = DateTime.Now.AddDays(-12),
            },
        };

        var setting = new Setting
        {
            Key = SettingEnum.PointsConversionRate,
            Value = "5", // 5%
        };

        _orderRepositoryMock
            .Setup(x => x.GetShippedOrdersBeforeDate(It.IsAny<DateTime>()))
            .ReturnsAsync(
                new Return<IEnumerable<Order>>
                {
                    Data = orders,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _settingRepositoryMock
            .Setup(x => x.GetSettingByKeyAsync(SettingEnum.PointsConversionRate))
            .ReturnsAsync(
                new Return<Setting>
                {
                    Data = setting,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _orderRepositoryMock
            .Setup(x => x.UpdateOrderAsync(It.IsAny<Order>()))
            .ReturnsAsync(
                new Return<bool>
                {
                    Data = true,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _orderRepositoryMock
            .Setup(x => x.AddOrderStatusHistoryAsync(It.IsAny<OrderStatusHistory>()))
            .ReturnsAsync(
                new Return<OrderStatusHistory>
                {
                    Data = new OrderStatusHistory(),
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _orderService.SystemAutoCompleteShippedOrdersAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);

        // Verify all orders were updated
        _orderRepositoryMock.Verify(x => x.UpdateOrderAsync(It.IsAny<Order>()), Times.Exactly(2));
        _orderRepositoryMock.Verify(
            x => x.AddOrderStatusHistoryAsync(It.IsAny<OrderStatusHistory>()),
            Times.Exactly(2)
        );
    }

    // Success
    // Should return true when no shipped orders found before the specified date
    [Fact]
    public async Task SystemAutoCompleteShippedOrdersAsync_ShouldReturnTrue_WhenNoOrdersFound()
    {
        // Arrange
        var autoCompleteDays = 10;

        _orderRepositoryMock
            .Setup(x => x.GetShippedOrdersBeforeDate(It.IsAny<DateTime>()))
            .ReturnsAsync(
                new Return<IEnumerable<Order>>
                {
                    Data = new List<Order>(),
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _orderService.SystemAutoCompleteShippedOrdersAsync(autoCompleteDays);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
        Assert.Equal(ErrorCode.Ok, result.StatusCode);

        // Verify no update operations were called
        _orderRepositoryMock.Verify(x => x.UpdateOrderAsync(It.IsAny<Order>()), Times.Never);
        _orderRepositoryMock.Verify(
            x => x.AddOrderStatusHistoryAsync(It.IsAny<OrderStatusHistory>()),
            Times.Never
        );
    }

    // Success
    // Should correctly calculate points earned based on conversion rate setting
    [Fact]
    public async Task SystemAutoCompleteShippedOrdersAsync_ShouldCalculatePointsCorrectly_WhenConversionRateExists()
    {
        // Arrange
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Status = OrderStatus.Shipped,
            SubTotal = 100000, // 100k
            DiscountAmount = 10000, // 10k
            ModifiedAt = DateTime.Now.AddDays(-15),
        };

        var setting = new Setting
        {
            Key = SettingEnum.PointsConversionRate,
            Value = "5", // 5%
        };

        _orderRepositoryMock
            .Setup(x => x.GetShippedOrdersBeforeDate(It.IsAny<DateTime>()))
            .ReturnsAsync(
                new Return<IEnumerable<Order>>
                {
                    Data = new List<Order> { order },
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _settingRepositoryMock
            .Setup(x => x.GetSettingByKeyAsync(SettingEnum.PointsConversionRate))
            .ReturnsAsync(
                new Return<Setting>
                {
                    Data = setting,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _orderRepositoryMock
            .Setup(x => x.UpdateOrderAsync(It.Is<Order>(o => o.PointsEarned == 4500))) // (100k - 10k) * 5% = 4500
            .ReturnsAsync(
                new Return<bool>
                {
                    Data = true,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _orderRepositoryMock
            .Setup(x => x.AddOrderStatusHistoryAsync(It.IsAny<OrderStatusHistory>()))
            .ReturnsAsync(
                new Return<OrderStatusHistory>
                {
                    Data = new OrderStatusHistory(),
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _orderService.SystemAutoCompleteShippedOrdersAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Data);

        // Verify points calculation
        _orderRepositoryMock.Verify(
            x => x.UpdateOrderAsync(It.Is<Order>(o => o.PointsEarned == 4500)),
            Times.Once
        );
    }

    // Success
    // Should set points to 0 when conversion rate setting is invalid or not found
    [Fact]
    public async Task SystemAutoCompleteShippedOrdersAsync_ShouldSetPointsToZero_WhenConversionRateInvalid()
    {
        // Arrange
        var autoCompleteDays = 10;
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Status = OrderStatus.Shipped,
            SubTotal = 100000,
            DiscountAmount = 10000,
            ModifiedAt = DateTime.Now.AddDays(-15),
        };

        _orderRepositoryMock
            .Setup(x => x.GetShippedOrdersBeforeDate(It.IsAny<DateTime>()))
            .ReturnsAsync(
                new Return<IEnumerable<Order>>
                {
                    Data = new List<Order> { order },
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _settingRepositoryMock
            .Setup(x => x.GetSettingByKeyAsync(SettingEnum.PointsConversionRate))
            .ReturnsAsync(
                new Return<Setting>
                {
                    Data = new Setting { Value = "invalid" }, // Invalid conversion rate
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _orderRepositoryMock
            .Setup(x => x.UpdateOrderAsync(It.Is<Order>(o => o.PointsEarned == 0)))
            .ReturnsAsync(
                new Return<bool>
                {
                    Data = true,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _orderRepositoryMock
            .Setup(x => x.AddOrderStatusHistoryAsync(It.IsAny<OrderStatusHistory>()))
            .ReturnsAsync(
                new Return<OrderStatusHistory>
                {
                    Data = new OrderStatusHistory(),
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _orderService.SystemAutoCompleteShippedOrdersAsync(autoCompleteDays);

        // Assert
        Assert.True(result.IsSuccess);
        _orderRepositoryMock.Verify(
            x => x.UpdateOrderAsync(It.Is<Order>(o => o.PointsEarned == 0)),
            Times.Once
        );
    }

    // Fail
    // Should return false when GetShippedOrdersBeforeDate fails
    [Fact]
    public async Task SystemAutoCompleteShippedOrdersAsync_ShouldReturnFalse_WhenGetShippedOrdersFails()
    {
        // Arrange
        var autoCompleteDays = 10;

        _orderRepositoryMock
            .Setup(x => x.GetShippedOrdersBeforeDate(It.IsAny<DateTime>()))
            .ReturnsAsync(
                new Return<IEnumerable<Order>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.OrderNotFound,
                    InternalErrorMessage = new Exception("Database error"),
                }
            );

        // Act
        var result = await _orderService.SystemAutoCompleteShippedOrdersAsync(autoCompleteDays);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.False(result.Data);
        Assert.Equal(ErrorCode.OrderNotFound, result.StatusCode);
        Assert.NotNull(result.InternalErrorMessage);
    }

    // Fail
    // Should return false when UpdateOrderAsync fails for any order
    [Fact]
    public async Task SystemAutoCompleteShippedOrdersAsync_ShouldReturnFalse_WhenUpdateOrderFails()
    {
        // Arrange
        var autoCompleteDays = 10;
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Status = OrderStatus.Shipped,
            SubTotal = 100000,
            DiscountAmount = 10000,
            ModifiedAt = DateTime.Now.AddDays(-15),
        };

        var setting = new Setting { Key = SettingEnum.PointsConversionRate, Value = "5" };

        _orderRepositoryMock
            .Setup(x => x.GetShippedOrdersBeforeDate(It.IsAny<DateTime>()))
            .ReturnsAsync(
                new Return<IEnumerable<Order>>
                {
                    Data = new List<Order> { order },
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _settingRepositoryMock
            .Setup(x => x.GetSettingByKeyAsync(SettingEnum.PointsConversionRate))
            .ReturnsAsync(
                new Return<Setting>
                {
                    Data = setting,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _orderRepositoryMock
            .Setup(x => x.UpdateOrderAsync(It.IsAny<Order>()))
            .ReturnsAsync(
                new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InternalServerError,
                    InternalErrorMessage = new Exception("Update failed"),
                }
            );

        // Act
        var result = await _orderService.SystemAutoCompleteShippedOrdersAsync(autoCompleteDays);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.False(result.Data);
        Assert.Equal(ErrorCode.InternalServerError, result.StatusCode);
        Assert.NotNull(result.InternalErrorMessage);
    }

    // Fail
    // Should return false when AddOrderStatusHistoryAsync fails
    [Fact]
    public async Task SystemAutoCompleteShippedOrdersAsync_ShouldReturnFalse_WhenAddOrderStatusHistoryFails()
    {
        // Arrange
        var autoCompleteDays = 10;
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Status = OrderStatus.Shipped,
            SubTotal = 100000,
            DiscountAmount = 10000,
            ModifiedAt = DateTime.Now.AddDays(-15),
        };

        var setting = new Setting { Key = SettingEnum.PointsConversionRate, Value = "5" };

        _orderRepositoryMock
            .Setup(x => x.GetShippedOrdersBeforeDate(It.IsAny<DateTime>()))
            .ReturnsAsync(
                new Return<IEnumerable<Order>>
                {
                    Data = [order],
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _settingRepositoryMock
            .Setup(x => x.GetSettingByKeyAsync(SettingEnum.PointsConversionRate))
            .ReturnsAsync(
                new Return<Setting>
                {
                    Data = setting,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _orderRepositoryMock
            .Setup(x => x.UpdateOrderAsync(It.IsAny<Order>()))
            .ReturnsAsync(
                new Return<bool>
                {
                    Data = true,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _orderRepositoryMock
            .Setup(x => x.AddOrderStatusHistoryAsync(It.IsAny<OrderStatusHistory>()))
            .ReturnsAsync(
                new Return<OrderStatusHistory>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InternalServerError,
                    InternalErrorMessage = new Exception("History creation failed"),
                }
            );

        // Act
        var result = await _orderService.SystemAutoCompleteShippedOrdersAsync(autoCompleteDays);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.False(result.Data);
        Assert.Equal(ErrorCode.InternalServerError, result.StatusCode);
        Assert.NotNull(result.InternalErrorMessage);
    }

    // Fail
    // Should return false when exception occurs during execution
    [Fact]
    public async Task SystemAutoCompleteShippedOrdersAsync_ShouldReturnFalse_WhenExceptionThrown()
    {
        // Arrange
        var autoCompleteDays = 10;

        _orderRepositoryMock
            .Setup(x => x.GetShippedOrdersBeforeDate(It.IsAny<DateTime>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _orderService.SystemAutoCompleteShippedOrdersAsync(autoCompleteDays);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.False(result.Data);
        Assert.Equal(ErrorCode.InternalServerError, result.StatusCode);
        Assert.NotNull(result.InternalErrorMessage);
        Assert.IsType<Exception>(result.InternalErrorMessage);
    }

    // Success
    // Should use default 10 days when autoCompleteDays parameter is not provided
    [Fact]
    public async Task SystemAutoCompleteShippedOrdersAsync_ShouldUseDefaultDays_WhenParameterNotProvided()
    {
        // Arrange
        _orderRepositoryMock
            .Setup(x => x.GetShippedOrdersBeforeDate(It.IsAny<DateTime>()))
            .ReturnsAsync(
                new Return<IEnumerable<Order>>
                {
                    Data = [],
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _orderService.SystemAutoCompleteShippedOrdersAsync(); // No parameter provided

        // Assert
        Assert.True(result.IsSuccess);

        // Verify the method was called with a date that represents 10 days ago (default value)
        _orderRepositoryMock.Verify(
            x =>
                x.GetShippedOrdersBeforeDate(
                    It.Is<DateTime>(date =>
                        Math.Abs((DateTime.Now.AddDays(-10) - date).TotalHours) < 1
                    )
                ), // Allow 1 hour tolerance
            Times.Once
        );
    }

    // Success
    // Should set ModifiedById to Guid.Empty for system operations
    [Fact]
    public async Task SystemAutoCompleteShippedOrdersAsync_ShouldSetSystemUser_WhenUpdatingOrders()
    {
        // Arrange
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            Status = OrderStatus.Shipped,
            SubTotal = 100000,
            DiscountAmount = 10000,
            ModifiedAt = DateTime.Now.AddDays(-15),
        };

        var setting = new Setting { Key = SettingEnum.PointsConversionRate, Value = "5" };

        _orderRepositoryMock
            .Setup(x => x.GetShippedOrdersBeforeDate(It.IsAny<DateTime>()))
            .ReturnsAsync(
                new Return<IEnumerable<Order>>
                {
                    Data = new List<Order> { order },
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _settingRepositoryMock
            .Setup(x => x.GetSettingByKeyAsync(SettingEnum.PointsConversionRate))
            .ReturnsAsync(
                new Return<Setting>
                {
                    Data = setting,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _orderRepositoryMock
            .Setup(x => x.UpdateOrderAsync(It.Is<Order>(o => o.ModifiedById == Guid.Empty)))
            .ReturnsAsync(
                new Return<bool>
                {
                    Data = true,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        _orderRepositoryMock
            .Setup(x =>
                x.AddOrderStatusHistoryAsync(
                    It.Is<OrderStatusHistory>(h => h.ModifiedById == Guid.Empty)
                )
            )
            .ReturnsAsync(
                new Return<OrderStatusHistory>
                {
                    Data = new OrderStatusHistory(),
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                }
            );

        // Act
        var result = await _orderService.SystemAutoCompleteShippedOrdersAsync();

        // Assert
        Assert.True(result.IsSuccess);
        _orderRepositoryMock.Verify(
            x => x.UpdateOrderAsync(It.Is<Order>(o => o.ModifiedById == Guid.Empty)),
            Times.Once
        );
        _orderRepositoryMock.Verify(
            x =>
                x.AddOrderStatusHistoryAsync(
                    It.Is<OrderStatusHistory>(h => h.ModifiedById == Guid.Empty)
                ),
            Times.Once
        );
    }
}
