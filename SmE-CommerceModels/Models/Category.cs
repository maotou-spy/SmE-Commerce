namespace SmE_CommerceModels.Models;

public class Category
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? CategoryImage { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public Guid? CreateById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public Guid? ModifiedById { get; set; }

    public string? Slug { get; set; }

    public virtual User? CreateBy { get; set; }

    public virtual User? ModifiedBy { get; set; }

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}
