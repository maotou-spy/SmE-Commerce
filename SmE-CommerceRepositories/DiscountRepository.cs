using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories
{
    public class DiscountRepository(SmECommerceContext dbContext) : IDiscountRepository
    {
        public async Task<Return<Discount>> AddDiscountAsync(Discount discount)
        {
            try
            {
                await dbContext.Discounts.AddAsync(discount);
                await dbContext.SaveChangesAsync();

                return new Return<Discount>
                {
                    Data = discount,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Created,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<Discount>()
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<Discount>> GetDiscountByNameAsync(string name)
        {
            try
            {
                var discount = await dbContext.Discounts.FirstOrDefaultAsync(x => x.DiscountName == name);

                return new Return<Discount>
                {
                    Data = discount,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Found,
                    TotalRecord = discount == null ? 0 : 1
                };
            }
            catch (Exception e)
            {
                return new Return<Discount>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = e
                };
            }
        }

        public async Task<Return<Discount>> UpdateDiscountAsync(Discount discount)
        {
            try
            {
                dbContext.Discounts.Update(discount);
                await dbContext.SaveChangesAsync();

                return new Return<Discount>
                {
                    Data = discount,
                    IsSuccess = true,
                    Message = SuccessfulMessage.Updated,
                    TotalRecord = 1
                };
            } catch (Exception e)
            {
                return new Return<Discount>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = e
                };
            }
        }
    }
}