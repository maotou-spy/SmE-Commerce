using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Discount;
using SmE_CommerceModels.RequestDtos.Discount.DiscountCode;
using SmE_CommerceModels.ResponseDtos.Discount;
using SmE_CommerceModels.ResponseDtos.Discount.Discount;
using SmE_CommerceModels.ResponseDtos.Discount.DiscountCode;
using SmE_CommerceModels.ReturnResult;
using Task = DocumentFormat.OpenXml.Office2021.DocumentTasks.Task;

namespace SmE_CommerceServices.Interface;

public interface IDiscountService
{
    Task<Return<bool>> AddDiscountAsync(AddDiscountReqDto discount);
    Task<Return<bool>> UpdateDiscountAsync(Guid id, UpdateDiscountReqDto discount);
    Task<Return<IEnumerable<ManagerGetDiscountsResDto>>> GetDiscountsForManagerAsync(string? name, int? pageNumber, int? pageSize);

    Task<Return<bool>> AddDiscountCodeAsync(Guid id, AddDiscountCodeReqDto req);
    Task<Return<GetDiscountCodeByCodeResDto>> GetDiscounCodeByCodeAsync(string code);
    Task<Return<bool>> UpdateDiscountCodeAsync(Guid codeId, UpdateDiscountCodeReqDto req);
    Task<Return<GetDiscountCodeByIdResDto>> GetDiscountCodeByIdAsync(Guid codeId);
    Task<Return<bool>> DeleteDiscountCodeAsync(Guid userId);
}