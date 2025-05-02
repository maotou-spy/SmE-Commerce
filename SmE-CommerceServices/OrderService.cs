using System.Transactions;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Order;
using SmE_CommerceModels.RequestDtos.Payment;
using SmE_CommerceModels.ResponseDtos.Order;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

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
            var order = await orderRepository.CustomerGetOrderByIdAsync(
                reqDto.OrderId,
                currentCustomerId
            );
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
                CreateById = currentCustomerId,
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

    private static string CreateFullAddress(Address? address)
    {
        if (address == null)
            return string.Empty;

        var addressParts = new List<string>
        {
            address.Address1.Trim(),
            address.Ward.Trim(),
            address.District.Trim(),
            address.City.Trim(),
        }
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .ToList();

        return string.Join(", ", addressParts);
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

                    if (productCart.Data.Price == null)
                        return new Return<bool>
                        {
                            Data = false,
                            IsSuccess = false,
                            StatusCode = ErrorCode.InvalidPrice,
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

            // Check payment method (no status check as per your requirement)
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

            var earnedPoints = await settingRepository.GetSettingByKeyAsync(
                SettingEnum.PointsConversionRate
            );

            if (earnedPoints is { IsSuccess: false, Data: null })
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = earnedPoints.StatusCode,
                    InternalErrorMessage = earnedPoints.InternalErrorMessage,
                };

            if (!int.TryParse(earnedPoints.Data?.Value, out var parsedPoint) || parsedPoint <= 0)
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InternalServerError,
                };

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
                CreateById = currentCustomer.Data.UserId,
                CreatedAt = DateTime.Now,
                PointsEarned = (int)(subTotal * parsedPoint / 100), // ex: 1% of total amount
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
                ModifiedById = currentCustomer.Data.UserId,
            };
            var orderStatusHistory = await orderRepository.CreateOrderStatusHistoryasync(orderHistory);
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
                    code.Data.ModifiedById = currentCustomer.Data.UserId;
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
                            currentCustomer.Data.ModifiedById = currentCustomer.Data.UserId;
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

    public async Task<Return<CustomerGetOrderDetailResDto>> GetOrderByIdAsync(Guid orderId)
    {
        try
        {
            // Validate user
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(
                RoleEnum.Customer
            );
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
                return new Return<CustomerGetOrderDetailResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentCustomer.StatusCode,
                    TotalRecord = 0,
                };

            // Get order
            var order = await orderRepository.CustomerGetOrderByIdAsync(
                orderId,
                currentCustomer.Data.UserId
            );
            if (!order.IsSuccess || order.Data == null)
                return new Return<CustomerGetOrderDetailResDto>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = order.StatusCode,
                    InternalErrorMessage = order.InternalErrorMessage,
                };

            // Map to response dto
            var orderDetail = new CustomerGetOrderDetailResDto
            {
                OrderId = order.Data.OrderId,
                OrderCode = order.Data.OrderCode,
                ReceiverName = order.Data.Address.ReceiverName,
                ReceiverPhone = order.Data.Address.ReceiverPhone,
                AddressFull = CreateFullAddress(order.Data.Address),
                ShippingCode = order.Data.ShippingCode,
                TotalAmount = order.Data.TotalAmount,
                ShippingFee = order.Data.ShippingFee,
                DiscountAmount = order.Data.DiscountAmount,
                PointUsed = order.Data.PointsUsed,
                PointsEarned = order.Data.PointsEarned,
                Note = order.Data.Note,
                SubTotal = order.Data.SubTotal,
                EstimatedDeliveryDate = order.Data.EstimatedDeliveryDate,
                ActualDeliveryDate = order.Data.ActualDeliveryDate,
                Status = order.Data.Status,
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
                            && x.ProductVariant.VariantAttributes.Any()
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

            return new Return<CustomerGetOrderDetailResDto>
            {
                Data = orderDetail,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = 1,
            };
        }
        catch (Exception e)
        {
            return new Return<CustomerGetOrderDetailResDto>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    #endregion

    #region Admin

    public async Task<Return<List<ManagerGetOrdersResDto>>> ManagerGetOrdersAsync(
        string statusFilter
    )
    {
        try
        {
            var currentUser = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Manager);
            if (!currentUser.IsSuccess || currentUser.Data == null)
                return new Return<List<ManagerGetOrdersResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = currentUser.StatusCode,
                    InternalErrorMessage = currentUser.InternalErrorMessage,
                };

            var orders = await orderRepository.GetOrdersByStatusAndUserIdAsync(null, statusFilter);
            if (!orders.IsSuccess || orders.Data == null)
                return new Return<List<ManagerGetOrdersResDto>>
                {
                    Data = null,
                    IsSuccess = false,
                    StatusCode = orders.StatusCode,
                    InternalErrorMessage = orders.InternalErrorMessage,
                };

            var orderList = orders
                .Data.Select(x => new ManagerGetOrdersResDto
                {
                    OrderId = x.OrderId,
                    UserId = x.UserId,
                    UserName = x.User.FullName,
                    AddressId = x.AddressId,
                    addressFull = CreateFullAddress(x.Address),
                    note = x.Note ?? null,
                    status = x.Status,
                    orderItems = x
                        .OrderItems.Select(ord => new GetOrderItemResDto
                        {
                            ProductId = ord.ProductId,
                            VariantId = ord.ProductVariantId ?? Guid.Empty,
                            Quantity = ord.Quantity,
                            Price = ord.Price,
                            ProductName = ord.ProductName,
                            VariantName =
                                ord
                                    is
                                {
                                    ProductVariantId: not null,
                                    ProductVariant.VariantAttributes: not null
                                }
                                && ord.ProductVariant.VariantAttributes.Count != 0
                                    ? string.Join(
                                        "-",
                                        ord.ProductVariant.VariantAttributes.Where(v =>
                                                v.Value != null
                                            )
                                            .Select(v => v.Value)
                                    )
                                    : ord.VariantName,
                            VariantImage = ord.Product.PrimaryImage,
                            OrderItemId = ord.OrderItemId,
                        })
                        .ToList(),
                })
                .ToList();

            return new Return<List<ManagerGetOrdersResDto>>
            {
                Data = orderList,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok,
                TotalRecord = orderList.Count,
            };
        }
        catch (Exception e)
        {
            return new Return<List<ManagerGetOrdersResDto>>
            {
                Data = null,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e,
            };
        }
    }

    #endregion
}
