using SmE_CommerceModels.RequestDtos.Order;
using SmE_CommerceModels.ResponseDtos.Order;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IOrderService
{
    Task<Return<bool>> CustomerCreateOrderAsync(CreateOrderReqDto req);

    Task<Return<GetOrderDetailsResDto>> GetOrderByIdAsync(Guid orderId);
    
    Task<Return<bool>> ManagerUpdateOrderStatusAsync(ManagerUpdateOrderStatusReqDto req);

    Task<Return<bool>> CustomerUpdateOrderStatusAsync(CustomerUpdateOrderStatusReqDto req);
    
    Task<Return<IEnumerable<GetOrderResDto>>> GetOrdersAsync(OrderFilterReqDto filter);
}
