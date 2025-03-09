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
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(
                RoleEnum.Customer
            );
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                return new Return<List<GetCartResDto>>
                {
                    Data = [],
                    IsSuccess = false,
                    StatusCode = ErrorCode.NotAuthority,
                    InternalErrorMessage = currentCustomer.InternalErrorMessage,
                };

            var cartItems = await cartRepository.GetCartItemsByUserIdAsync(
                currentCustomer.Data.UserId
            );
            if (!cartItems.IsSuccess)
                return new Return<List<GetCartResDto>>
                {
                    Data = [],
                    IsSuccess = false,
                    StatusCode = cartItems.StatusCode,
                    InternalErrorMessage = cartItems.InternalErrorMessage,
                };

            if (cartItems.Data == null || cartItems.Data.Count == 0)
                return new Return<List<GetCartResDto>>
                {
                    Data = [],
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                };

            var cartItemsToUpdate = new List<CartItem>();
            var cartItemsRes = cartItems
                .Data.Select(cartItem =>
                {
                    var productVariant = cartItem.ProductVariant;
                    var product = cartItem.Product;

                    var isProductActive = product.Status == ProductStatus.Active;
                    var isVariantActive =
                        productVariant == null || productVariant.Status == ProductStatus.Active;

                    // Only product has variant, price is null
                    var hasError = productVariant is null && product.Price == null;

                    var isAvailable = isProductActive && isVariantActive && !hasError;

                    var stockQuantity = productVariant?.StockQuantity ?? product.StockQuantity;
                    if (stockQuantity < 0) // Ensure stock quantity is not negative
                        stockQuantity = 0;

                    var quantity = cartItem.Quantity;
                    var isQuantityUpdated = false;

                    if (quantity > stockQuantity)
                    {
                        quantity = stockQuantity;
                        isQuantityUpdated = true;
                    }

                    if (!isAvailable)
                        quantity = 0;

                    var isPriceUpdated = cartItem.Price != (productVariant?.Price ?? product.Price);

                    if (!isPriceUpdated && !isQuantityUpdated)
                        return new GetCartResDto
                        {
                            CartItemId = cartItem.CartItemId,
                            ProductId = cartItem.ProductId,
                            ProductVariantId = cartItem.ProductVariantId,
                            ProductName = product.Name,
                            ImageUrl = product.PrimaryImage,
                            ProductSlug = product.Slug,
                            Quantity = quantity,
                            StockQuantity = stockQuantity,
                            Price = productVariant?.Price ?? product.Price ?? 0,
                            IsPriceUpdated = isPriceUpdated,
                            IsQuantityUpdated = isQuantityUpdated,
                            Status = product.Status,
                        };
                    cartItem.Price = productVariant?.Price ?? product.Price ?? 0;
                    cartItem.Quantity = quantity;
                    cartItemsToUpdate.Add(cartItem);

                    return new GetCartResDto
                    {
                        CartItemId = cartItem.CartItemId,
                        ProductId = cartItem.ProductId,
                        ProductVariantId = cartItem.ProductVariantId,
                        ProductName = product.Name,
                        ImageUrl = product.PrimaryImage,
                        ProductSlug = product.Slug,
                        Quantity = quantity,
                        StockQuantity = stockQuantity,
                        Price = product.Price ?? 0,
                        IsPriceUpdated = isPriceUpdated,
                        IsQuantityUpdated = isQuantityUpdated,
                        Status = product.Status,
                    };
                })
                .ToList();

            if (cartItemsToUpdate.Count <= 0)
                return new Return<List<GetCartResDto>>
                {
                    Data = cartItemsRes,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                    TotalRecord = cartItemsRes.Count,
                };

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var updateResult = await cartRepository.UpdateCartItemRangeAsync(cartItemsToUpdate);
                if (!updateResult.IsSuccess)
                    return new Return<List<GetCartResDto>>
                    {
                        Data = [],
                        IsSuccess = false,
                        StatusCode = updateResult.StatusCode,
                        InternalErrorMessage = updateResult.InternalErrorMessage,
                    };
                transaction.Complete();
            }

            return new Return<List<GetCartResDto>>
            {
                Data = cartItemsRes,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = cartItemsRes.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<List<GetCartResDto>>
            {
                Data = [],
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<int?>> AddToCartAsync(CartItemReqDto cartItem)
    {
        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Validate cart item input
            if (cartItem.Quantity <= 0)
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.BadRequest,
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
                    InternalErrorMessage = currentCustomer.InternalErrorMessage,
                };

            decimal price;
            int stockQuantity;

            // Check if the product has a variant
            if (cartItem.ProductVariantId is not null)
            {
                // Case 1: Product has variant
                var existingProductVariant = await productRepository.GetProductVariantByIdAsync(
                    cartItem.ProductVariantId ?? Guid.Empty
                );
                if (
                    !existingProductVariant.IsSuccess
                    || existingProductVariant.Data is not { Status: ProductStatus.Active }
                    || existingProductVariant.Data.Product.Status != ProductStatus.Active
                )
                    return new Return<int?>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = ErrorCode.ProductNotFound,
                    };

                if (existingProductVariant.Data.Price <= 0)
                    return new Return<int?>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidPrice,
                    };

                price = existingProductVariant.Data.Price;
                stockQuantity = existingProductVariant.Data.StockQuantity;
            }
            else
            {
                // Case 2: Product has no variant
                var existingProduct = await productRepository.GetProductByIdAsync(
                    cartItem.ProductId
                );
                if (
                    !existingProduct.IsSuccess
                    || existingProduct.Data is not { Status: ProductStatus.Active }
                )
                    return new Return<int?>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = ErrorCode.ProductNotFound,
                    };

                if (existingProduct.Data.Price <= 0)
                    return new Return<int?>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidPrice,
                    };

                price = existingProduct.Data.Price ?? 0;
                stockQuantity = existingProduct.Data.StockQuantity;
            }

            // Check if stock is sufficient
            if (stockQuantity < cartItem.Quantity)
                return new Return<int?>
                {
                    Data = stockQuantity, // Return remaining stock
                    IsSuccess = false,
                    StatusCode = ErrorCode.OutOfStock,
                };

            // Check if the cart item already exists
            var existingCartItem = await cartRepository.GetCartItemByProductIdAndUserIdAsync(
                cartItem.ProductId,
                currentCustomer.Data.UserId
            );

            // Update existing cart item
            if (existingCartItem is { IsSuccess: true, Data: not null })
            {
                var newQuantity = existingCartItem.Data.Quantity + cartItem.Quantity;

                if (stockQuantity < newQuantity)
                    return new Return<int?>
                    {
                        Data = stockQuantity, // Return remaining stock
                        IsSuccess = false,
                        StatusCode = ErrorCode.OverStockQuantity,
                    };

                existingCartItem.Data.Quantity = newQuantity;
                existingCartItem.Data.Price = price;
                var updatedCart = await cartRepository.UpdateCartItemAsync(existingCartItem.Data);

                if (!updatedCart.IsSuccess)
                    return new Return<int?>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = updatedCart.StatusCode,
                        InternalErrorMessage = updatedCart.InternalErrorMessage,
                    };

                transactionScope.Complete();
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                };
            }

            // Add new cart item
            var newCartItem = new CartItem
            {
                ProductVariantId =
                    cartItem.ProductVariantId != Guid.Empty ? cartItem.ProductVariantId : null,
                ProductId = cartItem.ProductId,
                UserId = currentCustomer.Data.UserId,
                Quantity = cartItem.Quantity,
                Price = price,
            };

            var addedResult = await cartRepository.AddToCartAsync(newCartItem);

            if (!addedResult.IsSuccess)
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = addedResult.StatusCode,
                    InternalErrorMessage = addedResult.InternalErrorMessage,
                };

            transactionScope.Complete();
            return new Return<int?>
            {
                Data = null,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<int?>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
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
                    InternalErrorMessage = currentCustomer.InternalErrorMessage,
                };

            // Check if the cart item exists
            var existingCartItem = await cartRepository.GetCartItemByIdAsync(cartId);
            if (existingCartItem is not { IsSuccess: true, Data: not null })
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CartNotFound,
                };

            // Check if the cart item belongs to the current customer
            if (existingCartItem.Data.UserId != currentCustomer.Data.UserId)
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CartNotFound,
                };

            // Validate updated quantity
            if (updatedQuantity <= 0)
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = ErrorCode.BadRequest,
                };

            // Check product or product variant
            if (existingCartItem.Data.ProductVariantId.HasValue)
            {
                // Case: Product Variant exists
                var existingProductVariant = await productRepository.GetProductVariantByIdAsync(
                    existingCartItem.Data.ProductVariantId.Value
                );

                if (
                    !existingProductVariant.IsSuccess
                    || existingProductVariant.Data is not { Status: ProductStatus.Active }
                )
                    return new Return<int?>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = ErrorCode.ProductNotFound, // Variant not active
                    };

                // Check if stock is sufficient for the updated quantity
                if (existingProductVariant.Data.StockQuantity < updatedQuantity)
                    return new Return<int?>
                    {
                        Data = existingProductVariant.Data.StockQuantity, // Return remaining stock
                        IsSuccess = false,
                        StatusCode = ErrorCode.OverStockQuantity,
                    };
            }
            else
            {
                // Case: No Product Variant → Check Product
                var existingProduct = await productRepository.GetProductByIdAsync(
                    existingCartItem.Data.ProductId
                );

                if (
                    !existingProduct.IsSuccess
                    || existingProduct.Data is not { Status: ProductStatus.Active }
                )
                    return new Return<int?>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = ErrorCode.ProductNotFound,
                    };

                // Ensure product does not have variants (avoid mix-up)
                if (existingProduct.Data.ProductVariants.Count > 0)
                    return new Return<int?>
                    {
                        Data = null,
                        IsSuccess = false,
                        StatusCode = ErrorCode.BadRequest,
                    };

                // Check if stock is sufficient for the updated quantity
                if (existingProduct.Data.StockQuantity < updatedQuantity)
                    return new Return<int?>
                    {
                        Data = existingProduct.Data.StockQuantity, // Return remaining stock
                        IsSuccess = false,
                        StatusCode = ErrorCode.OverStockQuantity,
                    };
            }

            // Update existing cart item
            existingCartItem.Data.Quantity = updatedQuantity;
            var updatedCart = await cartRepository.UpdateCartItemAsync(existingCartItem.Data);

            if (!updatedCart.IsSuccess)
                return new Return<int?>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = updatedCart.StatusCode,
                    InternalErrorMessage = updatedCart.InternalErrorMessage,
                };

            transactionScope.Complete();
            return new Return<int?>
            {
                Data = updatedQuantity,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception ex)
        {
            return new Return<int?>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
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
                    InternalErrorMessage = currentCustomer.InternalErrorMessage,
                };

            // Check if the cart item exists
            var existingCartItem = await cartRepository.GetCartItemByIdAsync(cartId);
            if (existingCartItem is not { IsSuccess: true, Data: not null })
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CartNotFound,
                };

            // Check if the cart item belongs to the current customer
            if (existingCartItem.Data.UserId != currentCustomer.Data.UserId)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.CartNotFound,
                };

            // Remove cart item
            var deletedCart = await cartRepository.RemoveCartItemByIdAsync(cartId);
            if (!deletedCart.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = deletedCart.StatusCode,
                    InternalErrorMessage = deletedCart.InternalErrorMessage,
                };

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
                    InternalErrorMessage = currentCustomer.InternalErrorMessage,
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
                    InternalErrorMessage = deletedCart.InternalErrorMessage,
                };

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
