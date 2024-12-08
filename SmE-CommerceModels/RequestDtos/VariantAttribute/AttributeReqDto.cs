using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.VariantAttribute;

public class AttributeReqDto
{
    [Required(ErrorMessage = "Attribute name is required")]
    public required string AttributeName { get; init; }
}
