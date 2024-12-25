using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.VariantName;

public class VariantNameReqDto
{
    [Required(ErrorMessage = "Attribute name is required")]
    public required string VariantName { get; init; }
}
