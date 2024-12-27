using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Product;

public class UpdateProductVariantReqDto
{
    [Required(ErrorMessage = "Variant Value is required")]
    public required string VariantValue { get; set; }

    public string? Sku { get; set; }

    public decimal Price { get; set; } = 0;

    [Required(ErrorMessage = "Stock Quantity is required")]
    public required int StockQuantity { get; set; }

    [Required(ErrorMessage = "Variant values are required")]
    public required List<ProductVariantValueReqDto> VariantValues { get; set; }

    public string? Status { get; set; }
}
