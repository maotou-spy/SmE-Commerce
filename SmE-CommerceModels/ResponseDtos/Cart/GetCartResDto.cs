using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.ResponseDtos.Cart;

public class GetCartResDto
{
    public Guid CartItemId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? ProductVariantId { get; set; }

    public string? ImageUrl { get; set; }

    public required string ProductName { get; set; }

    public decimal Price { get; set; }

    public required int Quantity { get; set; }

    public int StockQuantity { get; set; }

    public string Status { get; set; } = ProductStatus.Active;

    public string? ProductSlug { get; set; }

    public bool IsPriceUpdated { get; set; } = false;

    public bool IsQuantityUpdated { get; set; } = false;
}
