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
        #region Discount
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
                    Message = SuccessMessage.Created,
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
                    Message = SuccessMessage.Found,
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
                    Message = SuccessMessage.Updated,
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

        public async Task<Return<Discount>> GetDiscountByIdAsync(Guid id)
        {
            try
            {
                var discount = await dbContext.Discounts.FirstOrDefaultAsync(x => x.DiscountId == id);

                return new Return<Discount>
                {
                    Data = discount,
                    IsSuccess = true,
                    Message = SuccessMessage.Found,
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
        #endregion

        #region DiscountCode
        public async Task<Return<DiscountCode>> AddDiscountCodeAsync(DiscountCode discountCode)
        {
            try
            {
                await dbContext.DiscountCodes.AddAsync(discountCode);
                await dbContext.SaveChangesAsync();

                return new Return<DiscountCode>
                {
                    Data = discountCode,
                    IsSuccess = true,
                    Message = SuccessMessage.Created,
                    TotalRecord = 1
                };
            }
            catch (Exception ex)
            {
                return new Return<DiscountCode>()
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = ex,
                    TotalRecord = 0
                };
            }
        }

        public async Task<Return<DiscountCode>> UpdateDiscountCodeAsync(DiscountCode discountCode)
        {
            try
            {
                dbContext.DiscountCodes.Update(discountCode);
                await dbContext.SaveChangesAsync();

                return new Return<DiscountCode>
                {
                    Data = discountCode,
                    IsSuccess = true,
                    Message = SuccessMessage.Updated,
                    TotalRecord = 1
                };
            }
            catch (Exception e)
            {
                return new Return<DiscountCode>()
                {
                    Data = null,
                    IsSuccess = false,
                    Message = ErrorMessage.InternalServerError,
                    InternalErrorMessage = e,
                    TotalRecord = 0
                };
            }
        }
        #endregion
    }
}