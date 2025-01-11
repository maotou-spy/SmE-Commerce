using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface IPaymentRepository
{
    #region Payment

    Task<Return<bool>> CreatePaymentAsync(Payment payment);

    #endregion

    #region Payment Method

    Task<Return<PaymentMethod>> GetPaymentMethodByIdAsync(Guid paymentMethodId);

    #endregion
}