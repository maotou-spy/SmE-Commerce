﻿using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface ICartRepository
{
    Task<Return<CartItem>> GetCartItemByIdAsync(Guid cartId);

    Task<Return<CartItem>> GetCartItemByProductIdAndUserIdAsync(
        Guid productId,
        Guid? productVariantId,
        Guid userId
    );

    Task<Return<List<CartItem>>> GetCartItemsByUserIdAsync(Guid userId);

    Task<Return<bool>> AddToCartAsync(CartItem cartItem);

    Task<Return<bool>> UpdateCartItemAsync(CartItem cartItem);

    Task<Return<bool>> UpdateCartItemRangeAsync(List<CartItem> cartItems);

    Task<Return<bool>> RemoveCartItemByIdAsync(Guid cartId);

    Task<Return<bool>> ClearCartByUserIdAsync(Guid userId);
    Task<Return<CartItem>> GetCartItemByCustomerIdAndIdForUpdateAsync(Guid customerId, Guid id);
    Task<Return<bool>> RemoveCartItemRangeByIdAsync(List<Guid> cartId, Guid userId);
}
