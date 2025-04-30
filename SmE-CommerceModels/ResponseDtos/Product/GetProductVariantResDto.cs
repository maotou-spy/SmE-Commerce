using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.ResponseDtos.Product;

public class GetProductVariantResDto
{
    public Guid ProductVariantId { get; set; }

    public string? Sku { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string? VariantImage { get; set; }

    public string Status { get; set; } = ProductStatus.Active;

    public List<GetVariantAttributeResDto> VariantAttributes { get; set; } = null!;
}
