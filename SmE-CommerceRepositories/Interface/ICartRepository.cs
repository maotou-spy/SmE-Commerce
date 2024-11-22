using SmE_CommerceModels.Models;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceRepositories.Interface;

public interface ICartRepository
{
    Task<Return<CartItem>> GetCartItemById(Guid cartId);
    Task<Return<CartItem>> GetCartItemByProductIdAndUserIdAsync(Guid productId, Guid userId);
    Task<Return<List<CartItem>>> GetCartItemsByUserId(Guid userId, int? pageIndex, int? pageSize);
    Task<Return<List<CartItem>>> GetCartItems();
    Task<Return<bool>> AddToCartAsync(CartItem cartItem);
    Task<Return<bool>> SyncCart(List<CartItem> carts);
    Task<Return<bool>> UpdateCartItemAsync(CartItem cartItem);
    Task<Return<bool>> DeleteCartItem(Guid cartId);
}
