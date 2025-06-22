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
    public async Task<Return<List<GetCartResDto>>> GetCartAsync()
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
                    InternalErrorMessage = currentCustomer.InternalErrorMessage,
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
            var cartItemsToRemove = new List<Guid>();
            var cartItemsRes = new List<GetCartResDto>();

            foreach (var cartItem in cartItems.Data)
            {
                var product = cartItem.Product;
                var productVariant = cartItem.ProductVariant;

                // Check availability
                var isProductActive = product.Status == ProductStatus.Active;
                var isVariantActive =
                    productVariant == null || productVariant.Status == ProductStatus.Active;
                var hasError = product.HasVariant && productVariant == null;
                var isAvailable = isProductActive && isVariantActive && !hasError;

                // Get stock quantity
                var stockQuantity = productVariant?.StockQuantity ?? product.StockQuantity;
                if (stockQuantity < 0)
                    stockQuantity = 0;

                // Adjust quantity
                var quantity = cartItem.Quantity;
                var isQuantityUpdated = false;

                if (quantity > stockQuantity)
                {
                    quantity = stockQuantity;
                    isQuantityUpdated = true;
                }

                // Remove cart item if not available
                if (!isAvailable)
                {
                    cartItemsToRemove.Add(cartItem.CartItemId);
                    continue;
                }

                // Check price update
                var currentPrice = productVariant?.Price ?? product.Price;

                var isPriceUpdated = cartItem.Price != currentPrice;

                // Update cart item if needed
                if (isPriceUpdated || isQuantityUpdated)
                {
                    cartItem.Price = currentPrice;
                    cartItem.Quantity = quantity;
                    cartItemsToUpdate.Add(cartItem);
                }

                // Map to response DTO
                cartItemsRes.Add(
                    new GetCartResDto
                    {
                        CartItemId = cartItem.CartItemId,
                        ProductId = cartItem.ProductId,
                        ProductVariantId = cartItem.ProductVariantId,
                        ProductName = product.Name,
                        ImageUrl = product.PrimaryImage,
                        ProductSlug = product.Slug,
                        Quantity = quantity,
                        StockQuantity = stockQuantity,
                        Price = currentPrice,
                        IsPriceUpdated = isPriceUpdated,
                        IsQuantityUpdated = isQuantityUpdated,
                        Status = product.Status,
                    }
                );
            }

            // Update or remove cart items if needed
            if (cartItemsToUpdate.Count == 0 && cartItemsToRemove.Count == 0)
                return new Return<List<GetCartResDto>>
                {
                    Data = cartItemsRes,
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                    TotalRecord = cartItemsRes.Count,
                };

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (cartItemsToRemove.Count > 0)
                {
                    var removeResult = await cartRepository.RemoveCartItemRangeByIdAsync(
                        cartItemsToRemove,
                        currentCustomer.Data.UserId
                    );
                    if (!removeResult.IsSuccess)
                        return new Return<List<GetCartResDto>>
                        {
                            Data = [],
                            IsSuccess = false,
                            StatusCode = removeResult.StatusCode,
                            InternalErrorMessage = removeResult.InternalErrorMessage,
                        };
                }

                if (cartItemsToUpdate.Count > 0)
                {
                    var updateResult = await cartRepository.UpdateCartItemRangeAsync(
                        cartItemsToUpdate
                    );
                    if (!updateResult.IsSuccess)
                        return new Return<List<GetCartResDto>>
                        {
                            Data = [],
                            IsSuccess = false,
                            StatusCode = updateResult.StatusCode,
                            InternalErrorMessage = updateResult.InternalErrorMessage,
                        };
                }

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

    public async Task<Return<int>> AddToCartAsync(CartItemReqDto cartItem)
    {
        // Validate input
        if (cartItem.ProductId == Guid.Empty)
            return new Return<int>
            {
                IsSuccess = false,
                StatusCode = ErrorCode.BadRequest,
                Data = 0,
            };

        if (cartItem.Quantity <= 0)
            return new Return<int>
            {
                IsSuccess = false,
                StatusCode = ErrorCode.BadRequest,
                Data = 0,
            };

        using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Get current customer
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(
                RoleEnum.Customer
            );
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                return new Return<int>
                {
                    Data = 0,
                    IsSuccess = false,
                    StatusCode = ErrorCode.NotAuthority,
                    InternalErrorMessage = currentCustomer.InternalErrorMessage,
                };

            // Check product
            var productResult = await productRepository.GetProductByIdAsync(cartItem.ProductId);
            if (!productResult.IsSuccess || productResult.Data?.Status != ProductStatus.Active)
                return new Return<int>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.ProductNotFound,
                    InternalErrorMessage = productResult.InternalErrorMessage,
                    Data = 0,
                };

            var product = productResult.Data;
            decimal price;
            int stockQuantity;
            var productHasVariant = product.HasVariant;

            // Check if the product has a variant
            if (cartItem.ProductVariantId.HasValue)
            {
                if (!productHasVariant || product.ProductVariants.Count == 0)
                    return new Return<int>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.ProductNoVariant,
                        Data = 0,
                    };

                var productVariant = product.ProductVariants.FirstOrDefault(v =>
                    v.ProductVariantId == cartItem.ProductVariantId
                );
                if (
                    productVariant is not { Status: ProductStatus.Active }
                    || productVariant.Price <= 0
                )
                    return new Return<int>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.ProductNotFound,
                        Data = 0,
                    };

                price = productVariant.Price;
                stockQuantity = productVariant.StockQuantity;
            }
            else
            {
                if (product.Price <= 0)
                    return new Return<int>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidPrice,
                        Data = 0,
                    };

                price = product.Price;
                stockQuantity = product.StockQuantity;
            }

            // Check stock availability
            if (stockQuantity < cartItem.Quantity)
                return new Return<int>
                {
                    IsSuccess = false,
                    StatusCode = ErrorCode.QuantityExceedStock,
                    Data = stockQuantity,
                };

            // Check existing cart item
            var existingCartItem = await cartRepository.GetCartItemByProductIdAndUserIdAsync(
                cartItem.ProductId,
                cartItem.ProductVariantId,
                currentCustomer.Data.UserId
            );

            // Update or add cart item
            if (existingCartItem is { IsSuccess: true, Data: not null })
            {
                var newQuantity = existingCartItem.Data.Quantity + cartItem.Quantity;
                if (stockQuantity < newQuantity)
                    return new Return<int>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.QuantityExceedStock,
                        Data = stockQuantity,
                    };

                existingCartItem.Data.Quantity = newQuantity;
                existingCartItem.Data.Price = price;
                var updateResult = await cartRepository.UpdateCartItemAsync(existingCartItem.Data);
                if (!updateResult.IsSuccess)
                    return new Return<int>
                    {
                        IsSuccess = false,
                        StatusCode = updateResult.StatusCode,
                        Data = 0,
                        InternalErrorMessage = updateResult.InternalErrorMessage,
                    };

                transactionScope.Complete();
                return new Return<int>
                {
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                    Data = newQuantity,
                };
            }

            var newCartItem = new CartItem
            {
                ProductId = cartItem.ProductId,
                ProductVariantId = cartItem.ProductVariantId,
                UserId = currentCustomer.Data.UserId,
                Quantity = cartItem.Quantity,
                Price = price,
                CreatedAt = DateTime.Now
            };

            var addResult = await cartRepository.AddToCartAsync(newCartItem);
            if (!addResult.IsSuccess)
                return new Return<int>
                {
                    IsSuccess = false,
                    StatusCode = addResult.StatusCode,
                    Data = 0,
                    InternalErrorMessage = addResult.InternalErrorMessage,
                };

            transactionScope.Complete();
            return new Return<int>
            {
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                Data = cartItem.Quantity,
            };
        }
        catch (Exception ex)
        {
            return new Return<int>
            {
                Data = 0,
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

    public async Task<Return<bool>> RemoveCartItemByIdAsync(Guid cartId)
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

    public async Task<Return<bool>> RemoveCartItemByIdsAsync(List<Guid> cartIds)
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
            var deletedCart = await cartRepository.RemoveCartItemRangeByIdAsync(
                cartIds,
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

    public async Task<Return<bool>> ClearCartAsync()
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
