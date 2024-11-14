using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Product;

public class AddProductAttributeReqDto
{
    [Required(ErrorMessage = "Attribute name is required")]
    public required string AttributeName { get; set; }

    [Required(ErrorMessage = "Attribute value is required")]
    public required string AttributeValue { get; set; }
}
