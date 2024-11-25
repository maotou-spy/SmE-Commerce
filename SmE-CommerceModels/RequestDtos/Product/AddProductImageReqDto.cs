using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Product;

public class AddProductImageReqDto
{
    [Required(ErrorMessage = "Image URL is required")]
    public required string Url { get; set; }

    public string? AltText { get; set; }
}