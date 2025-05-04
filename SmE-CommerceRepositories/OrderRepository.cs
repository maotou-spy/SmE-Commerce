using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Order;
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

    public async Task<Return<Order>> GetOrderByIdAsync(Guid orderId, Guid? userId)
    {
        try
        {
            var query = defaultdb
                .Orders.Include(x => x.Address)
                .Include(x => x.User)
                .Include(x => x.DiscountCode)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.ProductVariant)
                .ThenInclude(x => x!.VariantAttributes)
                .Include(x => x.OrderItems)
                .ThenInclude(x => x.Product)
                .Where(x => x.OrderId == orderId);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            var order = await query.FirstOrDefaultAsync();

            if (order == null)
                return new Return<Order>
                {
                    Data = null,
                    StatusCode = ErrorCode.OrderNotFound,
                    IsSuccess = false,
                    TotalRecord = 0
                };

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
                    TotalRecord = 0
                };
            return new Return<List<Order>>
            {
                Data = orders,
                StatusCode = ErrorCode.Ok,
                IsSuccess = true,
                TotalRecord = orders.Count
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
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<IEnumerable<Order>>> GetOrdersAsync(
        OrderFilterReqDto filter,
        Guid userId
    )
    {
        try
        {
            var query = defaultdb.Set<Order>().AsQueryable();

            if (userId != Guid.Empty)
                query = query.Where(o => o.UserId.Equals(userId));

            // Apply search term filter across Email, Phone, and OrderCode
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(o =>
                    EF.Functions.Like(o.User.Email, $"%{searchTerm}%")
                    || (o.User.Phone != null && EF.Functions.Like(o.User.Phone, $"%{searchTerm}%"))
                    || EF.Functions.Like(o.OrderCode, $"%{searchTerm}%")
                );
            }

            if (filter.FromDate.HasValue)
            {
                var fromDateTime = filter.FromDate.Value;
                query = query.Where(o => o.CreatedAt >= fromDateTime);
            }

            if (filter.ToDate.HasValue)
            {
                var toDateTime = filter.ToDate.Value.AddDays(1).AddTicks(-1);
                query = query.Where(o => o.CreatedAt <= toDateTime);
            }

            if (!string.IsNullOrEmpty(filter.Status))
                query = query.Where(o => EF.Functions.Like(o.Status, filter.Status));

            // Apply sorting
            var sortOrder = filter.SortOrder.ToUpper();
            if (sortOrder != FilterSortOrder.Descending && sortOrder != FilterSortOrder.Ascending)
                sortOrder = FilterSortOrder.Descending; // Default sort order as Descending

            query =
                sortOrder == FilterSortOrder.Descending
                    ? query.OrderByDescending(o => o.CreatedAt)
                    : query.OrderBy(o => o.CreatedAt);

            // Pagination
            var totalCount = await query.CountAsync();
            var items = await query
                .Select(o => new Order
                {
                    OrderId = o.OrderId,
                    OrderCode = o.OrderCode,
                    UserId = o.UserId,
                    User = new User
                    {
                        UserId = o.User.UserId,
                        FullName = o.User.FullName,
                        Email = o.User.Email,
                        Phone = o.User.Phone
                    },
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    ModifiedAt = o.ModifiedAt
                })
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new Return<IEnumerable<Order>>
            {
                Data = items,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecord = totalCount
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<Order>>
            {
                Data = [],
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecord = 0
            };
        }
    }

    public async Task<Return<OrderStatusHistory>> CreateOrderStatusHistoryAsync(OrderStatusHistory req)
    {
        try
        {
            await defaultdb.OrderStatusHistories.AddAsync(req);
            await defaultdb.SaveChangesAsync();
            return new Return<OrderStatusHistory>
            {
                Data = req,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            return new Return<OrderStatusHistory>
            {
                Data = null,
                StatusCode = ErrorCode.InternalServerError,
                IsSuccess = false,
                TotalRecord = 0,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<Order>> GetOrderByIdForUpdateAsync(Guid orderId)
    {
        try
        {
            var order = await defaultdb
                .Orders
                .Include(x => x.OrderStatusHistories)
                .FirstOrDefaultAsync(x => x.OrderId == orderId);

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

            await defaultdb.Database.ExecuteSqlRawAsync(
                "SELECT * FROM public.\"Order\" WHERE \"OrderId\" = {0} FOR UPDATE", orderId);

            return new Return<Order>
            {
                Data = order,
                StatusCode = ErrorCode.Ok,
                IsSuccess = true,
                TotalRecord = 1
            };
        } catch (Exception ex)
        {
            return new Return<Order>
            {
                StatusCode = ErrorCode.InternalServerError,
                IsSuccess = false,
                TotalRecord = 0,
                InternalErrorMessage = ex
            };
        }
    }
    public async Task<List<Order>> GetOrdersByIdsAsync(IEnumerable<Guid> orderIds)
    {
        return await defaultdb
            .Orders
            .Include(x => x.OrderStatusHistories)
            .Where(o => orderIds.Contains(o.OrderId))
            .ToListAsync();
    }

    public async Task<bool> UpdateOrderStatusRangeAsync(List<Order> orders, string status, string? reason)
    {
        try
        {
            foreach (var order in orders)
            {
                order.Status = status;
                order.OrderStatusHistories.Add(new OrderStatusHistory
                {
                    OrderId = order.OrderId,
                    Status = status,
                    Reason = reason ?? null,
                    ModifiedAt = order.ModifiedAt,
                    ModifiedBy = order.ModifiedBy
                });
            }

            defaultdb.Orders.UpdateRange(orders);
            await defaultdb.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
