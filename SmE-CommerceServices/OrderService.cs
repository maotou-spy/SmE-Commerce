using System.Transactions;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Order;
using SmE_CommerceModels.RequestDtos.Payment;
using SmE_CommerceModels.ResponseDtos.Order;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;
using SmE_CommerceUtilities.Utils;

namespace SmE_CommerceServices;

public class OrderService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IUserRepository userRepository,
    IDiscountRepository discountRepository,
    IAddressRepository addressRepository,
    IPaymentRepository paymentRepository,
    ICartRepository cartRepository,
    ISettingRepository settingRepository,
    IHelperService helperService
) : IOrderService
{
    #region Private Methods

    private async Task<Return<bool>> CreatePaymentAsync(
        CreatePaymentReqDto reqDto,
        Guid currentCustomerId
    )
    {
        try
        {
            // validate order
            var order = await orderRepository.GetOrderByIdAsync(reqDto.OrderId, currentCustomerId);
            if (!order.IsSuccess || order.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = order.StatusCode,
                    InternalErrorMessage = order.InternalErrorMessage,
                };

            // validate amount
            if (reqDto.Amount <= 0)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidAmount,
                };

            var payment = new Payment
            {
                PaymentMethodId = reqDto.PaymentMethodId,
                OrderId = reqDto.OrderId,
                Amount = reqDto.Amount,
                Description = reqDto.Description ?? "",
                Status = reqDto.Status,
                CreatedAt = DateTime.Now,
                CreateById = order.Data.CreateById,
            };

            var result = await paymentRepository.CreatePaymentAsync(payment);
            if (!result.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage,
                };

            return new Return<bool>
            {
                Data = result.Data,
                IsSuccess = result.IsSuccess,
                StatusCode = result.StatusCode,
                InternalErrorMessage = result.InternalErrorMessage,
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    #endregion

    #region Order

    private const decimal ShippingFee = 25000;

    public async Task<Return<bool>> CustomerCreateOrderAsync(CreateOrderReqDto req)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            // Validate user
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(
                RoleEnum.Customer
            );
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentCustomer.StatusCode,
                    TotalRecord = 0,
                };

            // Check if order items are empty
            if (req.CartItemId.Count == 0)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.OrderItemNotFound,
                };

            var uniqueCartItemIds = req.CartItemId.Distinct().ToList();
            if (uniqueCartItemIds.Count == 0)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.OrderItemNotFound,
                };

            // Get cart items
            var orderItemsWithPrice =
                new List<(Guid ProductId, Guid? ProductVariantId, int Quantity, decimal Price)>();
            foreach (var item in req.CartItemId)
            {
                // 2 things will get from CartItem: ProductVariantId and Quantity
                var cartItem = await cartRepository.GetCartItemByCustomerIdAndIdForUpdateAsync(
                    currentCustomer.Data.UserId,
                    item
                );
                if (!cartItem.IsSuccess || cartItem.Data is null)
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = cartItem.StatusCode,
                        InternalErrorMessage = cartItem.InternalErrorMessage,
                    };

                // If cart have Variant ID
                if (cartItem.Data.ProductVariantId.HasValue)
                {
                    var variantCart = await productRepository.GetProductVariantByIdForUpdateAsync(
                        cartItem.Data.ProductVariantId
                    );
                    if (
                        !variantCart.IsSuccess
                        || variantCart.Data is null
                        || variantCart.Data.Status != ProductStatus.Active
                    )
                        return new Return<bool>
                        {
                            Data = false,
                            IsSuccess = false,
                            StatusCode = variantCart.StatusCode,
                            InternalErrorMessage = variantCart.InternalErrorMessage,
                        };

                    // Validate stock quantity
                    if (variantCart.Data.StockQuantity < cartItem.Data.Quantity)
                        return new Return<bool>
                        {
                            Data = false,
                            IsSuccess = false,
                            StatusCode = ErrorCode.OutOfStock,
                        };

                    orderItemsWithPrice.Add(
                        (
                            cartItem.Data.ProductId,
                            variantCart.Data.ProductVariantId,
                            cartItem.Data.Quantity,
                            variantCart.Data.Price
                        )
                    );
                }
                // If cart dont have Variant ID
                else if (!cartItem.Data.ProductVariantId.HasValue)
                {
                    var productCart = await productRepository.GetProductByIdForUpdateAsync(
                        cartItem.Data.ProductId
                    );
                    if (
                        !productCart.IsSuccess
                        || productCart.Data is null
                        || productCart.Data.Status != ProductStatus.Active
                    )
                        return new Return<bool>
                        {
                            Data = false,
                            IsSuccess = false,
                            StatusCode = productCart.StatusCode,
                            InternalErrorMessage = productCart.InternalErrorMessage,
                        };

                    // Validate stock quantity
                    if (productCart.Data.StockQuantity < cartItem.Data.Quantity)
                        return new Return<bool>
                        {
                            Data = false,
                            IsSuccess = false,
                            StatusCode = ErrorCode.OutOfStock,
                        };
                    orderItemsWithPrice.Add(
                        (
                            productCart.Data.ProductId,
                            cartItem.Data.ProductVariantId,
                            cartItem.Data.Quantity,
                            productCart.Data.Price
                        )
                    );
                }
            }

            // Calculate subtotal
            var subTotal = orderItemsWithPrice.Sum(x => x.Price * x.Quantity);
            if (subTotal <= 0)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidSubTotal,
                };

            // Shipping fee
            var shippingFeeResult = await settingRepository.GetSettingByKeyAsync(
                SettingEnum.ShippingFee
            );
            if (!shippingFeeResult.IsSuccess || shippingFeeResult.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = shippingFeeResult.StatusCode,
                    InternalErrorMessage = shippingFeeResult.InternalErrorMessage,
                };
            var shippingFee = decimal.TryParse(shippingFeeResult.Data.Value, out var parsedValue)
                ? parsedValue
                : ShippingFee;

            // Apply discount if exists
            var discountAmount = 0m;
            if (req.DiscountCodeId.HasValue)
            {
                var code = await discountRepository.GetDiscountCodeByIdForUpdateAsync(
                    req.DiscountCodeId.Value
                );
                if (
                    !code.IsSuccess
                    || code.Data is not { Status: DiscountCodeStatus.Active }
                    || code.Data.ToDate < DateTime.Now
                    || code.Data.FromDate > DateTime.Now
                )
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidDiscountCode,
                    };

                var discount = code.Data.Discount;
                if (discount.IsFirstOrder)
                {
                    var isFirstOrder = await orderRepository.GetOrderByUserIdAsync(
                        currentCustomer.Data.UserId
                    );
                    if (isFirstOrder.IsSuccess || isFirstOrder.Data?.Count > 0)
                        return new Return<bool>
                        {
                            Data = false,
                            IsSuccess = false,
                            StatusCode = ErrorCode.OnlyForTheNewUser,
                        };
                }

                if (discount.MinimumOrderAmount.HasValue && discount.MinimumOrderAmount > subTotal)
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.OrderAmountTooLow,
                    };

                if (discount.DiscountProducts.Count != 0)
                {
                    var eligibleProducts = discount
                        .DiscountProducts.Select(x => x.ProductId)
                        .ToList();
                    var productIds = orderItemsWithPrice.Select(x => x.ProductId).ToList();
                    if (!productIds.Any(x => eligibleProducts.Contains(x)))
                        return new Return<bool>
                        {
                            Data = false,
                            IsSuccess = false,
                            StatusCode = ErrorCode.InvalidDiscountCode,
                        };
                }

                if (
                    discount.MinQuantity.HasValue
                    && orderItemsWithPrice.Sum(x => x.Quantity) < discount.MinQuantity
                )
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidQuantity,
                    };

                if (
                    discount.MaxQuantity.HasValue
                    && orderItemsWithPrice.Sum(x => x.Quantity) > discount.MaxQuantity.Value
                )
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.ExceedMaxQuantity,
                    };

                discountAmount = discount.IsPercentage
                    ? subTotal * discount.DiscountValue / 100
                    : discount.DiscountValue;
                if (
                    discount.MaximumDiscount.HasValue
                    && discountAmount > discount.MaximumDiscount.Value
                )
                    discountAmount = discount.MaximumDiscount.Value;
            }

            // Calculate total amount
            var totalAmount = subTotal + shippingFee - discountAmount;
            if (totalAmount < 0)
                totalAmount = 0;

            // Check payment method
            var paymentMethod = await paymentRepository.GetPaymentMethodByIdAsync(
                req.PaymentMethodId
            );
            if (!paymentMethod.IsSuccess || paymentMethod.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.PaymentMethodNotFound,
                };

            // Check address
            var address = await addressRepository.GetAddressByIdAsync(req.AddressId);
            if (!address.IsSuccess || address.Data is not { Status: GeneralStatus.Active })
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.AddressNotFound,
                    TotalRecord = 0,
                };
            if (address.Data.UserId != currentCustomer.Data.UserId)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.NotYourAddress,
                };

            // Calculate points earned
            var pointsConversionRate = await settingRepository.GetSettingByKeyAsync(
                SettingEnum.PointsConversionRate
            );
            var convertedPoints = int.TryParse(pointsConversionRate.Data?.Value, out var points)
                ? points
                : 0;

            // Order items with product details
            var orderItems = new List<OrderItem>();
            foreach (var item in orderItemsWithPrice)
            {
                var productResult = await productRepository.GetProductByIdAsync(item.ProductId);
                var variantResult = item.ProductVariantId.HasValue
                    ? await productRepository.GetProductVariantByIdAsync(
                        item.ProductVariantId.Value
                    )
                    : null;

                if (!productResult.IsSuccess || productResult.Data == null)
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = productResult.StatusCode,
                        InternalErrorMessage = productResult.InternalErrorMessage,
                    };

                orderItems.Add(
                    new OrderItem
                    {
                        ProductVariantId = item.ProductVariantId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        ProductName = productResult.Data.Name,
                        VariantName = variantResult?.Data is not null
                            ? string.Join("-", variantResult.Data.VariantAttributes)
                            : "",
                        ProductId = item.ProductId, // Đảm bảo gán ProductId
                    }
                );
            }

            // Create order record
            var order = new Order
            {
                AddressId = req.AddressId,
                DiscountCodeId = req.DiscountCodeId,
                TotalAmount = totalAmount,
                SubTotal = subTotal,
                ShippingFee = shippingFee,
                DiscountAmount = discountAmount,
                Note = req.Note!.Trim(),
                OrderItems = orderItems,
                Status = OrderStatus.Pending,
                PointsUsed = Math.Min((int)totalAmount, currentCustomer.Data.Point),
                UserId = currentCustomer.Data.UserId,
                CreateById = currentCustomer.Data.Username,
                CreatedAt = DateTime.Now,
                PointsEarned = (int)((subTotal - discountAmount) * convertedPoints / 100), // ex: 1% of total amount
            };

            var result = await orderRepository.CreateOrderAsync(order);
            if (!result.IsSuccess || result.Data is null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage,
                };

            // update order status history
            var orderHistory = new OrderStatusHistory
            {
                OrderId = result.Data.OrderId,
                Status = OrderStatus.Pending,
                ModifiedAt = DateTime.Now,
                ModifiedById = currentCustomer.Data.Username,
            };
            var orderStatusHistory = await orderRepository.AddOrderStatusHistoryAsync(orderHistory);
            if (!orderStatusHistory.IsSuccess || orderStatusHistory.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = orderStatusHistory.StatusCode,
                    InternalErrorMessage = orderStatusHistory.InternalErrorMessage,
                };

            // update discount code status if this one can only be used once
            if (req.DiscountCodeId.HasValue)
            {
                var code = await discountRepository.GetDiscountCodeByIdForUpdateAsync(
                    req.DiscountCodeId.Value
                );
                if (code is { IsSuccess: true, Data: not null })
                {
                    code.Data.Status = DiscountCodeStatus.Used;
                    code.Data.ModifiedAt = DateTime.Now;
                    code.Data.ModifiedById = currentCustomer.Data.Username;
                    var updateResult = await discountRepository.UpdateDiscountCodeAsync(code.Data);
                    if (!updateResult.IsSuccess)
                        return new Return<bool>
                        {
                            Data = false,
                            IsSuccess = false,
                            StatusCode = updateResult.StatusCode,
                            InternalErrorMessage = updateResult.InternalErrorMessage,
                        };
                }
            }

            if (currentCustomer.Data.Point < 0)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidPointBalance,
                };

            // create payment record
            switch (req.IsUsingPoint)
            {
                // Handle points payment
                // Check if user has enough points to pay
                case true:
                {
                    var pointToUse = Math.Min((int)totalAmount, currentCustomer.Data.Point);
                    var remainingAmount = totalAmount - pointToUse;

                    // pay by points
                    if (pointToUse > 0)
                    {
                        var pointPayment = await CreatePaymentAsync(
                            new CreatePaymentReqDto
                            {
                                Amount = pointToUse,
                                OrderId = result.Data.OrderId,
                                PaymentMethodId = req.PaymentMethodId,
                                Status = PaymentStatus.Paid,
                            },
                            currentCustomer.Data.UserId
                        );
                        if (!pointPayment.IsSuccess)
                            return new Return<bool>
                            {
                                Data = false,
                                IsSuccess = false,
                                StatusCode = pointPayment.StatusCode,
                                InternalErrorMessage = pointPayment.InternalErrorMessage,
                            };

                        currentCustomer.Data.Point -= pointToUse;
                        currentCustomer.Data.ModifiedAt = DateTime.Now;
                        currentCustomer.Data.ModifiedById = currentCustomer.Data.Username;
                        var updatePointResult = await userRepository.UpdateUserAsync(
                            currentCustomer.Data
                        );
                        if (!updatePointResult.IsSuccess)
                            return new Return<bool>
                            {
                                Data = false,
                                IsSuccess = false,
                                StatusCode = updatePointResult.StatusCode,
                                InternalErrorMessage = updatePointResult.InternalErrorMessage,
                            };
                    }

                    // if pointToUse < totalAmount => create payment for remaining amount
                    if (remainingAmount > 0)
                    {
                        var remainingPayment = await CreatePaymentAsync(
                            new CreatePaymentReqDto
                            {
                                Amount = remainingAmount,
                                OrderId = result.Data.OrderId,
                                PaymentMethodId = req.PaymentMethodId,
                                Status = PaymentStatus.Pending, // payment method COD
                            },
                            currentCustomer.Data.UserId
                        );
                        if (!remainingPayment.IsSuccess)
                            return new Return<bool>
                            {
                                Data = false,
                                IsSuccess = false,
                                StatusCode = remainingPayment.StatusCode,
                                InternalErrorMessage = remainingPayment.InternalErrorMessage,
                            };
                    }

                    break;
                }
                default:
                {
                    var paymentResult = await CreatePaymentAsync(
                        new CreatePaymentReqDto
                        {
                            Amount = totalAmount,
                            OrderId = result.Data.OrderId,
                            PaymentMethodId = req.PaymentMethodId,
                            Status = PaymentStatus.Pending, // payment method COD
                        },
                        currentCustomer.Data.UserId
                    );
                    if (!paymentResult.IsSuccess || !paymentResult.Data)
                        return new Return<bool>
                        {
                            Data = false,
                            IsSuccess = false,
                            StatusCode = paymentResult.StatusCode,
                            InternalErrorMessage = paymentResult.InternalErrorMessage,
                        };
                    break;
                }
            }

            // Clear cart items
            var cartItems = await cartRepository.RemoveCartItemRangeByIdAsync(
                req.CartItemId,
                currentCustomer.Data.UserId
            );
            if (!cartItems.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = cartItems.StatusCode,
                    InternalErrorMessage = cartItems.InternalErrorMessage,
                };

            // Update stock quantity and sold quantity
            foreach (var item in orderItemsWithPrice)
                if (item.ProductVariantId.HasValue)
                {
                    var variant = await productRepository.GetProductVariantByIdForUpdateAsync(
                        item.ProductVariantId.Value
                    );
                    if (!variant.IsSuccess || variant.Data == null)
                        return new Return<bool>
                        {
                            IsSuccess = false,
                            StatusCode = variant.StatusCode,
                            InternalErrorMessage = variant.InternalErrorMessage,
                        };
                    variant.Data.StockQuantity -= item.Quantity;
                    variant.Data.SoldQuantity += item.Quantity;
                    if (variant.Data.StockQuantity == 0)
                        variant.Data.Status = ProductStatus.OutOfStock;
                    var updateResult = await productRepository.UpdateProductVariantAsync(
                        variant.Data
                    );
                    if (!updateResult.IsSuccess)
                        return new Return<bool>
                        {
                            IsSuccess = false,
                            StatusCode = updateResult.StatusCode,
                            InternalErrorMessage = updateResult.InternalErrorMessage,
                        };
                }
                else
                {
                    var product = await productRepository.GetProductByIdForUpdateAsync(
                        item.ProductId
                    );
                    if (!product.IsSuccess || product.Data == null)
                        return new Return<bool>
                        {
                            IsSuccess = false,
                            StatusCode = product.StatusCode,
                            InternalErrorMessage = product.InternalErrorMessage,
                        };
                    product.Data.StockQuantity -= item.Quantity;
                    product.Data.SoldQuantity += item.Quantity;
                    if (product.Data.StockQuantity == 0)
                        product.Data.Status = ProductStatus.OutOfStock;
                    var updateResult = await productRepository.UpdateProductAsync(product.Data);
                    if (!updateResult.IsSuccess)
                        return new Return<bool>
                        {
                            IsSuccess = false,
                            StatusCode = updateResult.StatusCode,
                            InternalErrorMessage = updateResult.InternalErrorMessage,
                        };
                }

            // Update user points
            currentCustomer.Data.Point += order.PointsEarned;
            var updatePoint = await userRepository.UpdateUserAsync(currentCustomer.Data);
            if (!updatePoint.IsSuccess)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = updatePoint.StatusCode,
                    InternalErrorMessage = updatePoint.InternalErrorMessage,
                };

            transaction.Complete();
            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    public async Task<Return<GetOrderDetailsResDto>> GetOrderByIdAsync(Guid orderId)
    {
        try
        {
            // Validate user
            var validUser = await helperService.GetCurrentUser();
            if (!validUser.IsSuccess || validUser.Data == null)
                return new Return<GetOrderDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = validUser.StatusCode,
                    TotalRecord = 0,
                };

            var userId =
                validUser.Data.Role == RoleEnum.Customer ? validUser.Data.UserId : (Guid?)null;
            // Get order
            var order = await orderRepository.GetOrderByIdAsync(orderId, userId);
            if (!order.IsSuccess || order.Data == null)
                return new Return<GetOrderDetailsResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = order.StatusCode,
                    InternalErrorMessage = order.InternalErrorMessage,
                };

            // Map to response dto
            var orderDetail = new GetOrderDetailsResDto
            {
                OrderId = order.Data.OrderId,
                OrderCode = order.Data.OrderCode,
                UserId = order.Data.UserId,
                FullName = order.Data.User.FullName,
                ReceiverName = order.Data.Address.ReceiverName,
                ReceiverPhone = order.Data.Address.ReceiverPhone,
                AddressFull = StringUtils.CreateFullAddressString(order.Data.Address),
                TotalAmount = order.Data.TotalAmount,
                ShippingFee = order.Data.ShippingFee,
                DiscountCode = order.Data.DiscountCode?.Code,
                DiscountAmount = order.Data.DiscountAmount,
                PointsEarned = order.Data.PointsEarned,
                PointsUsed = order.Data.PointsUsed,
                Note = order.Data.Note,
                SubTotal = order.Data.SubTotal,
                Status = order.Data.Status,
                CreateAt = order.Data.CreatedAt,
                CreatedByUserName = order.Data.CreateById,
                ModifiedAt = order.Data.ModifiedAt,
                ModifiedByUserName = order.Data.ModifiedById,
                OrderItems = order
                    .Data.OrderItems.Select(x => new GetOrderItemResDto
                    {
                        ProductId = x.ProductId,
                        VariantId = x.ProductVariantId,
                        Quantity = x.Quantity,
                        Price = x.Price,
                        ProductName = x.ProductName,
                        VariantName =
                            x is { ProductVariantId: not null, ProductVariant: not null }
                            && x.ProductVariant.VariantAttributes.Count != 0
                                ? string.Join(
                                    "-",
                                    x.ProductVariant.VariantAttributes.Select(v => v.Value)
                                )
                                : x.VariantName,
                        VariantImage = x.Product.PrimaryImage,
                        OrderItemId = x.OrderItemId,
                    })
                    .ToList(),
            };

            return new Return<GetOrderDetailsResDto>
            {
                Data = orderDetail,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception e)
        {
            return new Return<GetOrderDetailsResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    public async Task<Return<IEnumerable<GetOrderResDto>>> GetOrdersAsync(OrderFilterReqDto filter)
    {
        try
        {
            if (filter.PageNumber <= 0)
                filter.PageNumber = PagingEnum.PageNumber;

            if (filter.PageSize <= 0)
                filter.PageSize = PagingEnum.PageSize;

            // Step 1: Get current user
            var currentUserIdResult = await helperService.GetCurrentUser();
            if (!currentUserIdResult.IsSuccess || currentUserIdResult.Data == null)
                return new Return<IEnumerable<GetOrderResDto>>
                {
                    Data = [],
                    IsSuccess = false,
                    StatusCode = ErrorCode.UserNotFound,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalRecord = 0,
                };

            var userId = currentUserIdResult.Data.Role.Equals(RoleEnum.Manager)
                ? Guid.Empty
                : currentUserIdResult.Data.UserId;

            // Step 2: Validation
            if (
                filter is { FromDate: not null, ToDate: not null }
                && filter.FromDate > filter.ToDate
            )
                return new Return<IEnumerable<GetOrderResDto>>
                {
                    Data = [],
                    IsSuccess = false,
                    StatusCode = ErrorCode.BadRequest,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalRecord = 0,
                };

            // Step 3: Mapping filter to OrderFilter
            var orderResult = await orderRepository.GetOrdersAsync(filter, userId);
            if (!orderResult.IsSuccess || orderResult.Data == null)
                return new Return<IEnumerable<GetOrderResDto>>
                {
                    Data = [],
                    IsSuccess = false,
                    StatusCode = orderResult.StatusCode,
                    InternalErrorMessage = orderResult.InternalErrorMessage,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalRecord = orderResult.TotalRecord,
                };

            // Step 4: Map to GetOrderResDto with null check
            var mappedOrders = orderResult
                .Data.Select(x => new GetOrderResDto
                {
                    OrderId = x.OrderId,
                    OrderCode = x.OrderCode,
                    UserId = x.UserId,
                    UserName = x.User.FullName,
                    UserEmail = x.User.Email,
                    UserPhone = x.User.Phone,
                    TotalAmount = x.TotalAmount,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    ModifiedAt = x.ModifiedAt,
                })
                .ToList();

            return new Return<IEnumerable<GetOrderResDto>>
            {
                Data = mappedOrders,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecord = orderResult.TotalRecord,
            };
        }
        catch (Exception ex)
        {
            return new Return<IEnumerable<GetOrderResDto>>
            {
                Data = [],
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecord = 0,
            };
        }
    }

    // Manager update status from pending => stuffing, stuffing => Shipped, Shipped => Completed Or Pending => Rejected
    // Only from Pending or Stuffing can be updated to Rejected
    public async Task<Return<bool>> ManagerUpdateOrderStatusAsync(
        ManagerUpdateOrderStatusReqDto req
    )
    {
        try
        {
            // Validate current manager
            var validUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!validUser.IsSuccess || validUser.Data == null)
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = validUser.StatusCode,
                    InternalErrorMessage = validUser.InternalErrorMessage,
                };

            if (!req.OrderIds.Any() || string.IsNullOrEmpty(req.Status))
                return new Return<bool> { IsSuccess = false, StatusCode = ErrorCode.InvalidInput };

            var orders = await orderRepository.GetOrdersByIdsAsync(req.OrderIds);
            if (orders.Count == 0)
                return new Return<bool> { IsSuccess = false, StatusCode = ErrorCode.OrderNotFound };

            var validTransitions = new Dictionary<string, List<string>>
            {
                { OrderStatus.Pending, [OrderStatus.Stuffing, OrderStatus.Rejected] },
                { OrderStatus.Stuffing, [OrderStatus.Shipped, OrderStatus.Rejected] },
                // Managers cannot update Shipped -> Completed directly
                // { OrderStatus.Shipped, [OrderStatus.Completed] },
            };

            if (
                orders.Any(o =>
                    !validTransitions.TryGetValue(o.Status, out var targets)
                    || !targets.Contains(req.Status)
                )
            )
                return new Return<bool> { IsSuccess = false, StatusCode = ErrorCode.InvalidStatus };

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var newStatus = req.Status;
            var newReason = req.Reason?.Trim();
            var now = DateTime.Now;

            // Update orders
            foreach (var order in orders)
            {
                order.Status = newStatus;
                order.ModifiedAt = now;
                order.ModifiedById = validUser.Data.Username;
                order.OrderStatusHistories.Add(
                    new OrderStatusHistory
                    {
                        OrderId = order.OrderId,
                        Status = newStatus,
                        Reason = newReason,
                        ModifiedAt = now,
                        ModifiedById = validUser.Data.Username,
                    }
                );
            }

            var updateOrders = await orderRepository.UpdateOrderStatusRangeAsync(orders);
            if (!updateOrders.IsSuccess || !updateOrders.Data)
                return new Return<bool>
                {
                    IsSuccess = false,
                    StatusCode = updateOrders.StatusCode,
                    InternalErrorMessage = updateOrders.InternalErrorMessage,
                };

            // Nếu không phải rejected thì kết thúc tại đây
            if (newStatus != OrderStatus.Rejected)
            {
                transaction.Complete();
                return new Return<bool>
                {
                    IsSuccess = true,
                    StatusCode = ErrorCode.Ok,
                    TotalRecord = orders.Count,
                };
            }

            // Nếu là Rejected thì rollback tồn kho và point
            foreach (var item in orders.SelectMany(o => o.OrderItems))
                if (item.ProductVariantId.HasValue)
                {
                    var variantResult = await productRepository.GetProductVariantByIdForUpdateAsync(
                        item.ProductVariantId.Value
                    );
                    if (!variantResult.IsSuccess || variantResult.Data == null)
                        return new Return<bool>
                        {
                            IsSuccess = false,
                            StatusCode = variantResult.StatusCode,
                            InternalErrorMessage = variantResult.InternalErrorMessage,
                        };

                    var variant = variantResult.Data;
                    variant.StockQuantity += item.Quantity;
                    variant.SoldQuantity = Math.Max(0, variant.SoldQuantity - item.Quantity);
                    variant.ModifiedAt = now;
                    variant.ModifiedById = validUser.Data.Username;

                    var updateVariant = await productRepository.UpdateProductVariantAsync(variant);
                    if (!updateVariant.IsSuccess)
                        return new Return<bool>
                        {
                            IsSuccess = false,
                            StatusCode = updateVariant.StatusCode,
                            InternalErrorMessage = updateVariant.InternalErrorMessage,
                        };
                }
                else
                {
                    var productResult = await productRepository.GetProductByIdForUpdateAsync(
                        item.ProductId
                    );
                    if (!productResult.IsSuccess || productResult.Data == null)
                        return new Return<bool>
                        {
                            IsSuccess = false,
                            StatusCode = productResult.StatusCode,
                            InternalErrorMessage = productResult.InternalErrorMessage,
                        };

                    var product = productResult.Data;
                    product.StockQuantity += item.Quantity;
                    product.SoldQuantity = Math.Max(0, product.SoldQuantity - item.Quantity);
                    product.ModifiedAt = now;
                    product.ModifiedById = validUser.Data.Username;

                    if (product.StockQuantity > 0)
                        product.Status = ProductStatus.Active;

                    var updateProduct = await productRepository.UpdateProductAsync(product);
                    if (!updateProduct.IsSuccess)
                        return new Return<bool>
                        {
                            IsSuccess = false,
                            StatusCode = updateProduct.StatusCode,
                            InternalErrorMessage = updateProduct.InternalErrorMessage,
                        };
                }

            // Trả lại điểm
            foreach (var order in orders)
            {
                var userResult = await userRepository.GetUserByIdAsync(order.UserId);
                if (!userResult.IsSuccess || userResult.Data == null)
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = userResult.StatusCode,
                        InternalErrorMessage = userResult.InternalErrorMessage,
                    };

                if (order.PointsUsed < 0)
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidPointBalance,
                    };

                userResult.Data.Point += order.PointsUsed;
                userResult.Data.ModifiedAt = now;
                userResult.Data.ModifiedById = validUser.Data.Username;

                var updateUser = await userRepository.UpdateUserAsync(userResult.Data);
                if (!updateUser.IsSuccess)
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = updateUser.StatusCode,
                        InternalErrorMessage = updateUser.InternalErrorMessage,
                    };
            }

            transaction.Complete();
            return new Return<bool>
            {
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = orders.Count,
            };
        }
        catch (Exception ex)
        {
            return new Return<bool>
            {
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = ex,
            };
        }
    }

    public async Task<Return<bool>> CustomerUpdateOrderStatusAsync(
        CustomerUpdateOrderStatusReqDto req
    )
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Customer);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.UserNotFound,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var order = await orderRepository.GetOrderByIdAsync(
                req.OrderId,
                currentUser.Data.UserId
            );
            if (!order.IsSuccess || order.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.OrderNotFound,
                    InternalErrorMessage = order.InternalErrorMessage,
                };

            // Validate status
            var orderStatusValid = new List<string> { OrderStatus.Pending, OrderStatus.Shipped };
            if (!orderStatusValid.Contains(order.Data.Status))
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidStatus,
                };

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var newStatus =
                    order.Data.Status == OrderStatus.Pending
                        ? OrderStatus.Cancelled
                        : OrderStatus.Completed;

                order.Data.Status = newStatus;
                order.Data.ModifiedAt = DateTime.Now;
                order.Data.ModifiedById = currentUser.Data.Username;
                order.Data.OrderStatusHistories.Add(
                    new OrderStatusHistory
                    {
                        OrderId = req.OrderId,
                        Status = newStatus,
                        Reason = req.Reason?.Trim(),
                        ModifiedAt = DateTime.Now,
                        ModifiedById = currentUser.Data.Username,
                    }
                );

                var orderUpdated = await orderRepository.UpdateOrderAsync(order.Data);
                if (!orderUpdated.IsSuccess)
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = orderUpdated.StatusCode,
                        InternalErrorMessage = orderUpdated.InternalErrorMessage,
                    };

                switch (newStatus)
                {
                    case OrderStatus.Cancelled:
                    {
                        // Return products to stock
                        foreach (var item in order.Data.OrderItems)
                            if (item.ProductVariantId.HasValue)
                            {
                                var variant =
                                    await productRepository.GetProductVariantByIdForUpdateAsync(
                                        item.ProductVariantId.Value
                                    );
                                if (!variant.IsSuccess || variant.Data == null)
                                    return new Return<bool>
                                    {
                                        Data = false,
                                        IsSuccess = false,
                                        StatusCode = variant.StatusCode,
                                        InternalErrorMessage = variant.InternalErrorMessage,
                                    };
                                variant.Data.StockQuantity += item.Quantity;
                                variant.Data.SoldQuantity -= item.Quantity;
                                if (variant.Data.StockQuantity > 0)
                                    variant.Data.Status = ProductStatus.Active;
                                var updateResult =
                                    await productRepository.UpdateProductVariantAsync(variant.Data);
                                if (!updateResult.IsSuccess)
                                    return new Return<bool>
                                    {
                                        Data = false,
                                        IsSuccess = false,
                                        StatusCode = updateResult.StatusCode,
                                        InternalErrorMessage = updateResult.InternalErrorMessage,
                                    };
                            }
                            else
                            {
                                var product = await productRepository.GetProductByIdForUpdateAsync(
                                    item.ProductId
                                );
                                if (!product.IsSuccess || product.Data == null)
                                    return new Return<bool>
                                    {
                                        Data = false,
                                        IsSuccess = false,
                                        StatusCode = product.StatusCode,
                                        InternalErrorMessage = product.InternalErrorMessage,
                                    };
                                product.Data.StockQuantity += item.Quantity;
                                product.Data.SoldQuantity -= item.Quantity;
                                if (product.Data.StockQuantity > 0)
                                    product.Data.Status = ProductStatus.Active;
                                var updateResult = await productRepository.UpdateProductAsync(
                                    product.Data
                                );
                                if (!updateResult.IsSuccess)
                                    return new Return<bool>
                                    {
                                        Data = false,
                                        IsSuccess = false,
                                        StatusCode = updateResult.StatusCode,
                                        InternalErrorMessage = updateResult.InternalErrorMessage,
                                    };
                            }

                        if (order.Data.PointsUsed < 0)
                            return new Return<bool>
                            {
                                Data = false,
                                IsSuccess = false,
                                StatusCode = ErrorCode.BadRequest,
                            };

                        // Return PointsUsed và divide PointsEarned
                        currentUser.Data.Point += order.Data.PointsUsed;

                        break;
                    }
                    case OrderStatus.Completed:
                    {
                        // Update points earned
                        currentUser.Data.Point += order.Data.PointsEarned;

                        break;
                    }
                }

                var updatedUser = await userRepository.UpdateUserAsync(currentUser.Data);
                if (!updatedUser.IsSuccess)
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = updatedUser.StatusCode,
                        InternalErrorMessage = updatedUser.InternalErrorMessage,
                    };

                // Commit transaction
                transaction.Complete();

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
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    public async Task<Return<bool>> SystemAutoCompleteShippedOrdersAsync(int autoCompleteDays = 10)
    {
        try
        {
            // Get previously autoCompleteDays
            var tenDaysAgo = DateTime.Now.AddDays(-autoCompleteDays);

            // Update orders that are Shipped and created before 10 days ago
            var updatedOrders = await orderRepository.GetShippedOrdersBeforeDate(tenDaysAgo);
            if (!updatedOrders.IsSuccess || updatedOrders.Data == null)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = updatedOrders.StatusCode,
                    InternalErrorMessage = updatedOrders.InternalErrorMessage,
                };

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            foreach (var order in updatedOrders.Data)
            {
                // Update order status to Completed
                var now = DateTime.Now;
                order.Status = OrderStatus.Completed;
                order.ModifiedAt = now;
                order.ModifiedById = RoleEnum.System; // System user

                // Calculate points earned
                var pointsConversionRate = await settingRepository.GetSettingByKeyAsync(
                    SettingEnum.PointsConversionRate
                );
                var convertedPoints = int.TryParse(pointsConversionRate.Data?.Value, out var points)
                    ? points
                    : 0;
                order.PointsEarned = (int)
                    ((order.SubTotal - order.DiscountAmount) * convertedPoints / 100)!;

                // Update order
                var updateOrderResult = await orderRepository.UpdateOrderAsync(order);
                if (!updateOrderResult.IsSuccess || !updateOrderResult.Data)
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = updateOrderResult.StatusCode,
                        InternalErrorMessage = updateOrderResult.InternalErrorMessage,
                    };

                // Create order status history
                var history = new OrderStatusHistory
                {
                    OrderId = order.OrderId,
                    Status = OrderStatus.Completed,
                    Reason = "System auto-completed the order",
                    ModifiedAt = DateTime.Now,
                    ModifiedById = RoleEnum.System, // System user
                };
                var createHistoryResult = await orderRepository.AddOrderStatusHistoryAsync(history);
                if (!createHistoryResult.IsSuccess || createHistoryResult.Data == null)
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = createHistoryResult.StatusCode,
                        InternalErrorMessage = createHistoryResult.InternalErrorMessage,
                    };
            }

            // Commit transaction
            transaction.Complete();
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

    #endregion
}
