using SmE_CommerceModels.RequestDtos.Cart;

namespace SmE_CommerceModels.RequestDtos.Order;

public class OrderReqDto
{
    public List<CartItemReqDto> CartItems { get; set; } = null!;
    public Guid AddressId { get; set; }

}
