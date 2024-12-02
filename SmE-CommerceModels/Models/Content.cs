using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

[Index("Slug", Name = "Contents_slug_key", IsUnique = true)]
[Index("Status", Name = "idx_contents_status")]
public partial class Content
{
    [Key]
    [Column("contentId")]
    public Guid ContentId { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column("content")]
    public string Content1 { get; set; } = null!;

    [Column("authorId")]
    public Guid AuthorId { get; set; }

    [Column("productId")]
    public Guid? ProductId { get; set; }

    /// <summary>
    /// Values: blog, facebook
    /// </summary>
    [Column("externalType")]
    [StringLength(50)]
    public string ExternalType { get; set; } = null!;

    /// <summary>
    /// blogId for blogs, facebookPostId for Facebook posts
    /// </summary>
    [Column("externalId")]
    [StringLength(255)]
    public string? ExternalId { get; set; }

    /// <summary>
    /// Values: draft, pending, published, unpublished, deleted
    /// </summary>
    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("publishedAt", TypeName = "timestamp without time zone")]
    public DateTime PublishedAt { get; set; }

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("createById")]
    public Guid? CreateById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    public Guid? ModifiedById { get; set; }

    [Column("slug")]
    [StringLength(255)]
    public string Slug { get; set; } = null!;

    [Column("metaTitle")]
    [StringLength(255)]
    public string MetaTitle { get; set; } = null!;

    [Column("metaDescription")]
    public string MetaDescription { get; set; } = null!;

    [Column("keywords", TypeName = "character varying[]")]
    public List<string> Keywords { get; set; } = null!;

    [Column("viewCount")]
    public int? ViewCount { get; set; }

    [Column("shortDescription")]
    public string ShortDescription { get; set; } = null!;

    [InverseProperty("Content")]
    public virtual ICollection<ContentCategoryMap> ContentCategoryMaps { get; set; } = new List<ContentCategoryMap>();

    [InverseProperty("Content")]
    public virtual ICollection<ContentImage> ContentImages { get; set; } = new List<ContentImage>();

    [InverseProperty("Content")]
    public virtual ICollection<ContentProduct> ContentProducts { get; set; } = new List<ContentProduct>();

    [ForeignKey("CreateById")]
    [InverseProperty("ContentCreateBies")]
    public virtual User? CreateBy { get; set; }

    [ForeignKey("ModifiedById")]
    [InverseProperty("ContentModifiedBies")]
    public virtual User? ModifiedBy { get; set; }
}
