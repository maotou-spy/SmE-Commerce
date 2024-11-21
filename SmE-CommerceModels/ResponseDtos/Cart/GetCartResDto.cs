namespace SmE_CommerceModels.ResponseDtos.Cart;

public class GetCartResDto
{
    public Guid CartItemId { get; set; }

    public Guid ProductId { get; set; }

    public required string ProductName { get; set; }

    public required decimal Price { get; set; }

    public required int Quantity { get; set; }

    public bool IsPriceUpdated { get; set; } = false;
}
