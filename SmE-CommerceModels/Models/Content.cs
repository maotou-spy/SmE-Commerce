namespace SmE_CommerceModels.Models;

public class Content
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

    public DateTime? CreatedAt { get; set; }

    public Guid? CreateById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public Guid? ModifiedById { get; set; }

    public string? Slug { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public List<string>? Keywords { get; set; }

    public int? ViewCount { get; set; }

    public string? ShortDescription { get; set; }

    public virtual ICollection<ContentCategoryMap> ContentCategoryMaps { get; set; } = new List<ContentCategoryMap>();

    public virtual ICollection<ContentImage> ContentImages { get; set; } = new List<ContentImage>();

    public virtual ICollection<ContentProduct> ContentProducts { get; set; } = new List<ContentProduct>();

    public virtual User? CreateBy { get; set; }

    public virtual User? ModifiedBy { get; set; }
}
