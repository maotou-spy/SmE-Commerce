using SmE_CommerceModels.RequestDtos.Cart;
using SmE_CommerceModels.ResponseDtos.Cart;
using SmE_CommerceModels.ReturnResult;

namespace SmE_CommerceServices.Interface;

public interface ICartService
{
    Task<Return<List<GetCartResDto>>> CustomerGetCartAsync();

    Task<Return<int?>> AddToCartAsync(CartItemReqDto cartItem);

    Task<Return<int?>> UpdateCartItemAsync(Guid cartId, int updatedQuantity);

    Task<Return<bool>> CustomerRemoveCartItemByIdAsync(Guid cartId);

    Task<Return<bool>> CustomerClearCartAsync();
}
