using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Order;

public class CreateOrderReqDto
{
    [Required(ErrorMessage = "Address is required")]
    public Guid AddressId { get; set; }

    public Guid? DiscountCodeId { get; set; }

    public string? Note { get; set; }

    public bool IsUsingPoint { get; set; } = false;

    [Required]
    public required Guid PaymentMethodId { get; set; }

    [Required(ErrorMessage = "Order items are required")]
    public List<CreateOrderItemReqDto> OrderItems { get; set; } = [];
}
