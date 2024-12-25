using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Product;

public class AddProductVariantReqDto
{
    [Required(ErrorMessage = "Product is required")]
    public required Guid ProductId { get; set; }

    [Required(ErrorMessage = "Variant is required")]
    public required Guid VariantNameId { get; set; }

    public string? Sku { get; set; }

    public decimal Price { get; set; } = 0;

    [Required(ErrorMessage = "Stock Quantity is required")]
    public required int StockQuantity { get; set; }

    [Required(ErrorMessage = "Variant values are required")]
    public required List<ProductVariantValue> VariantValues { get; set; }

    public string? Status { get; set; }
}
