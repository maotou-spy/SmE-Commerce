using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Cart;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class CartService(ICartRepository cartRepository, IHelperService helperService) : ICartService
{
    public async Task<Return<bool>> AddToCart(CartItemReqDto cartItem)
    {
        try
        {
            var currentCustomer = await helperService.GetCurrentUserWithRole(RoleEnum.Customer);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    Message = currentCustomer.Message
                };
            }

            // Validate the cart item
            if (cartItem.Quantity <= 0 || cartItem.Price <= 0)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    Message = ErrorMessage.InvalidInput
                };
            }

            // Check if the cart item exists for the current user
            var existingCartItem = await cartRepository.GetCartItemByProductIdAndUserId(cartItem.ProductId, currentCustomer.Data.UserId);

            if (existingCartItem is { IsSuccess: true, Data: not null })
            {
                // Item already exists, so update it
                existingCartItem.Data.Quantity += cartItem.Quantity;
                existingCartItem.Data.Price = cartItem.Price;

                var updatedCart = await cartRepository.UpdateCartItem(existingCartItem.Data);
                return new Return<bool>
                {
                    Data = updatedCart.IsSuccess,
                    IsSuccess = updatedCart.IsSuccess,
                    Message = updatedCart.IsSuccess ? SuccessMessage.Updated : updatedCart.Message
                };
            }

            // Item doesn't exist, so add it
            var cart = new CartItem
            {
                ProductId = cartItem.ProductId,
                UserId = currentCustomer.Data.UserId,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price,
            };

            var result = await cartRepository.AddToCart(cart);
            return new Return<bool>
            {
                Data = result.IsSuccess,
                IsSuccess = result.IsSuccess,
                Message = result.IsSuccess ? SuccessMessage.Created : result.Message
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }
}
