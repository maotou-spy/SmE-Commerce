using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Product;

public class ProductVariantReqDto
{
    // This version do not allow to add SKU
    // public string? Sku { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative.")]
    public decimal Price { get; init; } = 0;

    [Range(1, int.MaxValue, ErrorMessage = "Stock Quantity must be non-negative.")]
    [Required(ErrorMessage = "Stock Quantity is required")]
    public required int StockQuantity { get; init; }

    public string? VariantImage { get; set; }

    [Required(ErrorMessage = "Variant values are required")]
    public required List<ProductVariantValueReqDto> VariantValues { get; set; } = null!;

    // public bool Status { get; set; } = true;
}
