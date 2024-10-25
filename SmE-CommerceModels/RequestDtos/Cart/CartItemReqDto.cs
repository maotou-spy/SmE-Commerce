using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Cart;

public class CartItemReqDto
{
    [Required]
    public string ProductId { get; set; } = null!;

    [Required]
    public int Quantity { get; set; }
}
