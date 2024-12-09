using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.VariantAttribute;

public class VariantReqDto
{
    [Required(ErrorMessage = "Attribute name is required")]
    public required string AttributeName { get; init; }
}
