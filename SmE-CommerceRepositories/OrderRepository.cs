using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories;

public class OrderRepository(SmECommerceContext defaultdb) : IOrderRepository
{
    public async Task<Return<bool>> CreateOrder(Order order)
    {
        try
        {
            await defaultdb.Orders.AddAsync(order);
            await defaultdb.SaveChangesAsync();
            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                Message = SuccessfulMessage.Created
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex,
                TotalRecord = 0
            };
        }
    }
}
