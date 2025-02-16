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
    IHelperService helperService
) : IOrderService
{
    #region Order
    private const decimal ShippingFee = 25000;
    public async Task<Return<bool>> CustomerCreateOrderAsync(CreateOrderReqDto req)
    {
        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        try
        {
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Customer);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentCustomer.StatusCode,
                    TotalRecord = 0
                };
            }
            
            // Check if order items are empty
            if (req.OrderItems.Count == 0 || !req.OrderItems.Any())
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.OrderItemNotFound
                };
            }
            
            foreach (var item in req.OrderItems)
            {
                // Check product isExist?
                var product = await productRepository.GetProductByIdForUpdateAsync(item.ProductId);
                if (!product.IsSuccess || product.Data == null)
                {
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.ProductNotFound
                    };
                }
                if(product.Data.StockQuantity < item.Quantity)
                {
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.OutOfStock
                    };
                }

                // Check variant isExist? (if variantId is not null)
                if (item.VariantId != Guid.Empty)
                {
                    var variant = await productRepository.GetProductVariantsByProductIdAsync(item.ProductId);
                    if (!variant.IsSuccess || variant.Data == null)
                    {
                        return new Return<bool>
                        {
                            Data = false,
                            IsSuccess = false, 
                            StatusCode = ErrorCode.ProductVariantNotFound
                        };
                    }
                }

                // Check quantity > 0
                if (item.Quantity <= 0)
                {
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidQuantity
                    };
                }
            }
            
            // Calculate and validate subtotal
            var calculatedSubTotal = req.OrderItems.Sum(x => x.Price * x.Quantity);
            if (calculatedSubTotal != req.SubTotal)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidSubTotal
                };
            }

            // Calculate and validate total amount 
            var calculatedTotalAmount = calculatedSubTotal + ShippingFee;
            if (calculatedTotalAmount != req.TotalAmount)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidTotalAmount
                };
            }
            
            // Check discount code if exists and valid
            if (req.DiscountCodeId != null)
            {
                var discount = await discountRepository.GetDiscountByIdAsync(req.DiscountCodeId.Value);
                if (!discount.IsSuccess || discount.Data == null)
                {
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.DiscountCodeNotFound
                    };
                }
                
                // Check if discount has expired
                if (discount.Data!.ToDate < DateTime.Now)
                {
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidDiscountCode
                    };
                }
                
                // Check if discount has exceeded usage limit
                if (discount.Data!.UsageLimit != null && discount.Data!.UsageLimit <= 0)
                {
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidDiscountCode
                    };
                }
                
                // Check minimum order amount > calculatedSubTotal
                if (discount.Data!.MinimumOrderAmount > calculatedSubTotal)
                {
                    return new Return<bool>
                    {
                        Data = false,
                        IsSuccess = false,
                        StatusCode = ErrorCode.InvalidDiscountCode
                    };
                }
            }
            
            // Check if payment method exists
            var paymentMethod = await paymentRepository.GetPaymentMethodByIdAsync(req.PaymentMethodId);
            if (!paymentMethod.IsSuccess || paymentMethod.Data == null)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.PaymentMethodNotFound
                };
            }
            
            // Check if total amount is correct (> 0)
            if (req.TotalAmount <= 0)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidTotalAmount
                };
            }
            
            // Check if address exists and check if address belongs to current user
            var address = await addressRepository.GetAddressByIdAsync(req.AddressId);
            if (!address.IsSuccess || address.Data == null)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.AddressNotFound,
                    TotalRecord = 0
                };
            }
            if (address.Data.UserId != currentCustomer.Data.UserId)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.NotYourAddress
                };
            }
            
            // Check if user has enough point to use
            if (req.IsUsingPoint && currentCustomer.Data.Point <= 0)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidPoint
                };
            }
            
            // Create order record
            var order = new Order
            {
                AddressId = req.AddressId,
                DiscountCodeId = req.DiscountCodeId,
                TotalAmount = calculatedTotalAmount,
                SubTotal = calculatedSubTotal,  
                Note = req.Note,
                OrderItems = req.OrderItems.Select(x => new OrderItem
                {
                    ProductId = x.ProductId,
                    VariantId = x.VariantId,
                    Quantity = x.Quantity,
                    Price = x.Price
                }).ToList(),
                UserId = currentCustomer.Data.UserId,
                CreateById = currentCustomer.Data.UserId
            };
            
            var result = await orderRepository.CreateOrderAsync(order);
            if (!result.IsSuccess)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage
                };
            }
            
            // Check if isUsingPoint is true => create 2 payment records (1 for using point, 1 for using cod)
            if (req.IsUsingPoint)
            {
                decimal pointToUse;
                decimal remainingAmount;
                
                // Ở đây có thể xảy ra 2TH đó là point >= totalAmount hoặc point < totalAmount
                if (currentCustomer.Data.Point >= calculatedTotalAmount)
                {
                    // Nếu point nhiều hơn totalAmount, chỉ dùng đủ point để thanh toán
                    pointToUse = calculatedTotalAmount;
                    remainingAmount = 0;
        
                    // Update user point (trừ đi số point đã dùng)
                    var user = await userRepository.UpdateUser(new User
                    {
                        UserId = currentCustomer.Data.UserId,
                        Point = (int)(currentCustomer.Data.Point - calculatedTotalAmount),  // Chỉ trừ đi số point đã dùng
                        ModifiedAt = DateTime.Now,
                        ModifiedById = currentCustomer.Data.UserId
                    });
                }
                else
                {
                    // Nếu point ít hơn totalAmount, dùng hết point và trả thêm tiền
                    pointToUse = currentCustomer.Data.Point;
                    remainingAmount = calculatedTotalAmount - currentCustomer.Data.Point;
        
                    // Update user point (clear point vì đã dùng hết)
                    var user = await userRepository.UpdateUser(new User
                    {
                        UserId = currentCustomer.Data.UserId,
                        Point = 0,
                        ModifiedAt = DateTime.Now,
                        ModifiedById = currentCustomer.Data.UserId
                    });
                }

                // Create payment record for using point
                await CreatePaymentAsync(
                    new CreatePaymentReqDto
                    {
                        Amount = pointToUse,
                        OrderId = result.Data!.OrderId,
                        PaymentMethodId = req.PaymentMethodId,
                        Status = PaymentStatus.Pending
                    });

                // Create payment record for remaining amount (if any)
                if (remainingAmount > 0)
                {
                    await CreatePaymentAsync(
                        new CreatePaymentReqDto
                        {
                            Amount = remainingAmount,
                            OrderId = result.Data!.OrderId,
                            PaymentMethodId = req.PaymentMethodId,
                            Status = PaymentStatus.Pending
                        });
                }
            }
            else
            {
                // Create payment record for only using cod
                await CreatePaymentAsync
                (
                    new CreatePaymentReqDto
                    {
                        Amount = calculatedTotalAmount,
                        OrderId = result.Data!.OrderId,
                        PaymentMethodId = req.PaymentMethodId,
                        Status = PaymentStatus.Pending
                    });
            }
            
            // Clear cart items
            var cartItems = await cartRepository.ClearCartByUserIdAsync(currentCustomer.Data.UserId);
            if (!cartItems.IsSuccess)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = cartItems.StatusCode,
                    InternalErrorMessage = cartItems.InternalErrorMessage
                };
            }
            
            // Update stock Quantity and soldQuantity
            
            transaction.Complete();
            
            return new Return<bool>
            {
                Data = true,
                IsSuccess = true,
                StatusCode = ErrorCode.Ok
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e
            };
        }
    }

    #endregion

    #region Private Methods
    
    private async Task<Return<bool>> CreatePaymentAsync(CreatePaymentReqDto reqDto)
    {
        try
        {
            // validate user
            var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Customer);
            if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = currentCustomer.StatusCode,
                    TotalRecord = 0
                };
            }
            
            // validate payment method
            var paymentMethod = await paymentRepository.GetPaymentMethodByIdAsync(reqDto.PaymentMethodId);
            if (!paymentMethod.IsSuccess || paymentMethod.Data == null)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = paymentMethod.StatusCode,
                    InternalErrorMessage = paymentMethod.InternalErrorMessage
                };
            }
            
            // validate order
            var order = await orderRepository.GetOrderByIdAsync(reqDto.OrderId);
            if (!order.IsSuccess || order.Data == null)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = order.StatusCode,
                    InternalErrorMessage = order.InternalErrorMessage
                };
            }    
            
            // validate amount
            if (reqDto.Amount <= 0)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = ErrorCode.InvalidAmount
                };
            }
            
            var payment = new Payment
            {
                PaymentMethodId = reqDto.PaymentMethodId,
                OrderId = reqDto.OrderId,
                Amount = reqDto.Amount,
                Status = reqDto.Status,
                CreatedAt = DateTime.Now,
                CreateById = currentCustomer.Data.UserId
            };
            
            var result = await paymentRepository.CreatePaymentAsync(payment);
            if (!result.IsSuccess)
            {
                return new Return<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    StatusCode = result.StatusCode,
                    InternalErrorMessage = result.InternalErrorMessage
                };
            }
            
            return new Return<bool>
            {
                Data = result.Data,
                IsSuccess = result.IsSuccess,
                StatusCode = result.StatusCode,
                InternalErrorMessage = result.InternalErrorMessage
            };
        }
        catch (Exception e)
        {
            return new Return<bool>
            {
                Data = false,
                IsSuccess = false,
                StatusCode = ErrorCode.InternalServerError,
                InternalErrorMessage = e
            };
        }
    }
    
    #endregion
}
