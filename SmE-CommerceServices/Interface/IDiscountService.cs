using SmE_CommerceModels.RequestDtos.Discount;
using SmE_CommerceModels.RequestDtos.Discount.DiscountCode;
using SmE_CommerceModels.ResponseDtos.Discount.Discount;
using SmE_CommerceModels.ResponseDtos.Discount.DiscountCode;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface IDiscountService
{
    Task<Return<bool>> AddDiscountAsync(AddDiscountReqDto discount);
    Task<Return<bool>> UpdateDiscountAsync(Guid id, UpdateDiscountReqDto discount);

    Task<Return<IEnumerable<ManagerGetDiscountsResDto>>> GetDiscountsForManagerAsync(
        string? name,
        int? pageNumber,
        int? pageSize
    );

    Task<Return<bool>> DeleteDiscountAsync(Guid id);

    Task<Return<bool>> AddDiscountCodeAsync(Guid id, AddDiscountCodeReqDto req);
    Task<Return<GetDiscountCodeResDto>> GetDiscounCodeByCodeAsync(string code);
    Task<Return<bool>> UpdateDiscountCodeAsync(Guid codeId, UpdateDiscountCodeReqDto req);
    Task<Return<GetDiscountCodeByIdResDto>> GetDiscountCodeByIdAsync(Guid codeId);
    Task<Return<bool>> DeleteDiscountCodeAsync(Guid userId);

    Task<Return<IEnumerable<GetDiscountCodeResDto>>> GetDiscountCodeByDiscountIdAsync(
        Guid discountId,
        int? pageNumber,
        int? pageSize
    );
}
