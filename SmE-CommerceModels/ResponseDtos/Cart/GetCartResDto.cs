namespace SmE_CommerceModels.ResponseDtos.Cart;

public class GetCartResDto
{
    public Guid CartItemId { get; set; }

    public Guid ProductVariantId { get; set; }

    public string? ImageUrl { get; set; }

    public required string ProductName { get; set; }

    public required string ProductStatus { get; set; }

    public required string ProductSlug { get; set; }

    public required decimal Price { get; set; }

    public required int Quantity { get; set; }

    public int StockQuantity { get; set; }

    public bool IsPriceUpdated { get; set; } = false;
}
