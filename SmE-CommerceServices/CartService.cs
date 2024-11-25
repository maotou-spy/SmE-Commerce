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
                    ErrorCode = ErrorCodes.InvalidInput,
                    Message = ErrorMessage.InvalidInput
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
                    ErrorCode = ErrorCodes.NotAuthority,
                    Message = currentCustomer.Message
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
                    ErrorCode = ErrorCodes.ProductNotFound,
                    Message = ErrorMessage.ProductNotFound
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
                        ErrorCode = ErrorCodes.OutOfStock,
                        Message = ErrorMessage.OutOfStock
                    };
                }

                // Update existing cart item
                existingCartItem.Data.Quantity = newQuantity;
                var updatedCart = await cartRepository.UpdateCartItemAsync(existingCartItem.Data);

                if (!updatedCart.IsSuccess)
                {
                    throw new Exception(updatedCart.Message);
                }

                // Complete transaction and return success
                transactionScope.Complete();
                return new Return<bool>
                {
                    Data = true,
                    IsSuccess = true,
                    ErrorCode = ErrorCodes.Ok,
                    Message = SuccessMessage.Updated
                };
            }

            // Check if stock is sufficient for new cart item
            if (existingProduct.Data.StockQuantity < cartItem.Quantity)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    ErrorCode = ErrorCodes.OutOfStock,
                    Message = ErrorMessage.OutOfStock
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
                throw new Exception(addedResult.Message);
            }

            // Complete transaction and return success
            transactionScope.Complete();
            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                ErrorCode = ErrorCodes.Ok,
                Message = SuccessMessage.Created
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                ErrorCode = ErrorCodes.InternalServerError,
                Message = ErrorMessage.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }
}