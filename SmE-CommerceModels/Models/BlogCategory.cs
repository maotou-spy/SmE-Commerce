using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmE_CommerceModels.Models;

public class BlogCategory
{
    [Key]
    [Column("blogCategoryId")]
    public Guid BlogCategoryId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    /// <summary>
    ///     Values: active, inactive, deleted
    /// </summary>
    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("createById")]
    public Guid? CreateById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    public Guid? ModifiedById { get; set; }

    [InverseProperty("BlogCategory")]
    public virtual ICollection<ContentCategoryMap> ContentCategoryMaps { get; set; } =
        new List<ContentCategoryMap>();

    [ForeignKey("CreateById")]
    [InverseProperty("BlogCategoryCreateBies")]
    public virtual User? CreateBy { get; set; }

    [ForeignKey("ModifiedById")]
    [InverseProperty("BlogCategoryModifiedBies")]
    public virtual User? ModifiedBy { get; set; }
}
