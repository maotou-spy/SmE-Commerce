using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories;

public class PaymentRepository(SmECommerceContext dbContext) : IPaymentRepository
{
    #region Payment

    public async Task<Return<bool>> CreatePaymentAsync(Payment payment)
    {
        try
        {
            await dbContext.Payments.AddAsync(payment);
            await dbContext.SaveChangesAsync();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    #endregion

    #region Payment Method

    public async Task<Return<PaymentMethod>> GetPaymentMethodByIdAsync(Guid paymentMethodId)
    {
        try
        {
            var paymentMethod = await dbContext.PaymentMethods.FirstOrDefaultAsync(x =>
                x.PaymentMethodId == paymentMethodId
            );
            if (paymentMethod == null)
                return new Return<PaymentMethod>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.PaymentMethodNotFound,
                };

            return new Return<PaymentMethod>
            {
                Data = paymentMethod,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception e)
        {
            return new Return<PaymentMethod>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    #endregion
}
