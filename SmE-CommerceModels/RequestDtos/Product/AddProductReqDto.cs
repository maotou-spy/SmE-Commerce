using System.ComponentModel.DataAnnotations;
using SmE_CommerceModels.Enums;

namespace SmE_CommerceModels.RequestDtos.Product;

public class AddProductReqDto
{
    [Required(ErrorMessage = "Product name is required")]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Product price is required")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Invalid price")]
    public decimal Price { get; set; }

    [RegularExpression(@"^\d+$", ErrorMessage = "Invalid stock quantity")]
    public int StockQuantity { get; set; } = 0;

    [RegularExpression(@"^\d+$", ErrorMessage = "Invalid sold quantity")]
    public int SoldQuantity { get; set; } = 0;

    public bool IsTopSeller { get; set; } = false;

    public List<AddProductImageReqDto> Images { get; set; } = [];

    public List<AddProductAttributeReqDto> Attributes { get; set; } = [];

    public List<Guid> CategoryIds { get; set; } = [];

    public string Status { get; set; } = ProductStatus.Active;

    public string? Slug { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public List<string> MetaKeywords { get; set; } = [];
}
