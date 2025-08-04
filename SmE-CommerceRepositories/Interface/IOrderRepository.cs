using Microsoft.EntityFrameworkCore.Storage;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Order;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface IOrderRepository
{
    Task<Return<Order>> CreateOrderAsync(Order order);

    Task<Return<Order>> GetOrderByIdAsync(Guid orderId, Guid? userId);

    Task<Return<List<Order>>> GetOrderByUserIdAsync(Guid userId);

    Task<Return<IEnumerable<Order>>> GetOrdersAsync(OrderFilterReqDto filter, Guid userId);

    Task<Return<OrderStatusHistory>> AddOrderStatusHistoryAsync(OrderStatusHistory req);

    Task<Return<bool>> UpdateOrderStatusRangeAsync(List<Order> orders);

    Task<Return<bool>> UpdateOrderAsync(Order order);

    Task<List<Order>> GetOrdersByIdsAsync(IEnumerable<Guid> orderIds);

    Task<Return<Order>> GetOrderByIdForUpdateAsync(Guid orderId);

    Task<Return<IEnumerable<OrderStatusHistory>>> AddRangeOrderStatusHistoriesAsync(
        List<OrderStatusHistory> req
    );

    Task<Return<IEnumerable<Order>>> GetShippedOrdersBeforeDate(DateTime dateTime);
}
