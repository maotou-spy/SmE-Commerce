using System.ComponentModel.DataAnnotations;
using SmE_CommerceModels.RequestDtos.Cart;

namespace SmE_CommerceModels.RequestDtos.Order;

public class CreateOrderReqDto
{
    [Required(ErrorMessage = "Address is required")]
    public Guid AddressId { get; set; }

    public Guid? DiscountCodeId { get; set; }

    [Required(ErrorMessage = "Total amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than zero")]
    public decimal TotalAmount { get; set; }

    [Required(ErrorMessage = "Subtotal is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Subtotal must be greater than zero")]
    public decimal SubTotal { get; set; }

    public string? Note { get; set; }

    public string? CancelReason { get; set; }

    public string? ReturnReason { get; set; }

    // [Required(ErrorMessage = "Order items are required")]
    // public List<CreateOrderItemReqDto> OrderItems { get; set; } = [];
}
