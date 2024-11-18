using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Discount;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IDiscountService
{
    Task<Return<bool>> AddDiscountAsync(AddDiscountReqDto discount);
}