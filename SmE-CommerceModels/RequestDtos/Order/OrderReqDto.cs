using System.ComponentModel.DataAnnotations;
using SmE_CommerceModels.RequestDtos.Cart;

namespace SmE_CommerceModels.RequestDtos.Order;

public class OrderReqDto
{
    [Required] public List<CartItemReqDto> CartItems { get; set; } = null!;

    public Guid? AddressId { get; set; }

    public string? ShippingCode { get; set; }

    public Guid? DiscountCodeId { get; set; }

    public string? Note { get; set; }

    public string? Reason { get; set; }
}