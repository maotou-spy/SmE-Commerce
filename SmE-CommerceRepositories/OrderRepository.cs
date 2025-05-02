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
                IsSuccess = true,
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
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<Order>> CustomerGetOrderByIdAsync(Guid orderId, Guid userId)
    {
        try
        {
            var order = await defaultdb
                .Orders.Include(x => x.Address)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x!.VariantAttributes)
                .FirstOrDefaultAsync(x => x.OrderId == orderId && x.UserId == userId);
            if (order == null)
                return new Return<Order>
                {
                    Data = null,
                    StatusCode = ErrorCode.OrderNotFound,
                    IsSuccess = false,
                    TotalRecord = 0,
                };
            return new Return<Order>
            {
                Data = order,
                StatusCode = ErrorCode.Ok,
                IsSuccess = true,
                TotalRecord = 1,
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
                InternalErrorMessage = ex,
            };
        }
    }

    // getOrderByUserIdAsync
    public async Task<Return<List<Order>>> GetOrderByUserIdAsync(Guid userId)
    {
        try
        {
            var orders = await defaultdb.Orders.Where(x => x.UserId == userId).ToListAsync();
            if (orders.Count == 0)
                return new Return<List<Order>>
                {
                    Data = null,
                    StatusCode = ErrorCode.OrderNotFound,
                    IsSuccess = false,
                    TotalRecord = 0,
                };
            return new Return<List<Order>>
            {
                Data = orders,
                StatusCode = ErrorCode.Ok,
                IsSuccess = true,
                TotalRecord = orders.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<Order>>
            {
                Data = null,
                StatusCode = ErrorCode.InternalServerError,
                IsSuccess = false,
                TotalRecord = 0,
                InternalErrorMessage = ex,
            };
        }
    }

    // Manager get order by status and userId
    public async Task<Return<List<Order>>> GetOrdersByStatusAndUserIdAsync(
        Guid? userId,
        string statusFilter
    )
    {
        try
        {
            var query = defaultdb
                .Orders.Include(x => x.Address)
                .Include(x => x.User)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x!.Product)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId);

            query = query.Where(x => x.Status == statusFilter);

            var ordersList = await query.ToListAsync();

            if (ordersList.Count == 0)
                return new Return<List<Order>>
                {
                    Data = null,
                    StatusCode = ErrorCode.OrderNotFound,
                    IsSuccess = false,
                    TotalRecord = 0,
                };

            return new Return<List<Order>>
            {
                Data = ordersList,
                StatusCode = ErrorCode.Ok,
                IsSuccess = true,
                TotalRecord = ordersList.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<Order>>
            {
                Data = null,
                StatusCode = ErrorCode.InternalServerError,
                IsSuccess = false,
                TotalRecord = 0,
                InternalErrorMessage = ex,
            };
        }
    }

    #region admin

    // Manager Confirm Order
    public async Task<Return<Order>> ManagerConfirmOrderAsync(Guid orderId)
    {
        try
        {
            var order = await defaultdb.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (order == null)
                return new Return<Order>
                {
                    Data = null,
                    StatusCode = ErrorCode.OrderNotFound,
                    IsSuccess = false,
                    TotalRecord = 0,
                };
            order.Status = OrderStatus.Stuffing;
            defaultdb.Orders.Update(order);
            await defaultdb.SaveChangesAsync();
            return new Return<Order>
            {
                Data = order,
                StatusCode = ErrorCode.Ok,
                IsSuccess = true,
                TotalRecord = 1,
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
                InternalErrorMessage = ex,
            };
        }
    }

    #endregion
}
