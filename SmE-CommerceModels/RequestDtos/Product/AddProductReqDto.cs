using System.ComponentModel.DataAnnotations;
using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.RequestDtos.Product;

public class AddProductReqDto
{
    [Required(ErrorMessage = "Product name is required")]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Product price is required")]
    [RegularExpression(@"^\d+\.\d{0,2}$", ErrorMessage = "Invalid price")]
    public decimal Price { get; set; }

    [RegularExpression(@"^\d+$", ErrorMessage = "Invalid stock quantity")]
    public int StockQuantity { get; set; } = 0;

    [RegularExpression(@"^\d+$", ErrorMessage = "Invalid sold quantity")]
    public int SoldQuantity { get; set; } = 0;

    public bool IsTopSeller { get; set; } = false;

    [Required(ErrorMessage = "At least one product image is required")]
    public List<AddProductImageReqDto> Images { get; set; } = [];

    public List<AddProductAttributeReqDto>? Attributes { get; set; }

    [Required(ErrorMessage = "At least one category is required")]
    public List<Guid> CategoryIds { get; set; } = [];

    public string Status { get; set; } = ProductStatus.Active;

    public string? Slug { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public List<string>? MetaKeywords { get; set; }
}
