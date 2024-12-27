using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Product;

public class AddProductVariantReqDto
{
    // This version do not allow to add SKU
    // public string? Sku { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative.")]
    public decimal Price { get; set; } = 0;

    [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity must be non-negative.")]
    [Required(ErrorMessage = "Stock Quantity is required")]
    public required int StockQuantity { get; set; }

    public string? VariantImage { get; set; }

    [Required(ErrorMessage = "Variant values are required")]
    public required List<ProductVariantValueReqDto> VariantValues { get; set; }

    public bool Status { get; set; } = false;
}
