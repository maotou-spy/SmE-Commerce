using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface IOrderRepository
{
    Task<Return<Order>> CreateOrderAsync(Order order);
    Task<Return<Order>> GetOrderByIdAsync(Guid orderId, Guid? userId);
    Task<Return<List<Order>>> GetOrderByUserIdAsync(Guid userId);
    Task<Return<List<Order>>> GetOrdersByStatusAndUserIdAsync(Guid userId, string statusFilter, DateTime? fromDate, DateTime? toDate);
    Task<Return<OrderStatusHistory>> CreateOrderStatusHistoryAsync(OrderStatusHistory req);
}
