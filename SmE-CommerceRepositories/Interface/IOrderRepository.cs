using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface IOrderRepository
{
    Task<Return<Order>> CreateOrderAsync(Order order);

    Task<Return<Order>> GetOrderByIdAsync(Guid orderId);
    Task<Return<List<Order>>> GetOrderByUserIdAsync(Guid userId);
    Task<Return<bool>> CheckOrderCodeExistedAsync(string orderCode);
}
