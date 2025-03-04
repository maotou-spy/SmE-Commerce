using System.Transactions;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Order;
using SmE_CommerceModels.RequestDtos.Payment;
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
            // // validate order
            // var order = await orderRepository.GetOrderByIdAsync(reqDto.OrderId);
            // if (!order.IsSuccess || order.Data == null)
            //     return new Return<bool>
            //     {
            //         Data = false,
            //         IsSuccess = false,
            //         StatusCode = order.StatusCode,
            //         InternalErrorMessage = order.InternalErrorMessage
            //     };

            // // validate amount
            // if (reqDto.Amount <= 0)
            //     return new Return<bool>
            //     {
            //         Data = false,
            //         IsSuccess = false,
            //         StatusCode = ErrorCode.InvalidAmount
            //     };

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
            if (!req.OrderItems.Any())
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.OrderItemNotFound,
                };

            var variantDictList = new List<Dictionary<ProductVariant, Product>>();
            List<Guid> productIds = [];
            var orderItemsWithPrice = new List<(Guid VariantId, int Quantity, decimal Price)>();
            foreach (var item in req.OrderItems)
            {
                if (item.Quantity <= 0)
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidQuantity,
                    };

                var product = await productRepository.GetProductByVariantIdForUpdateAsync(
                    item.VariantId
                );
                if (!product.IsSuccess || product.Data is not { Status: ProductStatus.Active })
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.ProductNotFound,
                    };
                if (product.Data.StockQuantity < item.Quantity)
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.OutOfStock,
                    };

                productIds.Add(product.Data.ProductId);

                var variant = product.Data.ProductVariants.SingleOrDefault(x =>
                    x.ProductVariantId == item.VariantId
                );
                if (variant == null)
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.ProductNotFound,
                    };

                variantDictList.Add(
                    new Dictionary<ProductVariant, Product> { { variant, product.Data } }
                );

                orderItemsWithPrice.Add((item.VariantId, item.Quantity, product.Data.Price));
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
                    if (!isFirstOrder.IsSuccess || isFirstOrder.Data?.Count > 0)
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

            // Create order record
            var order = new Order
            {
                AddressId = req.AddressId,
                DiscountCodeId = req.DiscountCodeId,
                TotalAmount = totalAmount,
                SubTotal = subTotal,
                ShippingFee = shippingFee,
                Discountamount = discountAmount,
                Note = req.Note,
                OrderItems = orderItemsWithPrice
                    .Select(
                        (item, index) =>
                        {
                            var variantDict = variantDictList[index];
                            var variant = variantDict.Keys.First();
                            var product = variantDict.Values.First();

                            return new OrderItem
                            {
                                VariantId = item.VariantId,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                ProductName = product.Name,
                                VariantName =
                                    variant.Sku
                                    ?? (
                                        variant.VariantAttributes.Count != 0
                                            ? string.Join(
                                                ", ",
                                                variant.VariantAttributes.Select(v => v.Value)
                                            )
                                            : "DefaultVariant"
                                    ),
                            };
                        }
                    )
                    .ToList(),
                Status = OrderStatus.Pending,
                UserId = currentCustomer.Data.UserId,
                CreateById = currentCustomer.Data.UserId,
                PointsEarned = (int)(subTotal * parsedPoint / 100), // ex: 1% of total amount,
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

            // create payment record
            switch (req.IsUsingPoint)
            {
                // Handle points payment
                // Check if user has not enough points => return error
                case true when currentCustomer.Data.Point < 0:
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidPointBalance,
                    };

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
            var cartItems = await cartRepository.ClearCartByUserIdAsync(
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
            {
                var variant = await productRepository.GetProductVariantByIdForUpdateAsync(
                    item.VariantId
                );
                variant.Data!.StockQuantity -= item.Quantity;
                variant.Data.SoldQuantity += item.Quantity;
                if (variant.Data.StockQuantity == 0)
                    variant.Data.Status = ProductStatus.OutOfStock;
                var updateResult = await productRepository.UpdateProductVariantAsync(variant.Data);
                if (!updateResult.IsSuccess)
                    return new Return<bool>
                    {
                        IsSuccess = false,
                        StatusCode = updateResult.StatusCode,
                        InternalErrorMessage = updateResult.InternalErrorMessage,
                    };
            }

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

    #endregion
}
