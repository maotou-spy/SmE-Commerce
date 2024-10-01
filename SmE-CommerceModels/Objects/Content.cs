using System;
using System.Collections.Generic;

namespace SmE_CommerceModels.Objects;

public partial class Content
{
    public uint ContentId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content1 { get; set; }

    public uint? AuthorId { get; set; }

    public uint? ProductId { get; set; }

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

    public DateTime? CreatedDate { get; set; }

    public uint? CreatedById { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public uint? ModifiedById { get; set; }

    /// <summary>
    /// Bring to homepage
    /// </summary>
    public ulong? IsFeature { get; set; }

    public virtual User? Author { get; set; }

    public virtual ICollection<ContentCategoryMap> ContentCategoryMaps { get; set; } = new List<ContentCategoryMap>();

    public virtual ICollection<ContentImage> ContentImages { get; set; } = new List<ContentImage>();

    public virtual Product? Product { get; set; }
}
