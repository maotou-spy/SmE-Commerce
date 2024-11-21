using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Cart;

public class CartItemReqDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Required] public int Quantity { get; set; } = 1;

    [Required]
    public decimal Price { get; set; }
}
