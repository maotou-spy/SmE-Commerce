using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Product;

public class ProductVariantValueReqDto
{
    [Required(ErrorMessage = "Variant Name is required")]
    public required Guid VariantNameId { get; set; }

    [Required(ErrorMessage = "Variant Value is required")]
    public required string VariantValue { get; set; }
}
