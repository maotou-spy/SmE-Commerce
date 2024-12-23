using System.ComponentModel.DataAnnotations;

namespace SmE_CommerceModels.RequestDtos.Category;

public class AddCategoryReqDto
{
    [Required]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Category name can only contain letters and spaces.")]
    public required string Name { get; set; } = null!;

    public string? Description { get; set; }
}