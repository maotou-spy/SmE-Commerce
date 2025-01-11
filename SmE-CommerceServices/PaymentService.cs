using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Payment;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class PaymentService(IPaymentRepository paymentRepository, IOrderRepository orderRepository, IHelperService helperService) : IPaymentService
{
    public async Task<Return<bool>> CreatePaymentAsync(CreatePaymentReqDto reqDto)
    {
        try
        {
            // validate user
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Customer);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentCustomer.StatusCode,
                    TotalRecord = 0
                };
            }
            
            // validate payment method
            var paymentMethod = await paymentRepository.GetPaymentMethodByIdAsync(reqDto.PaymentMethodId);
            if (!paymentMethod.IsSuccess || paymentMethod.Data == null)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = paymentMethod.StatusCode,
                    InternalErrorMessage = paymentMethod.InternalErrorMessage
                };
            }
            
            // validate order
            var order = await orderRepository.GetOrderByIdAsync(reqDto.OrderId);
            if (!order.IsSuccess || order.Data == null)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = order.StatusCode,
                    InternalErrorMessage = order.InternalErrorMessage
                };
            }    
            
            // validate amount
            if (reqDto.Amount <= 0)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidAmount
                };
            }
            
            var payment = new Payment
            {
                PaymentMethodId = reqDto.PaymentMethodId,
                OrderId = reqDto.OrderId,
                Amount = reqDto.Amount,
                CreatedAt = DateTime.Now,
                CreateById = currentCustomer.Data.UserId
            };
            
            var result = await paymentRepository.CreatePaymentAsync(payment);
            if (!result.IsSuccess)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage
                };
            }
            
            return new Return<bool>
            {
                Data = result.Data,
                IsSuccess = result.IsSuccess,
                StatusCode = result.StatusCode,
                InternalErrorMessage = result.InternalErrorMessage
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e
            };
        }
    }
}