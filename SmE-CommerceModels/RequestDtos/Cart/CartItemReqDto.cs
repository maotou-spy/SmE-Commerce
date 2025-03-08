using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Cart;

public class CartItemReqDto
{
    [Required]
    public Guid ProductId { get; set; }

    public Guid? ProductVariantId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 1")]
    public int Quantity { get; set; } = 1;
}
