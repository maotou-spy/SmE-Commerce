using SmE_CommerceModels.RequestDtos.Payment;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IPaymentService
{
    Task<Return<bool>> CreatePaymentAsync(CreatePaymentReqDto reqDto);
}