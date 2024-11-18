using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface IDiscountRepository
{
    Task<Return<Discount>> AddDiscountAsync(Discount discount);
    Task<Return<Discount>> GetDiscountByNameAsync(string name);
    Task<Return<Discount>> UpdateDiscountAsync(Discount discount);
}