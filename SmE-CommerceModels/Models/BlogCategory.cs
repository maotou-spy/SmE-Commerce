namespace SmE_CommerceModels.Models;

public partial class BlogCategory : Common
{
    public Guid BlogCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    /// <summary>
    /// Values: active, inactive, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public virtual ICollection<ContentCategoryMap> ContentCategoryMaps { get; set; } = new List<ContentCategoryMap>();
}
