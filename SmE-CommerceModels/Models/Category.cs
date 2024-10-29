namespace SmE_CommerceModels.Models;

public partial class Category : Common
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? CategoryImage { get; set; }

    public string? CategoryImageHash { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public string? Slug { get; set; }

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}
