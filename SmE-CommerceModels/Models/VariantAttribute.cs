using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmE_CommerceModels.Models;

public partial class VariantAttribute
{
    [Key]
    [Column("attributeId")]
    public Guid AttributeId { get; set; }

    [Column("productVariantId")]
    public Guid ProductVariantId { get; set; }

    [Column("value")]
    [StringLength(255)]
    public string Value { get; set; } = null!;

    [Column("createdAt", TypeName = "timestamp without time zone")]
    public DateTime? CreatedAt { get; set; }

    [Column("createdById")]
    public Guid? CreatedById { get; set; }

    [Column("modifiedAt", TypeName = "timestamp without time zone")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modifiedById")]
    public Guid? ModifiedById { get; set; }

    [Column("variantNameId")]
    public Guid VariantNameId { get; set; }

    [ForeignKey("CreatedById")]
    [InverseProperty("VariantAttributeCreatedBies")]
    public virtual User? CreatedBy { get; set; }

    [ForeignKey("ModifiedById")]
    [InverseProperty("VariantAttributeModifiedBies")]
    public virtual User? ModifiedBy { get; set; }

    [ForeignKey("ProductVariantId")]
    [InverseProperty("VariantAttributes")]
    public virtual ProductVariant ProductVariant { get; set; } = null!;

    [ForeignKey("VariantNameId")]
    [InverseProperty("VariantAttributes")]
    public virtual VariantName VariantName { get; set; } = null!;
}
