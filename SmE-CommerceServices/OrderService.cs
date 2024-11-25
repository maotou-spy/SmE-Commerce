using System.Transactions;
using SmE_CommerceModels.Enums;
using SmE_CommerceModels.Models;
using SmE_CommerceModels.RequestDtos.Order;
using SmE_CommerceModels.ReturnResult;
using SmE_CommerceRepositories.Interface;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices;

public class OrderService(IOrderRepository orderRepository, IHelperService helperService) : IOrderService
{
    // public async Task<Return<bool>> CreateOrder(OrderReqDto orderReqDto)
    // {
    //     using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
    //     try
    //     {
    //         var currentCustomer = await helperService.GetCurrentUserWithRoleAsync(RoleEnum.Customer);
    //         if (!currentCustomer.IsSuccess || currentCustomer.Data == null)
    //         {
    //             return new Return<bool>
    //             {
    //                 Data = false,
    //                 IsSuccess = false
    //             };
    //         }
    //
    //         // Tính tổng tiền và kiểm tra tồn kho sản phẩm trước khi tạo đơn
    //         decimal totalAmount = 0;
    //         foreach (var orderDetail in orderReqDto.OrderDetails)
    //         {
    //             var product = await productRepository.GetProductById(orderDetail.ProductId);
    //             if (product == null || product.Stock < orderDetail.Quantity)
    //             {
    //                 return new Return<bool>
    //                 {
    //                     Data = false,
    //                     IsSuccess = false
    //                 };
    //             }
    //
    //             totalAmount += orderDetail.Price * orderDetail.Quantity;
    //         }
    //
    //         // Áp dụng mã giảm giá nếu có
    //         decimal discountAmount = 0;
    //         if (orderReqDto.DiscountCodeId != null)
    //         {
    //             var discount = await discountRepository.GetDiscountById(orderReqDto.DiscountCodeId);
    //             if (discount != null && discount.IsValid())
    //             {
    //                 discountAmount = discount.CalculateDiscount(totalAmount);
    //             }
    //         }
    //
    //         var order = new Order
    //         {
    //             UserId = currentCustomer.Data.UserId,
    //             AddressId = orderReqDto.AddressId,
    //             ShippingCode = orderReqDto.ShippingCode,
    //             TotalAmount = totalAmount - discountAmount, // Tổng tiền sau khi giảm giá
    //             DiscountCodeId = orderReqDto.DiscountCodeId,
    //             DiscountAmount = discountAmount,
    //             Status = "Pending", // Trạng thái mặc định
    //             CreatedAt = DateTime.UtcNow,
    //             CreatedById = currentCustomer.Data.UserId,
    //         };
    //
    //         var orderResult = await orderRepository.CreateOrder(order);
    //         if (!orderResult.IsSuccess)
    //         {
    //             return new Return<bool>
    //             {
    //                 Data = false,
    //                 IsSuccess = false,
    //                 InternalErrorMessage = orderResult.InternalErrorMessage
    //             };
    //         }
    //
    //         // Lưu chi tiết đơn hàng
    //         foreach (var orderDetail in orderReqDto.OrderDetails)
    //         {
    //             var orderDetailResult = await orderRepository.CreateOrderDetail(new OrderDetail
    //             {
    //                 OrderId = order.Id,
    //                 ProductId = orderDetail.ProductId,
    //                 Quantity = orderDetail.Quantity,
    //                 Price = orderDetail.Price
    //             });
    //             if (!orderDetailResult.IsSuccess)
    //             {
    //                 return new Return<bool>
    //                 {
    //                     Data = false,
    //                     IsSuccess = false,
    //                     InternalErrorMessage = orderDetailResult.InternalErrorMessage
    //                 };
    //             }
    //
    //             // Cập nhật lại tồn kho
    //             await productRepository.DecreaseStock(orderDetail.ProductId, orderDetail.Quantity);
    //         }
    //
    //         scope.Complete();
    //         return new Return<bool>
    //         {
    //             Data = true,
    //             IsSuccess = true
    //         };
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         throw;
    //     }
    // }
}
