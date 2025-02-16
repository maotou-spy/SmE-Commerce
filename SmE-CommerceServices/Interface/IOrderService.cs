using SmE_CommerceModels.RequestDtos.Order;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IOrderService
{
    #region Order

    Task<Return<bool>> CustomerCreateOrderAsync(CreateOrderReqDto req);

    #endregion
}