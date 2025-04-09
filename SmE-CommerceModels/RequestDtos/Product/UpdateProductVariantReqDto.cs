using System.ComponentModel.DataAnnotations;
using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.RequestDtos.Product;

public class UpdateProductVariantReqDto
{
    [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative")]
    public decimal Price { get; set; } = 0;

    [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity must be non-negative")]
    public int StockQuantity { get; set; } = 0;

    public string? VariantImage { get; set; }

    public string Status { get; set; } = ProductStatus.Active;

    public List<ProductVariantValueReqDto>? VariantValues { get; set; }
}
