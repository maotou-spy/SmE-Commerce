using Microsoft.EntityFrameworkCore;
using SmE_CommerceModels.DBContext;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceRepositories;

public class CartRepository(SmECommerceContext defaultdbContext) : ICartRepository
{
    public async Task<Return<CartItem>> GetCartItemByCustomerIdAndIdForUpdateAsync(
        Guid customerId,
        Guid id
    )
    {
        try
        {
            var cartItem = await defaultdbContext.CartItems.FirstOrDefaultAsync(x =>
                x.UserId == customerId && x.CartItemId == id
            );

            if (cartItem == null)
                return new Return<CartItem>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CartNotFound,
                    TotalRecord = 0,
                };

            await defaultdbContext.Database.ExecuteSqlRawAsync(
                "SELECT * FROM public.\"CartItems\" WHERE \"cartItemId\" = {0} AND \"userId\" = {1} FOR UPDATE",
                id,
                customerId
            );

            return new Return<CartItem>
            {
                Data = cartItem,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception e)
        {
            return new Return<CartItem>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    public async Task<Return<CartItem>> GetCartItemByIdAsync(Guid cartId)
    {
        try
        {
            var cartItem = await defaultdbContext.CartItems.FirstOrDefaultAsync(x =>
                x.CartItemId == cartId
            );

            if (cartItem == null)
                return new Return<CartItem>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CartNotFound,
                    TotalRecord = 0,
                };

            await defaultdbContext.Database.ExecuteSqlRawAsync(
                "SELECT * FROM public.\"CartItems\" WHERE \"cartItemId\" = {0} FOR UPDATE",
                cartId
            );

            return new Return<CartItem>
            {
                Data = cartItem,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception ex)
        {
            return new Return<CartItem>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<bool>> UpdateCartItemRangeAsync(List<CartItem> cartItems)
    {
        try
        {
            defaultdbContext.CartItems.UpdateRange(cartItems);
            await defaultdbContext.SaveChangesAsync();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<CartItem>> GetCartItemByProductIdAndUserIdAsync(
        Guid productId,
        Guid userId
    )
    {
        try
        {
            var cart = await defaultdbContext.CartItems.SingleOrDefaultAsync(x =>
                x.ProductId == productId && x.UserId == userId
            );

            return new Return<CartItem>
            {
                Data = cart,
                IsSuccess = true,
                TotalRecord = cart == null ? 0 : 1,
                StatusCode = cart == null ? ErrorCode.CartNotFound : ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<CartItem>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<List<CartItem>>> GetCartItemsByUserIdAsync(Guid userId)
    {
        try
        {
            var query = defaultdbContext
                .CartItems.Where(x => x.UserId == userId)
                .Include(x => x.ProductVariant)
                .Include(x => x.Product)
                .AsQueryable();

            var totalRecords = await query.CountAsync();

            var carts = await query.ToListAsync();

            return new Return<List<CartItem>>
            {
                Data = carts,
                IsSuccess = true,
                TotalRecord = totalRecords,
                StatusCode = carts.Count == 0 ? ErrorCode.CartNotFound : ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<CartItem>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<bool>> AddToCartAsync(CartItem cartItem)
    {
        try
        {
            await defaultdbContext.CartItems.AddAsync(cartItem);
            await defaultdbContext.SaveChangesAsync();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<bool>> UpdateCartItemAsync(CartItem cart)
    {
        try
        {
            defaultdbContext.CartItems.Update(cart);
            await defaultdbContext.SaveChangesAsync();
            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<bool>> RemoveCartItemByIdAsync(Guid cartId)
    {
        try
        {
            var cart = await defaultdbContext.CartItems.SingleOrDefaultAsync(x =>
                x.CartItemId == cartId
            );

            if (cart == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CartNotFound,
                };

            defaultdbContext.CartItems.Remove(cart);
            await defaultdbContext.SaveChangesAsync();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<bool>> RemoveCartItemRangeByIdAsync(List<Guid> cartId, Guid userId)
    {
        try
        {
            var carts = await defaultdbContext
                .CartItems.Where(x => cartId.Contains(x.CartItemId) && x.UserId == userId)
                .ToListAsync();

            if (carts.Count == 0)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CartNotFound,
                };

            defaultdbContext.CartItems.RemoveRange(carts);
            await defaultdbContext.SaveChangesAsync();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<bool>> ClearCartByUserIdAsync(Guid userId)
    {
        try
        {
            var carts = await defaultdbContext
                .CartItems.Where(x => x.UserId == userId)
                .ToListAsync();

            if (carts.Count == 0)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CartNotFound,
                };

            defaultdbContext.CartItems.RemoveRange(carts);
            await defaultdbContext.SaveChangesAsync();

            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }
}
