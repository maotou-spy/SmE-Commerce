using SmE_CommerceModels.RequestDtos.Order;
using SmE_CommerceModels.ResponseDtos.Order;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IOrderService
{
    #region Order

    Task<Return<bool>> CustomerCreateOrderAsync(CreateOrderReqDto req);
    Task<Return<CustomerGetOrderDetailResDto>> GetOrderByIdAsync(Guid orderId);

    #endregion

    #region Admin

    Task<Return<List<ManagerGetOrdersResDto>>> ManagerGetOrdersAsync(string statusFilter);

    #endregion
}