using System.Transactions;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Cart;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class CartService(ICartRepository cartRepository, IProductRepository productRepository, IHelperService helperService) : ICartService
{
    public async Task<Return<bool>> AddToCart(CartItemReqDto cartItem)
    {
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Validate cart item input
            if (cartItem.ProductId == Guid.Empty || cartItem.Quantity <= 0)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ValidationError
                };
            }

            // Get current customer
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Customer);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.NotAuthority
                };
            }

            // Check if the product exists and is valid
            var existingProduct = await productRepository.GetProductByIdAsync(cartItem.ProductId);
            if (!existingProduct.IsSuccess || existingProduct.Data is not { Status: ProductStatus.Active })
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound
                };
            }

            // Check if the cart item exists for the current user
            var existingCartItem =
                await cartRepository.GetCartItemByProductIdAndUserIdAsync(cartItem.ProductId, currentCustomer.Data.UserId);

            if (existingCartItem is { IsSuccess: true, Data: not null })
            {
                var newQuantity = existingCartItem.Data.Quantity + cartItem.Quantity;

                // Check if the new quantity exceeds stock
                if (existingProduct.Data.StockQuantity < newQuantity)
                {
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.OutOfStock
                    };
                }

                // Update existing cart item
                existingCartItem.Data.Quantity = newQuantity;
                var updatedCart = await cartRepository.UpdateCartItemAsync(existingCartItem.Data);
                if (!updatedCart.IsSuccess)
                {
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = updatedCart.StatusCode,
                        InternalErrorMessage = updatedCart.InternalErrorMessage
                    };
                }

                // Complete transaction and return success
                transactionScope.Complete();
                return new Return<bool>
                {
                    Data = true,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok
                };
            }

            // Check if stock is sufficient for new cart item
            if (existingProduct.Data.StockQuantity < cartItem.Quantity)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.OutOfStock
                };
            }

            // Add new cart item
            var newCartItem = new CartItem
            {
                ProductId = cartItem.ProductId,
                UserId = currentCustomer.Data.UserId,
                Quantity = cartItem.Quantity,
                Price = existingProduct.Data.Price
            };

            var addedResult = await cartRepository.AddToCartAsync(newCartItem);

            if (!addedResult.IsSuccess)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = addedResult.StatusCode,
                    InternalErrorMessage = addedResult.InternalErrorMessage
                };
            }

            // Complete transaction and return success
            transactionScope.Complete();
            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }
}
