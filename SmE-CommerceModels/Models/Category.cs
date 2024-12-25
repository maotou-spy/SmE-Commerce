using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmE_CommerceModels.Models;

public class Category
{
    [Key]
    [Column("categoryId")]
    public Guid CategoryId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [StringLength(100)]
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

    [Column("slug")]
    [StringLength(255)]
    public string? Slug { get; set; }

    [ForeignKey("CreateById")]
    [InverseProperty("CategoryCreateBies")]
    public virtual User? CreateBy { get; set; }

    [ForeignKey("ModifiedById")]
    [InverseProperty("CategoryModifiedBies")]
    public virtual User? ModifiedBy { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<ProductCategory> ProductCategories { get; set; } =
        new List<ProductCategory>();
}
