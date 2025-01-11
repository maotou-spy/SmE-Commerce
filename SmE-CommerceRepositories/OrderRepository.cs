using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories;

public class OrderRepository(SmECommerceContext defaultdb) : IOrderRepository
{
    public async Task<Return<Order>> CreateOrderAsync(Order order)
    {
        try
        {
            await defaultdb.Orders.AddAsync(order);
            await defaultdb.SaveChangesAsync();
            return new Return<Order>
            {
                Data = order,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            return new Return<Order>
            {
                Data = null,
                StatusCode = ErrorCode.InternalServerError,
                IsSuccess = false,
                TotalRecord = 0,
                InternalErrorMessage = ex
            };
        }
    }
    
    public async Task<Return<Order>> GetOrderByIdAsync(Guid orderId)
    {
        try
        {
            var order = await defaultdb.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (order == null)
            {
                return new Return<Order>
                {
                    Data = null,
                    StatusCode = ErrorCode.OrderNotFound,
                    IsSuccess = false,
                    TotalRecord = 0
                };
            }
            return new Return<Order>
            {
                Data = order,
                StatusCode = ErrorCode.Ok,
                IsSuccess = true,
                TotalRecord = 1
            };
        }
        catch (Exception ex)
        {
            return new Return<Order>
            {
                Data = null,
                StatusCode = ErrorCode.InternalServerError,
                IsSuccess = false,
                TotalRecord = 0,
                InternalErrorMessage = ex
            };
        }
    }
}
