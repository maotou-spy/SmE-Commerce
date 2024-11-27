using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface IDiscountRepository
{
    Task<Return<Discount>> AddDiscountAsync(Discount discount);
    Task<Return<Discount>> GetDiscountByNameAsync(string name);
    Task<Return<Discount>> UpdateDiscountAsync(Discount discount);
    Task<Return<Discount>> GetDiscountByIdAsync(Guid id);
    Task<Return<Discount>> GetDiscountByIdForUpdateAsync(Guid id);

    Task<Return<DiscountCode>> AddDiscountCodeAsync(DiscountCode discountCode);
    Task<Return<DiscountCode>> UpdateDiscountCodeAsync(DiscountCode discountCode);
    Task<Return<DiscountCode>> GetDiscountCodeByCodeAsync(string code);
}