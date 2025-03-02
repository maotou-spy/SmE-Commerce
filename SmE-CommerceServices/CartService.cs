using System.Transactions;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Cart;
using SmE_CommerceModels.ResponseDtos.Cart;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class CartService(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IHelperService helperService
) : ICartService
{
    public async Task<Return<List<GetCartResDto>>> CustomerGetCartAsync()
    {
        try
        {
            // Get current customer
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(
                RoleEnum.Customer
            );
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                return new Return<List<GetCartResDto>>
                {
                    Data = [],
                    IsSuccess = false,
                    StatusCode = ErrorCode.NotAuthority,
                    InternalErrorMessage = currentCustomer.InternalErrorMessage
                };

            // Get cart items
            var cartItems = await cartRepository.GetCartItemsByUserIdAsync(
                currentCustomer.Data.UserId
            );
            if (!cartItems.IsSuccess)
                return new Return<List<GetCartResDto>>
                {
                    Data = [],
                    IsSuccess = false,
                    StatusCode = cartItems.StatusCode,
                    InternalErrorMessage = cartItems.InternalErrorMessage
                };

            // Check if cart is empty
            if (cartItems.Data == null || cartItems.Data.Count == 0)
                return new Return<List<GetCartResDto>>
                {
                    Data = [],
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok
                };

            // Map cart items to response DTO
            var cartItemsRes = cartItems
                .Data.Where(_ => true)
                .Select(cartItem =>
                {
                    var isPriceUpdated =
                        cartItem.Price != cartItem.ProductVariant.Product.Price;

                    return new GetCartResDto
                    {
                        CartItemId = cartItem.CartItemId,
                        ProductVariantId = cartItem.ProductVariantId,
                        ProductName = cartItem.ProductVariant.Product.Name,
                        ImageUrl = cartItem.ProductVariant.Product.PrimaryImage,
                        ProductSlug = cartItem.ProductVariant.Product.Slug,
                        Quantity =
                            cartItem.ProductVariant.Product.Status == ProductStatus.Active ? cartItem.Quantity : 0,
                        StockQuantity = cartItem.ProductVariant.Product.StockQuantity,
                        Price = cartItem.ProductVariant.Product.Price,
                        IsPriceUpdated = isPriceUpdated,
                        ProductStatus = cartItem.ProductVariant.Product.Status
                    };
                })
                .ToList();

            return new Return<List<GetCartResDto>>
            {
                Data = cartItemsRes,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<List<GetCartResDto>>
            {
                Data = [],
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<int?>> AddToCartAsync(CartItemReqDto cartItem)
    {
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Validate cart item input
            if (cartItem.ProductVariantId == Guid.Empty || cartItem.Quantity <= 0)
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.BadRequest
                };

            // Get current customer
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(
                RoleEnum.Customer
            );
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.NotAuthority,
                    InternalErrorMessage = currentCustomer.InternalErrorMessage
                };

            // Check if the product variant is existing and is active
            var existingProduct = await productRepository.GetProductByProductVariantIdAsync(cartItem.ProductVariantId);
            if (!existingProduct.IsSuccess || existingProduct.Data == null)
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound
                };
            if (existingProduct.Data.Price <= 0)
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidPrice
                };

            // Check if stock is sufficient for add new cart item
            if (existingProduct.Data.StockQuantity < cartItem.Quantity)
                return new Return<int?>
                {
                    Data = existingProduct.Data.StockQuantity, // Return remaining stock
                    IsSuccess = false,
                    StatusCode = ErrorCode.OutOfStock
                };

            // Check if the cart item already exists
            // * If exists, update the quantity
            // => Update existing cart item
            var existingCartItem = await cartRepository.GetCartItemByProductVariantIdAndUserIdAsync(
                cartItem.ProductVariantId,
                currentCustomer.Data.UserId
            );

            if (existingCartItem is { IsSuccess: true, Data: not null })
            {
                var newQuantity = existingCartItem.Data.Quantity + cartItem.Quantity;

                if (existingProduct.Data.StockQuantity < newQuantity)
                    return new Return<int?>
                    {
                        Data = existingProduct.Data.StockQuantity, // Return remaining stock
                        IsSuccess = false,
                        StatusCode = ErrorCode.OverStockQuantity
                    };

                existingCartItem.Data.Quantity = newQuantity;
                var updatedCart = await cartRepository.UpdateCartItemAsync(existingCartItem.Data);

                if (!updatedCart.IsSuccess)
                    return new Return<int?>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = updatedCart.StatusCode,
                        InternalErrorMessage = updatedCart.InternalErrorMessage
                    };

                // Complete transaction and return success
                transactionScope.Complete();
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok
                };
            }

            // * If not exists, add new cart item
            // => Add new cart item
            var newCartItem = new CartItem
            {
                ProductVariantId = cartItem.ProductVariantId,
                UserId = currentCustomer.Data.UserId,
                Quantity = cartItem.Quantity,
                Price = existingProduct.Data.Price
            };

            var addedResult = await cartRepository.AddToCartAsync(newCartItem);

            if (!addedResult.IsSuccess)
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = addedResult.StatusCode,
                    InternalErrorMessage = addedResult.InternalErrorMessage
                };

            // Complete transaction and return success
            transactionScope.Complete();
            return new Return<int?>
            {
                Data = null,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<int?>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<int?>> UpdateCartItemAsync(Guid cartId, int updatedQuantity)
    {
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Get current customer
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(
                RoleEnum.Customer
            );
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.NotAuthority,
                    InternalErrorMessage = currentCustomer.InternalErrorMessage
                };

            // Check if the cart item exists
            var existingCartItem = await cartRepository.GetCartItemByIdAsync(cartId);
            if (existingCartItem is not { IsSuccess: true, Data: not null })
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CartNotFound
                };

            // Check if the cart item belongs to the current customer
            if (existingCartItem.Data.UserId != currentCustomer.Data.UserId)
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CartNotFound
                };

            // Check if the product exists and is active
            var existingProduct = await productRepository.GetProductByProductVariantIdAsync(
                existingCartItem.Data.ProductVariantId
            );
            if (
                !existingProduct.IsSuccess
                || existingProduct.Data is not { Status: ProductStatus.Active }
            )
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound
                };

            // Check if stock is sufficient for the updated quantity
            if (existingProduct.Data.StockQuantity < updatedQuantity)
                return new Return<int?>
                {
                    Data = existingProduct.Data.StockQuantity, // Return remaining stock
                    IsSuccess = false,
                    StatusCode = ErrorCode.OverStockQuantity
                };

            // Update existing cart item
            existingCartItem.Data.Quantity = updatedQuantity;
            var updatedCart = await cartRepository.UpdateCartItemAsync(existingCartItem.Data);

            if (!updatedCart.IsSuccess)
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = updatedCart.StatusCode,
                    InternalErrorMessage = updatedCart.InternalErrorMessage
                };

            // Complete transaction and return success
            transactionScope.Complete();
            return new Return<int?>
            {
                Data = updatedQuantity, // Return the updated quantity
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception ex)
        {
            return new Return<int?>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex
            };
        }
    }

    public async Task<Return<bool>> CustomerRemoveCartItemByIdAsync(Guid cartId)
    {
        try
        {
            // Get current customer
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(
                RoleEnum.Customer
            );
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.NotAuthority,
                    InternalErrorMessage = currentCustomer.InternalErrorMessage
                };

            // Check if the cart item exists
            var existingCartItem = await cartRepository.GetCartItemByIdAsync(cartId);
            if (existingCartItem is not { IsSuccess: true, Data: not null })
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CartNotFound
                };

            // Check if the cart item belongs to the current customer
            if (existingCartItem.Data.UserId != currentCustomer.Data.UserId)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CartNotFound
                };

            // Remove cart item
            var deletedCart = await cartRepository.RemoveCartItemByIdAsync(cartId);
            if (!deletedCart.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = deletedCart.StatusCode,
                    InternalErrorMessage = deletedCart.InternalErrorMessage
                };

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

    public async Task<Return<bool>> CustomerClearCartAsync()
    {
        try
        {
            // Get current customer
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(
                RoleEnum.Customer
            );
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.NotAuthority,
                    InternalErrorMessage = currentCustomer.InternalErrorMessage
                };

            // Remove cart items
            var deletedCart = await cartRepository.ClearCartByUserIdAsync(
                currentCustomer.Data.UserId
            );
            if (!deletedCart.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = deletedCart.StatusCode,
                    InternalErrorMessage = deletedCart.InternalErrorMessage
                };

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
