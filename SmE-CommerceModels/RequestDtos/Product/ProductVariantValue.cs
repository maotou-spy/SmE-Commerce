using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Product;

public class ProductVariantValue
{
    [Required(ErrorMessage = "Variant Value is required")]
    public required string VariantValue { get; set; }

    public string? VariantImage { get; set; }
}
