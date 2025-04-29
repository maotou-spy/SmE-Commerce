using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmE_CommerceModels.Models;

public class VariantAttribute
{
    [Key]
    [Column("attributeId")]
    public Guid AttributeId { get; set; }

    [Column("productVariantId")]
    public Guid ProductVariantId { get; set; }

    [Column("value")]
    [StringLength(255)]
    public string Value { get; set; } = null!;

    [Column("variantNameId")]
    public Guid VariantNameId { get; set; }

    [ForeignKey("ProductVariantId")]
    [InverseProperty("VariantAttributes")]
    public virtual ProductVariant ProductVariant { get; set; } = null!;

    [ForeignKey("VariantNameId")]
    [InverseProperty("VariantAttributes")]
    public virtual VariantName VariantName { get; set; } = null!;
}
