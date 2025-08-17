using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmE_CommerceModels.Models;

public class VariantName
{
    [Key]
    [Column("variantNameId")]
    public Guid VariantNameId { get; set; }

    [Column("variantName")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    [Column("createdById")]
    [StringLength(50)]
    public required string CreatedById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    [StringLength(50)]
    public string? ModifiedById { get; set; }

    [InverseProperty("VariantName")]
    public virtual ICollection<VariantAttribute> VariantAttributes { get; set; } =
        new List<VariantAttribute>();
}
