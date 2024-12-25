using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmE_CommerceModels.Models;

public partial class VariantName
{
    [Key]
    [Column("variantNameId")]
    public Guid VariantNameId { get; set; }

    [Column("variantName")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("createdById")]
    public Guid? CreatedById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    public Guid? ModifiedById { get; set; }

    [ForeignKey("CreatedById")]
    [InverseProperty("VariantNameCreatedBies")]
    public virtual User? CreatedBy { get; set; }

    [ForeignKey("ModifiedById")]
    [InverseProperty("VariantNameModifiedBies")]
    public virtual User? ModifiedBy { get; set; }

    [InverseProperty("VariantName")]
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } =
        new List<ProductVariant>();
}
