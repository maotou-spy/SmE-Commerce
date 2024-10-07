namespace SmE_CommerceModels.Models;

public partial class Content : Common
{
    public Guid ContentId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content1 { get; set; }

    public Guid? AuthorId { get; set; }

    public Guid? ProductId { get; set; }

    /// <summary>
    /// Values: blog, facebook
    /// </summary>
    public string ExternalType { get; set; } = null!;

    /// <summary>
    /// blogId for blogs, facebookPostId for Facebook posts
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// Values: draft, pending, published, unpublished, deleted
    /// </summary>
    public string Status { get; set; } = null!;

    public DateTime? PublishedAt { get; set; }

    public virtual ICollection<ContentCategoryMap> ContentCategoryMaps { get; set; } = new List<ContentCategoryMap>();

    public virtual ICollection<ContentImage> ContentImages { get; set; } = new List<ContentImage>();

    public virtual ICollection<ContentProduct> ContentProducts { get; set; } = new List<ContentProduct>();
}
